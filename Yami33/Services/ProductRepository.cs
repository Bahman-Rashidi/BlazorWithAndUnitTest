using Yami33.Models;
using Yami33.Utility;

namespace Yami33.Services
{
    public class ProductRepository : IProductRepository
    {
        public Task<Product> CreateAsync(Product obj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateAsync(Product obj)
        {
            throw new NotImplementedException();
        }
    }
}
