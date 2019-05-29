using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data
{
    public interface IProduct
    {
        Task<Product> GetProductForCustomerByIdAsync(int id);
        Task<Product> GetProductForSalonByIdAsync(int id);
        Task<List<Product>> GetAllProductsAsync();
        Task AddProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<ProductCollection> GetProductCollectionForCustomersByIdAsync(int id);
        Task<ProductCollection> GetProductCollectionForSalonsByIdAsync(int id);
        Task<List<ProductCollection>> GetAllProductCollectionsForCustomersAsync();
        Task<List<ProductCollection>> GetAllProductCollectionsForSalonsAsync();
        Task AddProductCollectionAsync(ProductCollection collection);
        Task DeleteProductCollectionAsync(int id);
        string GetProductPriceRange(Product product, string role);
        IEnumerable<Price> GetPrices(Product product, string role);
    }
}
