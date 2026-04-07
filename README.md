# Management Fee Quarterization — Live Coding Assessment

**Time:** ~30 minutes  
**Stack:** .NET 8, C#, xUnit

---

## The Problem

You are given a list of management fee lines, each with a start date and end date, and a list of quarters, each with a start date and end date.

Implement a method that maps each management fee line to every quarter it overlaps.

For each line-quarter assignment:
- set `BrokenQuarter` to `false` if the line covers the whole quarter
- set `BrokenQuarter` to `true` if the line overlaps the quarter but does not cover the entire quarter

---

## Where to Work

Open `src/ManagementFeeAssessment/Services/ManagementFeeQuarterAllocator.cs` and implement the `AssignLinesToQuarters` method.

The method signature is already in place — just replace the `throw new NotImplementedException();`.

You are welcome to add helper methods or additional classes anywhere in the `ManagementFeeAssessment` project if that helps you reason through the problem.

---

## Assumptions

- Dates are **inclusive** on both ends (a line from Jan 1 – Mar 31 fully covers Q1 Jan 1 – Mar 31)
- Quarter boundaries are already provided — you do not need to derive them
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

They will **fail** until the method is implemented — that is expected and intentional.

---

## How You Will Be Evaluated

- Does your solution produce correct results for the provided tests?
- Is your reasoning clear and easy to follow?
- Is the code readable?

**You do not need to finish everything to do well.** A partially complete solution with a clear explanation of your thinking is valued over rushed, silent code.
