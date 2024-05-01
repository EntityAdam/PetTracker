using System.Diagnostics.CodeAnalysis;

namespace Core.Interface;

[ExcludeFromCodeCoverage]
public class DateTimeOffsetProviderMock : IDateTimeOffsetProvider
{
    internal DateTimeOffset CurrentDateTimeOffset { get; set; }

    public DateTimeOffset GetCurrentDateTimeOffset() => CurrentDateTimeOffset;
}
