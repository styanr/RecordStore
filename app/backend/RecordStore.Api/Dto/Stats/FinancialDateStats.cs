namespace RecordStore.Api.Dto.Stats;

public class FinancialDateStats
{
    public string? Date { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
}