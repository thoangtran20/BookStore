﻿using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            //_dbContext.Products.Include(u => u.Category).Include(u => u.CoverType);
            
        }

        public void Update(Product product)
        {
            var objFromDb = _dbContext.Products.SingleOrDefault(a => a.Id == product.Id);   
            if (objFromDb != null) 
            {
                objFromDb.Title = product.Title;
                objFromDb.ISBN = product.ISBN;
                objFromDb.ListPrice = product.ListPrice;
                objFromDb.Price = product.Price;
                objFromDb.Price50 = product.Price50;
                objFromDb.Price100 = product.Price100;
                objFromDb.Description = product.Description;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.Author = product.Author;
                objFromDb.CoverTypeId = product.CoverTypeId;
                objFromDb.ProductImages = product.ProductImages;
                //if (product.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = product.ImageUrl;
                //}
            }
            //_dbContext.Products.Update(product);
        }
    }
}
