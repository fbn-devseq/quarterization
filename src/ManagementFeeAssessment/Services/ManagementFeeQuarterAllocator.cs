using ManagementFeeAssessment.Models;

namespace ManagementFeeAssessment.Services;

public class ManagementFeeQuarterAllocator
{
    /// <summary>
    /// Maps each management fee line to every quarter it overlaps.
    /// A line covers a quarter fully if its start date is on or before the quarter's start date
    /// and its end date is on or after the quarter's end date.
    /// Otherwise the assignment is marked as a broken quarter.
    /// </summary>
    public IEnumerable<QuarterAssignment> AssignLinesToQuarters(
        IEnumerable<ManagementFeeLine> lines,
        IEnumerable<Quarter> quarters)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// For each management fee line, distributes its Amount across every quarter it overlaps,
    /// proportional to the number of days the line falls within each quarter relative to
    /// the total number of days in the line.
    /// </summary>
    public IEnumerable<QuarterFeeAllocation> ProrateFeeAcrossQuarters(
        IEnumerable<ManagementFeeLine> lines,
        IEnumerable<Quarter> quarters)
    {
        throw new NotImplementedException();
    }
}
