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

    // ---------------------------------------------------------------------------
    // Part 2: ProrateFeeAcrossQuarters
    // ---------------------------------------------------------------------------

    [Fact]
    public void Proration_LineExactlyMatchesQuarter_FullAmountAllocatedToThatQuarter()
    {
        // Line: Jan 1 – Mar 31 (91 days), fully inside Q1 (91 days)
        // Expected: Q1 gets 100% of the amount
        var line = new ManagementFeeLine
        {
            Id = "L1",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 3, 31),
            Amount = 9100m
        };

        var result = _allocator.ProrateFeeAcrossQuarters([line], [Q1_2024]).ToList();

        Assert.Single(result);
        Assert.Equal("Q1 2024", result[0].QuarterName);
        Assert.Equal(9100m, result[0].ProratedAmount);
    }

    [Fact]
    public void Proration_LineSpansTwoQuarters_AmountSplitProportionally()
    {
        // Line: Jan 1 – Jun 30
        // Q1: Jan 1 – Mar 31 = 91 days
        // Q2: Apr 1 – Jun 30 = 91 days
        // Total line days = 182
        // Q1 share = 91/182 = 50%, Q2 share = 91/182 = 50%
        var line = new ManagementFeeLine
        {
            Id = "L2",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            Amount = 1000m
        };

        var result = _allocator.ProrateFeeAcrossQuarters([line], [Q1_2024, Q2_2024])
                               .OrderBy(a => a.QuarterName)
                               .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(500m, result.Single(a => a.QuarterName == "Q1 2024").ProratedAmount);
        Assert.Equal(500m, result.Single(a => a.QuarterName == "Q2 2024").ProratedAmount);
    }

    [Fact]
    public void Proration_LineSpansThreeQuarters_UnevenSplit()
    {
        // Line: Feb 1 – Jun 30
        // Q1 overlap: Feb 1 – Mar 31 = 60 days
        // Q2 overlap: Apr 1 – Jun 30 = 91 days
        // Total line days = 151
        // Q1 share = 60/151, Q2 share = 91/151
        var line = new ManagementFeeLine
        {
            Id = "L3",
            StartDate = new DateTime(2024, 2, 1),
            EndDate = new DateTime(2024, 6, 30),
            Amount = 1510m
        };

        int totalDays = (line.EndDate - line.StartDate).Days + 1; // 151

        var result = _allocator.ProrateFeeAcrossQuarters([line], [Q1_2024, Q2_2024])
                               .OrderBy(a => a.QuarterName)
                               .ToList();

        Assert.Equal(2, result.Count);

        decimal expectedQ1 = 1510m * (60m / totalDays);
        decimal expectedQ2 = 1510m * (91m / totalDays);

        Assert.Equal(expectedQ1, result.Single(a => a.QuarterName == "Q1 2024").ProratedAmount);
        Assert.Equal(expectedQ2, result.Single(a => a.QuarterName == "Q2 2024").ProratedAmount);
    }

    [Fact]
    public void Proration_ProratedAmountsAcrossAllQuartersSumToTotalAmount()
    {
        // Regardless of how the split works, the sum should always equal the original amount
        var line = new ManagementFeeLine
        {
            Id = "L4",
            StartDate = new DateTime(2024, 2, 15),
            EndDate = new DateTime(2024, 8, 20),
            Amount = 7777m
        };

        var quarters = new[] { Q1_2024, Q2_2024, Q3_2024 };
        var result = _allocator.ProrateFeeAcrossQuarters([line], quarters).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(7777m, result.Sum(a => a.ProratedAmount));
    }
}
