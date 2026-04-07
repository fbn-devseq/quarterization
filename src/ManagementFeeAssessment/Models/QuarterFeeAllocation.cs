namespace ManagementFeeAssessment.Models;

public class QuarterFeeAllocation
{
    public string LineId { get; set; } = string.Empty;
    public string QuarterName { get; set; } = string.Empty;
    public decimal ProratedAmount { get; set; }
}
