namespace QuickstartTemplate.ApplicationCore.Contracts;

public interface IDateTimeProvider 
{
    /// <summary>
    /// Returns the current time (UTC).
    /// </summary>
    /// <returns></returns>
    DateTimeOffset GetNow();
}