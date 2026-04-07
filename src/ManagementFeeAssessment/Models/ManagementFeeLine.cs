namespace ManagementFeeAssessment.Models;

public class ManagementFeeLine
{
    public string Id { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
}
