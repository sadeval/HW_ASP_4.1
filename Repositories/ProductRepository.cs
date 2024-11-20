using OnlineStoreAPI.Models;
using System.Collections.Concurrent;

namespace OnlineStoreAPI.Repositories
{
    public class ProductRepository
    {
        private readonly ConcurrentDictionary<int, Product> _products = new();
        private int _nextId = 1;

        public IEnumerable<Product> GetAll() => _products.Values;

        public Product GetById(int id) => _products.GetValueOrDefault(id);

        public Product Add(Product product)
        {
            product.Id = _nextId++;
            _products[product.Id] = product;
            return product;
        }

        public bool Delete(int id)
        {
            return _products.TryRemove(id, out _);
        }
    }
}
