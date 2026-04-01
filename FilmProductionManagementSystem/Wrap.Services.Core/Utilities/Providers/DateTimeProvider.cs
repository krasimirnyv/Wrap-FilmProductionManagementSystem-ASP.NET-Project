namespace Wrap.Services.Core.Utilities.Providers;

using Interfaces;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}