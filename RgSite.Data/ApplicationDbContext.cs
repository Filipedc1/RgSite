﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RgSite.Data.Models;

namespace RgSite.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<AppUser> AppUsers                      { get; set; }

        public DbSet<Salon> Salons                          { get; set; }
        public DbSet<Product> Products                      { get; set; }
        public DbSet<ProductCollection> ProductCollections  { get; set; }
        public DbSet<Price> Prices                          { get; set; }
        public DbSet<CartItem> ShoppingCartItems            { get; set; }
        public DbSet<State> States                          { get; set; }
        public DbSet<Order> Orders                          { get; set; }
        public DbSet<OrderDetail> OrderDetails              { get; set; }
        public DbSet<BillingDetail> BillingDetails          { get; set; }
        public DbSet<Comment> Comment                       { get; set; }


        // Junction table
        public DbSet<CollectionProduct> CollectionProducts  { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CollectionProduct>()
                   .HasKey(k => new { k.ProductCollectionId, k.ProductId });
        }
    }
}
