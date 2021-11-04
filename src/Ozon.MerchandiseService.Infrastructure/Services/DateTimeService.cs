using System;
using Ozon.MerchandiseService.Infrastructure.Services.Interfaces;

namespace Ozon.MerchandiseService.Infrastructure.Services
{
    public class DateTimeService: IDateTimeService
    {
        public DateTimeOffset Now => DateTimeOffset.Now; 
    }
}