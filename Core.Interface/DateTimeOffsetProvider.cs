namespace Core.Interface;

public class DateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset GetCurrentDateTimeOffset() => DateTimeOffset.UtcNow;
}
