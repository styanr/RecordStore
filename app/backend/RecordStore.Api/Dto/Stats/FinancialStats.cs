namespace RecordStore.Api.Dto.Stats;

public class FinancialStats
{
    public int TotalOrders { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
}