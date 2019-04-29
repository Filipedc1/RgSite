using Microsoft.EntityFrameworkCore;
using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Service
{
    public class ProductService : IProduct, IProductCollection
    {
        private readonly ApplicationDbContext _database;

        public ProductService(ApplicationDbContext context)
        {
            _database = context;
        }

        public async Task AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _database.Products
                                  .Include(p => p.CollectionProducts)
                                  .Include(p => p.CustomerPrices)
                                  .Include(p => p.SalonPrices)
                                  .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _database.Products
                                  .Include(p => p.CollectionProducts)
                                  .Include(p => p.CustomerPrices)
                                  .Include(p => p.SalonPrices)
                                  .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddProductCollectionAsync(ProductCollection collection)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteProductCollectionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductCollection>> GetAllProductCollectionsForCustomersAsync()
        {
            return await _database.ProductCollections
                                  .Include(p => p.CollectionProducts)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(p => p.CustomerPrices)
                                  .ToListAsync();
        }

        public async Task<List<ProductCollection>> GetAllProductCollectionsForSalonsAsync()
        {
            return await _database.ProductCollections
                                  .Include(p => p.CollectionProducts)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(p => p.SalonPrices)
                                  .ToListAsync();
        }

        public async Task<ProductCollection> GetProductCollectionByIdAsync(int id)
        {
            return await _database.ProductCollections
                                  .Include(p => p.CollectionProducts)
                                    .ThenInclude(p => p.Product)
                                  .FirstOrDefaultAsync(p => p.ProductCollectionId == id);
        }
    }
}
