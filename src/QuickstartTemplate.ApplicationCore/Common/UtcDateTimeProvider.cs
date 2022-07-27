using QuickstartTemplate.ApplicationCore.Contracts;

namespace QuickstartTemplate.ApplicationCore.Common;

public class UtcDateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Returns the current time (UTC).
    /// </summary>
    /// <returns></returns>
    public DateTimeOffset GetNow() => DateTimeOffset.UtcNow;
}