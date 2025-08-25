

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yami33.Models;
using Yami33.Utility;

namespace Yami33.Services
{
    public class MockProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public MockProductRepository()
        {
            // چند داده نمونه
            _products.Add(new Product { Id = 1, Name = "Laptop", Price = 950, Description = "High performance laptop", ImageUrl = "/images/product/06a7abb7-1327-41bc-b379-8834c73b9160..png" });
            _products.Add(new Product { Id = 2, Name = "Phone", Price = 499, Description = "Smartphone with good camera", ImageUrl = "/images/product/22aa1c1c-d0d0-4bdf-814a-b56a2aeb2aee..png" });
        }

        public Task<Product> CreateAsync(Product obj)
        {
            obj.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(obj);
            return Task.FromResult(obj);
        }

        public Task<Product> UpdateAsync(Product obj)
        {
            var existing = _products.FirstOrDefault(p => p.Id == obj.Id);
            if (existing != null)
            {
                existing.Name = obj.Name;
                existing.Price = obj.Price;
                existing.Description = obj.Description;
                existing.ImageUrl = obj.ImageUrl;
            }
            return Task.FromResult(existing);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<Product> GetAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.AsEnumerable());
        }
    }
}
