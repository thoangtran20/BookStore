using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsDisplayOrderExists(int displayOrder)
        {
            return _dbContext.Categories.Any(c => c.DisplayOrder == displayOrder);
        }

        public void Update(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
