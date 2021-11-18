using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Contracts
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
    }
}