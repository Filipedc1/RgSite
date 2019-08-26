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
                                  .Include(p => p.Prices)
                                  .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _database.Products
                                  .Include(p => p.CollectionProducts)
                                  .Include(p => p.Prices)
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

        public async Task<List<ProductCollection>> GetAllProductCollectionsAsync()
        {
            return await _database.ProductCollections
                                  .Include(p => p.CollectionProducts)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(p => p.Prices)
                                  .ToListAsync();
        }

        public async Task<ProductCollection> GetProductCollectionByIdAsync(int id)
        {
            return await _database.ProductCollections
                                  .Include(p => p.CollectionProducts)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(p => p.Prices)
                                  .FirstOrDefaultAsync(p => p.ProductCollectionId == id);
        }


        public string GetProductPriceRange(Product product, string role)
        {
            string range = string.Empty;

            if ((role == RoleName.Customer || role == RoleName.Admin) && product.Prices != null && product.Prices.Count() > 0)
            {
                range = $"${product.Prices.FirstOrDefault().CustomerCost} - ${product.Prices.LastOrDefault().CustomerCost}";
            }
            else if (role == RoleName.Salon && product.Prices != null && product.Prices.Count() > 0)
            {
                range = $"${product.Prices.FirstOrDefault().SalonCost} - ${product.Prices.LastOrDefault().SalonCost}";
            }
            else
            {
                range = "N/A";
            }

            return range;
        }

        public IEnumerable<Price> GetPrices(Product product, string role)
        {
            var prices = new List<Price>();

            if (role == RoleName.Customer || role == RoleName.Admin)
            {
                var results = product.Prices.Select(p => new Price
                {
                    Id = p.Id,
                    Size = p.Size,
                    CustomerCost = p.CustomerCost,
                    isCustomer = true
                }).ToList();

                prices.AddRange(results);
            }
            else if (role == RoleName.Salon)
            {
                var results = product.Prices.Select(p => new Price
                {
                    Id = p.Id,
                    Size = p.Size,
                    SalonCost = p.SalonCost,
                    isCustomer = false
                }).ToList();

                prices.AddRange(results);
            }

            return prices;
        }

        public async Task<List<Price>> GetPrices()
        {
            return await _database.Prices.ToListAsync();
        }
    }
}
