using SmallEcommerece.Models;
using SmallEcommerece.Repository;

namespace SmallEcommerece.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmallEcommereceContext _context;
        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }

        public UnitOfWork(SmallEcommereceContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(context);
            Products = new GenericRepository<Product>(context);
        }

        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
