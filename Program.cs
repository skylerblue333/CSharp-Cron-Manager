using System.Globalization;
using System.Text.Json;
using SkySchedule;

if (args.Length is < 1 or > 2)
{
    Console.Error.WriteLine("usage: sky-schedule '<minute hour day month day-of-week>' [after-iso8601]");
    return 2;
}

try
{
    var schedule = ScheduleExpression.Parse(args[0]);
    var after = args.Length == 2
        ? DateTimeOffset.Parse(args[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
        : DateTimeOffset.UtcNow;
    var next = schedule.Next(after);
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        expression = args[0],
        after = after.ToUniversalTime(),
        nextRun = next,
        timezone = "UTC"
    }));
    return 0;
}
catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
{
    Console.Error.WriteLine(JsonSerializer.Serialize(new { error = ex.Message }));
    return 1;
}
