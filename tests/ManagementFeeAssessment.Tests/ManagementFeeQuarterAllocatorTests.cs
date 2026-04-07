using ManagementFeeAssessment.Models;
using ManagementFeeAssessment.Services;

namespace ManagementFeeAssessment.Tests;

public class ManagementFeeQuarterAllocatorTests
{
    private readonly ManagementFeeQuarterAllocator _allocator = new();

    private static Quarter Q1_2024 => new()
    {
        Name = "Q1 2024",
        StartDate = new DateTime(2024, 1, 1),
        EndDate = new DateTime(2024, 3, 31)
    };

    private static Quarter Q2_2024 => new()
    {
        Name = "Q2 2024",
        StartDate = new DateTime(2024, 4, 1),
        EndDate = new DateTime(2024, 6, 30)
    };

    private static Quarter Q3_2024 => new()
    {
        Name = "Q3 2024",
        StartDate = new DateTime(2024, 7, 1),
        EndDate = new DateTime(2024, 9, 30)
    };

    [Fact]
    public void Line_ExactlyMatchesQuarter_BrokenQuarterIsFalse()
    {
        var line = new ManagementFeeLine
        {
            Id = "L1",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 3, 31)
        };

        var result = _allocator.AssignLinesToQuarters([line], [Q1_2024]).ToList();

        Assert.Single(result);
        Assert.Equal("L1", result[0].LineId);
        Assert.Equal("Q1 2024", result[0].QuarterName);
        Assert.False(result[0].BrokenQuarter);
    }

    [Fact]
    public void Line_StartsAfterQuarterStart_EndsAtQuarterEnd_BrokenQuarterIsTrue()
    {
        var line = new ManagementFeeLine
        {
            Id = "L2",
            StartDate = new DateTime(2024, 2, 1),
            EndDate = new DateTime(2024, 3, 31)
        };

        var result = _allocator.AssignLinesToQuarters([line], [Q1_2024]).ToList();

        Assert.Single(result);
        Assert.Equal("L2", result[0].LineId);
        Assert.Equal("Q1 2024", result[0].QuarterName);
        Assert.True(result[0].BrokenQuarter);
    }

    [Fact]
    public void Line_StartsAtQuarterStart_EndsBeforeQuarterEnd_BrokenQuarterIsTrue()
    {
        var line = new ManagementFeeLine
        {
            Id = "L3",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 2, 29)
        };

        var result = _allocator.AssignLinesToQuarters([line], [Q1_2024]).ToList();

        Assert.Single(result);
        Assert.Equal("L3", result[0].LineId);
        Assert.Equal("Q1 2024", result[0].QuarterName);
        Assert.True(result[0].BrokenQuarter);
    }

    [Fact]
    public void Line_SpansMultipleQuarters_FullMiddleQuarterIsNotBroken()
    {
        // Line spans all of Q1, all of Q2, and all of Q3
        // Q1 -> broken (line starts before Q1 boundary? No — line starts on Q1 start)
        // Actually: line starts Jan 1 and ends Sep 30 — covers all three quarters fully
        // Let's use a line that starts mid-Q1 and ends mid-Q3 so Q2 is fully covered
        var line = new ManagementFeeLine
        {
            Id = "L4",
            StartDate = new DateTime(2024, 2, 1),
            EndDate = new DateTime(2024, 8, 15)
        };

        var quarters = new[] { Q1_2024, Q2_2024, Q3_2024 };
        var result = _allocator.AssignLinesToQuarters([line], quarters)
                               .OrderBy(a => a.QuarterName)
                               .ToList();

        Assert.Equal(3, result.Count);

        var q1 = result.Single(a => a.QuarterName == "Q1 2024");
        var q2 = result.Single(a => a.QuarterName == "Q2 2024");
        var q3 = result.Single(a => a.QuarterName == "Q3 2024");

        Assert.True(q1.BrokenQuarter);   // starts Feb 1, after Q1 start
        Assert.False(q2.BrokenQuarter);  // covers all of Q2 (Apr 1 – Jun 30)
        Assert.True(q3.BrokenQuarter);   // ends Aug 15, before Q3 end
    }

    [Fact]
    public void Line_OutsideAllQuarters_ReturnsNoAssignments()
    {
        var line = new ManagementFeeLine
        {
            Id = "L5",
            StartDate = new DateTime(2023, 1, 1),
            EndDate = new DateTime(2023, 3, 31)
        };

        var result = _allocator.AssignLinesToQuarters([line], [Q1_2024]).ToList();

        Assert.Empty(result);
    }
}
