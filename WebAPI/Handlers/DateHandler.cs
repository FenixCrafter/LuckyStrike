namespace WebApi.Handlers;

public class DateHandler
{
    public string GetUniqueDayKey(DateTime date)
    {
        return date.Year.ToString() + date.Month.ToString() + date.Day.ToString();
    }
}
