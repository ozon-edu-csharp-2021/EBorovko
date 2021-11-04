using System;

namespace Ozon.MerchandiseService.Infrastructure.Services.Interfaces
{
    public interface IDateTimeService
    {
        DateTimeOffset Now { get; }
    }
}