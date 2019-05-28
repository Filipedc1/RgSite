using Microsoft.EntityFrameworkCore;
using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Service
{
    public class ProductService : IProduct
    {
        private readonly ApplicationDbContext _database;
        private readonly IAppUser userService;

        public ProductService(ApplicationDbContext context, IAppUser userService)
        {
            _database = context;
            this.userService = userService;
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

        public async Task<Product> GetProductForCustomerByIdAsync(int id)
        {
            return await _database.Products
                                  .Include(p => p.CollectionProducts)
                                  .Include(p => p.CustomerPrices)
                                  .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> GetProductForSalonByIdAsync(int id)
        {
            return await _database.Products
                                  .Include(p => p.CollectionProducts)
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

        public async Task<ProductCollection> GetProductCollectionForCustomersByIdAsync(int id)
        {
            return await _database.ProductCollections
                                  .Include(p => p.CollectionProducts)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(p => p.CustomerPrices)
                                  .FirstOrDefaultAsync(p => p.ProductCollectionId == id);
        }

        public async Task<ProductCollection> GetProductCollectionForSalonsByIdAsync(int id)
        {
            return await _database.ProductCollections
                                  .Include(p => p.CollectionProducts)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(p => p.SalonPrices)
                                  .FirstOrDefaultAsync(p => p.ProductCollectionId == id);
        }

        public string GetProductPriceRange(Product product, string role)
        {
            string range = string.Empty;

            if (role == RoleName.Customer && product.CustomerPrices != null && product.CustomerPrices.Count() > 0)
            {
                range = $"${product.CustomerPrices.FirstOrDefault().Cost} - ${product.CustomerPrices.LastOrDefault().Cost}";
            }
            else if (role == RoleName.Salon && product.SalonPrices != null && product.SalonPrices.Count() > 0)
            {
                range = $"${product.SalonPrices.FirstOrDefault()} - ${product.SalonPrices.LastOrDefault()}";
            }
            else
            {
                range = "N/A";
            }

            return range;
        }

        public IEnumerable<IPrice> GetPrices(Product product, string role)
        {
            var prices = new List<IPrice>();

            if (role == RoleName.Customer)
            {
                prices = product.CustomerPrices.ToList<IPrice>();
            }
            else if (role == RoleName.Salon)
            {
                prices = product.SalonPrices.ToList<IPrice>();
            }

            return prices;
        }

        public decimal GetPrice(Product product, string selectedSize, string role)
        {
            var prices = new List<IPrice>();

            if (role == RoleName.Customer)
            {
                prices = product.CustomerPrices.ToList<IPrice>();
            }
            else if (role == RoleName.Salon)
            {
                prices = product.SalonPrices.ToList<IPrice>();
            }

            return prices.FirstOrDefault(p => p.Size == selectedSize).Cost;
        }
    }
}
