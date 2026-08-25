using SkySchedule;
using Xunit;

namespace SkySchedule.Tests;

public sealed class ScheduleTests
{
    [Fact]
    public void ExactScheduleMatchesUtcMinute()
    {
        var schedule = ScheduleExpression.Parse("30 14 24 8 1");
        Assert.True(schedule.IsMatch(new DateTimeOffset(2026, 8, 24, 14, 30, 0, TimeSpan.Zero)));
        Assert.False(schedule.IsMatch(new DateTimeOffset(2026, 8, 24, 14, 31, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void StepScheduleFindsNextBoundary()
    {
        var schedule = ScheduleExpression.Parse("*/15 * * * *");
        var next = schedule.Next(new DateTimeOffset(2026, 8, 24, 10, 7, 59, TimeSpan.Zero));
        Assert.Equal(new DateTimeOffset(2026, 8, 24, 10, 15, 0, TimeSpan.Zero), next);
    }

    [Theory]
    [InlineData("")]
    [InlineData("* * * *")]
    [InlineData("60 * * * *")]
    [InlineData("*/0 * * * *")]
    [InlineData("* 24 * * *")]
    [InlineData("* * 0 * *")]
    [InlineData("* * * 13 *")]
    [InlineData("* * * * 7")]
    public void InvalidSchedulesAreRejected(string expression)
    {
        Assert.ThrowsAny<Exception>(() => ScheduleExpression.Parse(expression));
    }

    [Fact]
    public void SearchWindowIsBounded()
    {
        var schedule = ScheduleExpression.Parse("0 0 1 1 *");
        var after = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<InvalidOperationException>(() => schedule.Next(after, 30));
    }
}
