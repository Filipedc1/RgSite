using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data
{
    public interface IProduct
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsAsync();
        Task AddProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<ProductCollection> GetProductCollectionByIdAsync(int id);
        Task<List<ProductCollection>> GetAllProductCollectionsAsync();
        Task AddProductCollectionAsync(ProductCollection collection);
        Task DeleteProductCollectionAsync(int id);
        string GetProductPriceRange(Product product, string role);
        Task<List<Price>> GetPrices();
    }
}
