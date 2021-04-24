using RgSite.Core.Models;
using RgSite.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RgSite.Core.Interfaces
{
    public interface IProductService
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

        //Comment methods
        Task<bool> AddNewCommentAsync(int productId, Comment comment, AppUser commentAuthor);
        Task<bool> DeleteCommentAsync(Comment comment);
        Task<IEnumerable<Comment>> GetAllCommentsByProductIdAsync(int productId);
        Task<Comment> GetCommentByIdAsync(int id);
    }
}
