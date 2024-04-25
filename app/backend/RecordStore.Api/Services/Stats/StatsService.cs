using System.Data;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
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
        var functionName = GetOrderFunctionName(period);
        await using var command = _context.Database.GetDbConnection().CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = $"SELECT * FROM {functionName}()";

        await _context.Database.OpenConnectionAsync();

        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);

        return MapToOrderStats(dataTable, period);
    }

    public async Task<List<FinancialDateStats>> GetFinancialStatsAsync(string period)
    {
        var functionName = GetFinancialFunctionName(period);
        await using var command = _context.Database.GetDbConnection().CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = $"SELECT * FROM {functionName}()";

        await _context.Database.OpenConnectionAsync();

        await using var result = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(result);

        return MapToFinancialStats(dataTable, period);
    }

    public async Task<FinancialStats> GetFinancialStatsAsync()
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

    private static string GetFinancialFunctionName(string period)
    {
        return period switch
        {
            "year" => "get_yearly_financial_stats",
            "month" => "get_monthly_financial_stats",
            "week" => "get_weekly_financial_stats",
            _ => throw new ArgumentException("Invalid period", nameof(period))
        };
    }

    private static string GetOrderFunctionName(string period)
    {
        return period switch
        {
            "year" => "get_yearly_order_stats",
            "month" => "get_monthly_order_stats",
            "week" => "get_weekly_order_stats",
            _ => throw new ArgumentException("Invalid period", nameof(period))
        };
    }

    private static List<OrderDateStats> MapToOrderStats(DataTable dataTable, string period)
    {
        return (from DataRow row in dataTable.Rows
            select new OrderDateStats
            {
                Date = GetFormattedDate(row, period),
                TotalOrders = Convert.ToInt32(row["num_orders"])
            }).ToList();
    }

    private static string? GetFormattedDate(DataRow row, string period)
    {
        return period switch
        {
            "year" => row["year"].ToString(),
            "month" => CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(
                Convert.ToInt32(row["month"])) + " " + row["year"],
            "week" => "Week " + row["week"] + " " + row["year"],
            _ => throw new ArgumentException("Invalid period", nameof(period))
        };
    }
}