namespace Bulud.Base.Exceptions;

public static class DateExtensions
{
    public static TimeSpan Subtract(this DateOnly endDate, DateOnly startDate)
    {
        return TimeSpan.FromDays(endDate.DayNumber - startDate.DayNumber);
    }
}