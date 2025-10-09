namespace Bulud.Base.Extensions;

public class DateExtensions
{
    public static TimeSpan Subtract(DateOnly endDate, DateOnly startDate)
    {
        return TimeSpan.FromDays(endDate.DayNumber - startDate.DayNumber);
    }
}