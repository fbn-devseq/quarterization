# Management Fee Quarterization - Live Coding Assessment

**Time:** ~30 minutes  
**Stack:** .NET 8, C#, xUnit

---

## The Problem

### Part 1 - Assign lines to quarters

You are given a list of management fee lines, each with a start date and end date, and a list of quarters, each with a start date and end date.

Implement a method that maps each management fee line to every quarter it overlaps.

For each line-quarter assignment:
- set `BrokenQuarter` to `false` if the line covers the whole quarter
- set `BrokenQuarter` to `true` if the line overlaps the quarter but does not cover the entire quarter

### Part 2 - Prorate the fee amount across quarters

Each management fee line also has an `Amount`. Implement a second method that distributes that amount across every quarter the line overlaps, proportional to the number of days the line falls within each quarter relative to the total number of days in the line.

For example: a line running from 1 Jan to 30 Jun with an amount of €1000 overlaps Q1 (91 days) and Q2 (91 days). Total line length is 182 days, so each quarter receives €500.

---

## Where to Work

Open `src/ManagementFeeAssessment/Services/ManagementFeeQuarterAllocator.cs` and implement both methods.

The method signatures are already in place - just replace the `throw new NotImplementedException();` in each one.

You are welcome to add helper methods or additional classes anywhere in the `ManagementFeeAssessment` project if that helps you reason through the problem.

---

## Assumptions

- Dates are **inclusive** on both ends (a line from Jan 1 – Mar 31 fully covers Q1 Jan 1 – Mar 31)
- Quarter boundaries are already provided - you do not need to derive them
- Quarters do not overlap each other
- A management fee line may span multiple quarters
- Ignore input validation and data persistence unless explicitly told otherwise
- Focus on clear reasoning and correct core logic, not production-readiness

---

## Running the Tests

```bash
dotnet restore
dotnet test
```

Tests are in `tests/ManagementFeeAssessment.Tests/ManagementFeeQuarterAllocatorTests.cs`.

They will **fail** until the method is implemented - that is expected and intentional.

---

## How You Will Be Evaluated

- Does your solution produce correct results for the provided tests?
- Is your reasoning clear and easy to follow?
- Is the code readable?

**You do not need to finish everything to do well.** A partially complete solution with a clear explanation of your thinking is valued over rushed, silent code.
