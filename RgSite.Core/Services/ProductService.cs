using Microsoft.EntityFrameworkCore;
using RgSite.Core.Interfaces;
using RgSite.Core.Models;
using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _database;
        private readonly IUserService _userService;

        public ProductService(ApplicationDbContext context, IUserService userService)
        {
            _database = context;
            this._userService = userService;
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
            string range = "N/A";
            if (!product.Prices.Any()) return range;

            if (role == RoleConstants.Customer || role == RoleConstants.Admin)
            {
                range = $"${product.Prices.FirstOrDefault().CustomerCost} - ${product.Prices.LastOrDefault().CustomerCost}";
            }
            else if (role == RoleConstants.Salon)
            {
                range = $"${product.Prices.FirstOrDefault().SalonCost} - ${product.Prices.LastOrDefault().SalonCost}";
            }

            return range;
        }

        public async Task<List<Price>> GetPrices()
        {
            return await _database.Prices.ToListAsync();
        }



        #region Comment Methods

        public async Task<bool> AddNewCommentAsync(int productId, Comment comment, AppUser commentAuthor)
        {
            if (comment == null) return false;

            var productInDB = await GetProductByIdAsync(productId);
            if (productInDB != null)
            {
                comment.Product = productInDB;
                comment.TimeSent = DateTime.Now;
                comment.User = commentAuthor;

                await _database.Comment.AddAsync(comment);
                await _database.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteCommentAsync(Comment comment)
        {
            if (comment == null) return false;

            _database.Comment.Remove(comment);
            await _database.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsByProductIdAsync(int productId)
        {
            return await _database.Comment
                                  .Include(p => p.Product)
                                  .Include(u => u.User)
                                  .Where(c => c.Product.ProductId == productId)
                                  .ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _database.Comment
                                  .Include(c => c.Product)
                                  .FirstOrDefaultAsync(c => c.Id == id);
        }

        #endregion
    }
}
