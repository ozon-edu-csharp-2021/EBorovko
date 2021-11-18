using System.Collections.Generic;
using Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.Employee
{
    public class Employee: Entity
    {
        private readonly List<MerchandisePack> _merchandisePacks;
        
        /// <summary>
        /// Email сотрудника
        /// </summary>
        public Email Email { get; private set; }

        /// <summary>
        /// Перечень выданных паков
        /// </summary>
        public IReadOnlyList<MerchandisePack> MerchandisePacks => _merchandisePacks;

        /*public Employee()
        {
            _merchandisePacks = new List<MerchandisePack>();
        }*/
        
        public Employee(long id)
        {
            Id = id;
            _merchandisePacks = new List<MerchandisePack>();
        } 
        

        public Employee(long id, Email email)
        {
            Id = id;
            Email = email;
            _merchandisePacks = new List<MerchandisePack>();
        } 
        
        public Employee(long id, Email email, IEnumerable<MerchandisePack> merchandisePacks)
            : this(id, email)
        {
            _merchandisePacks =  new List<MerchandisePack>(merchandisePacks);
        }

        public void SetEmail(Email email)
        {
            Email = email;
        }
        

        public void Give(MerchandisePack merchandisePack)
        {
            _merchandisePacks.Add(merchandisePack);
        }
    }
}