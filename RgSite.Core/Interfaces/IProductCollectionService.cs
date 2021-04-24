using RgSite.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RgSite.Core.Interfaces
{
    public interface IProductCollectionService
    {
        Task<ProductCollection> GetProductCollectionByIdAsync(int id);
        Task<List<ProductCollection>> GetAllProductCollectionsForCustomersAsync();
        Task<List<ProductCollection>> GetAllProductCollectionsForSalonsAsync();
        Task AddProductCollectionAsync(ProductCollection collection);
        Task DeleteProductCollectionAsync(int id);
    }
}
