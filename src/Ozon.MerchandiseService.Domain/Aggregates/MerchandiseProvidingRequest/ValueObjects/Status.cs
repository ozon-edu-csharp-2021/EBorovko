using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects
{
    public class Status: Enumeration
    {
        public static readonly Status Draft = new(1, nameof(Draft));
        public static readonly Status Created = new(2, nameof(Created));
        public static readonly Status InWork = new(4, nameof(InWork));
        public static readonly Status Done = new(5, nameof(Done));
        
        private Status(int id, string name) : base(id, name)
        {
        }

        public static Status Parse(int id)
        {
            return id switch
            {
                1 => Draft,
                2 => Created,
                4 => InWork,
                5 => Done,
                _ => Draft
            };
        }
        
            
    }
}