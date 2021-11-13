using System.Collections.Generic;
using Ozon.MerchandiseService.Domain.Events;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.Employee
{
    public class Employee: Entity
    {
        private readonly List<MerchandisePack> _merchandisePacks;
        
        /// <summary>
        /// Email сотрудника
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Перечень выданных паков
        /// </summary>
        public IReadOnlyList<MerchandisePack> MerchandisePacks => _merchandisePacks; 

        public Employee(long id, string email)
        {
            Id = id;
            Email = email;
            _merchandisePacks = new List<MerchandisePack>();
        }
        
        public Employee(long id, string email, IEnumerable<MerchandisePack> merchandisePacks)
            : this(id, email)
        {
            _merchandisePacks =  new List<MerchandisePack>(merchandisePacks);
        }

        public void Give(MerchandisePack merchandisePack)
        {
            _merchandisePacks.Add(merchandisePack);
            
            this.AddDomainEvent(new GivingNewMerchandisePackDomainEvent(this));
        }
    }
}