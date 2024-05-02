using System.Data;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Stats;

namespace RecordStore.Api.Services.Stats;

public class StatsService : IStatsService
{
    private readonly RecordStoreContext _context;

    public StatsService(RecordStoreContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDateStats>> GetOrderStatsAsync(string period)
    {
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        
        var granularity = GetGranularity(period);
        command.CommandType = CommandType.Text;
        command.CommandText = $"SELECT * FROM get_order_stats(@granularity)";
        
        var granularityParam = new NpgsqlParameter("@granularity", NpgsqlDbType.Text) {Value = granularity};
        command.Parameters.Add(granularityParam);
        
        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);
        
        return MapToOrderStats(dataTable, period);
    }

    public async Task<List<FinancialDateStats>> GetFinancialStatsAsync(string period)
    {
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        
        var granularity = GetGranularity(period);
        command.CommandType = CommandType.Text;
        command.CommandText = $"SELECT * FROM get_financial_stats(@granularity)";

        var granularityParam = new NpgsqlParameter("@granularity", NpgsqlDbType.Text) {Value = granularity};
        command.Parameters.Add(granularityParam);
        
        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);

        return MapToFinancialStats(dataTable, period);
    }

    public async Task<FinancialStats> GetFinancialSummaryAsync()
    {
        await using var command = _context.Database.GetDbConnection().CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT * FROM get_financial_summary()";

        await _context.Database.OpenConnectionAsync();

        await using var result = await command.ExecuteReaderAsync();

        await result.ReadAsync();

        return new FinancialStats
        {
            TotalOrders = Convert.ToInt32(result["total_orders"]),
            TotalIncome = Convert.ToDecimal(result["total_income"]),
            TotalExpenses = Convert.ToDecimal(result["total_expenses"]),
            NetIncome = Convert.ToDecimal(result["net_income"])
        };
    }

    public async Task<List<OrderDateStats>> GetOrderStatsAsync(int id, string period)
    {
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        
        var granularity = GetGranularity(period);
        command.CommandType = CommandType.Text;
        command.CommandText = $"SELECT * FROM get_order_count_by_product(@id, @granularity)";
        
        var idParam = new NpgsqlParameter("@id", NpgsqlDbType.Integer) {Value = id};
        var granularityParam = new NpgsqlParameter("@granularity", NpgsqlDbType.Text) {Value = granularity};
        command.Parameters.Add(idParam);
        command.Parameters.Add(granularityParam);
        
        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);
        
        return MapToOrderStats(dataTable, period);
    }

    public async Task<List<ProductQuantitySoldStats>> GetProductQuantitySoldStatsAsync(int id, string period)
    {
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        
        var granularity = GetGranularity(period);
        command.CommandType = CommandType.Text;
        command.CommandText = $"SELECT * FROM get_product_quantity_sold(@id, @granularity)";
        
        var idParam = new NpgsqlParameter("@id", NpgsqlDbType.Integer) {Value = id};
        var granularityParam = new NpgsqlParameter("@granularity", NpgsqlDbType.Text) {Value = granularity};
        command.Parameters.Add(idParam);
        command.Parameters.Add(granularityParam);
        
        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);
        
        return MapToProductQuantitySoldStats(dataTable, period);
    }

    public async Task<List<AverageOrderValueStats>> GetAverageOrderValueStatsAsync(string period)
    {
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        
        var granularity = GetGranularity(period);
        command.CommandType = CommandType.Text;
        command.CommandText = $"SELECT * FROM get_average_order_value(@granularity)";
        
        var granularityParam = new NpgsqlParameter("@granularity", NpgsqlDbType.Text) {Value = granularity};
        command.Parameters.Add(granularityParam);
        
        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);
        
        return MapToAverageOrderValueStats(dataTable, period);
    }

    public async Task<List<OrdersPerRegionStats>> GetOrdersPerRegionStatsAsync()
    {
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        
        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT * FROM get_total_orders_by_region()";
        
        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);
        
        return MapToOrdersPerRegionStats(dataTable);
    }
    
    private List<OrdersPerRegionStats> MapToOrdersPerRegionStats(DataTable dataTable)
    {
        return (from DataRow row in dataTable.Rows
            select new OrdersPerRegionStats
            {
                Region = row["region"].ToString(),
                OrdersCount = Convert.ToInt32(row["total_orders"])
            }).ToList();
    }

    private List<FinancialDateStats> MapToFinancialStats(DataTable dataTable, string period)
    {
        return (from DataRow row in dataTable.Rows
            select new FinancialDateStats
            {
                Date = GetFormattedDate(row, period),
                TotalIncome = Convert.ToDecimal(row["revenue"]),
                TotalExpenses = Convert.ToDecimal(row["expenses"]),
                NetIncome = Convert.ToDecimal(row["profit"])
            }).ToList();
    }

    private List<OrderDateStats> MapToOrderStats(DataTable dataTable, string period)
    {
        return (from DataRow row in dataTable.Rows
            select new OrderDateStats
            {
                Date = GetFormattedDate(row, period),
                TotalOrders = Convert.ToInt32(row["num_orders"])
            }).ToList();
    }
    
    private List<ProductQuantitySoldStats> MapToProductQuantitySoldStats(DataTable dataTable, string period)
    {
        return (from DataRow row in dataTable.Rows
            select new ProductQuantitySoldStats
            {
                Date = GetFormattedDate(row, period),
                QuantitySold = Convert.ToInt32(row["quantity_sold"])
            }).ToList();
    }
    
    private List<AverageOrderValueStats> MapToAverageOrderValueStats(DataTable dataTable, string period)
    {
        return (from DataRow row in dataTable.Rows
            select new AverageOrderValueStats
            {
                Date = GetFormattedDate(row, period),
                AverageOrderValue = Convert.ToDecimal(row["average_order_value"])
            }).ToList();
    }

    private string? GetFormattedDate(DataRow row, string period)
    {
        return period switch
        {
            "year" => row["year"].ToString(),
            "month" => CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(
                Convert.ToInt32(row["period"])) + " " + row["year"],
            "week" => "Week " + row["period"] + " " + row["year"],
            _ => throw new ArgumentException("Invalid period", nameof(period))
        };
    }
    
    private string GetGranularity(string period)
    {
        return period switch
        {
            "year" => "yearly",
            "month" => "monthly",
            "week" => "weekly",
            _ => throw new ArgumentException("Invalid period", nameof(period))
        };
    }
}