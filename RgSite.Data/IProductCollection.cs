using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Data
{
    public interface IProductCollection
    {
        Task<ProductCollection> GetProductCollectionByIdAsync(int id);
        Task<List<ProductCollection>> GetAllProductCollectionsForCustomersAsync();
        Task<List<ProductCollection>> GetAllProductCollectionsForSalonsAsync();
        Task AddProductCollectionAsync(ProductCollection collection);
        Task DeleteProductCollectionAsync(int id);
    }
}
