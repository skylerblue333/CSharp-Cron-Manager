using System.Globalization;

namespace SkySchedule;

public sealed class ScheduleExpression
{
    private readonly Field _minute;
    private readonly Field _hour;
    private readonly Field _day;
    private readonly Field _month;
    private readonly Field _dayOfWeek;

    private ScheduleExpression(Field minute, Field hour, Field day, Field month, Field dayOfWeek)
    {
        _minute = minute;
        _hour = hour;
        _day = day;
        _month = month;
        _dayOfWeek = dayOfWeek;
    }

    public static ScheduleExpression Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("schedule expression is required", nameof(expression));

        var fields = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (fields.Length != 5)
            throw new FormatException("schedule must contain exactly five fields: minute hour day month day-of-week");

        return new ScheduleExpression(
            Field.Parse(fields[0], 0, 59, "minute"),
            Field.Parse(fields[1], 0, 23, "hour"),
            Field.Parse(fields[2], 1, 31, "day"),
            Field.Parse(fields[3], 1, 12, "month"),
            Field.Parse(fields[4], 0, 6, "day-of-week")
        );
    }

    public bool IsMatch(DateTimeOffset instant)
    {
        var utc = instant.ToUniversalTime();
        return _minute.Matches(utc.Minute)
            && _hour.Matches(utc.Hour)
            && _day.Matches(utc.Day)
            && _month.Matches(utc.Month)
            && _dayOfWeek.Matches((int)utc.DayOfWeek);
    }

    public DateTimeOffset Next(DateTimeOffset after, int searchDays = 366)
    {
        if (searchDays is < 1 or > 3660)
            throw new ArgumentOutOfRangeException(nameof(searchDays), "searchDays must be between 1 and 3660");

        var cursor = after.ToUniversalTime();
        cursor = new DateTimeOffset(cursor.Year, cursor.Month, cursor.Day, cursor.Hour, cursor.Minute, 0, TimeSpan.Zero).AddMinutes(1);
        var limit = cursor.AddDays(searchDays);

        while (cursor <= limit)
        {
            if (IsMatch(cursor)) return cursor;
            cursor = cursor.AddMinutes(1);
        }

        throw new InvalidOperationException($"no matching schedule found within {searchDays} days");
    }

    private sealed class Field
    {
        private readonly int? _exact;
        private readonly int? _step;

        private Field(int? exact, int? step)
        {
            _exact = exact;
            _step = step;
        }

        public static Field Parse(string token, int min, int max, string name)
        {
            if (token == "*") return new Field(null, null);

            if (token.StartsWith("*/", StringComparison.Ordinal))
            {
                if (!int.TryParse(token[2..], NumberStyles.None, CultureInfo.InvariantCulture, out var step)
                    || step < 1 || step > max - min + 1)
                    throw new FormatException($"{name} step is invalid");
                return new Field(null, step);
            }

            if (!int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out var exact)
                || exact < min || exact > max)
                throw new FormatException($"{name} must be '*', '*/n', or an integer between {min} and {max}");

            return new Field(exact, null);
        }

        public bool Matches(int value)
        {
            if (_exact is int exact) return value == exact;
            if (_step is int step) return value % step == 0;
            return true;
        }
    }
}
