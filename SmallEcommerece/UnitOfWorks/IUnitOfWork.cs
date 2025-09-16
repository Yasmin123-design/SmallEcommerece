using SmallEcommerece.Models;
using SmallEcommerece.Repository;

namespace SmallEcommerece.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Product> Products { get; }
        Task<int> CommitAsync();
    }

}
