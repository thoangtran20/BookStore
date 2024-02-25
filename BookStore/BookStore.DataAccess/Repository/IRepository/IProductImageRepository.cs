using BookStore.Models;

namespace BookStore.DataAccess.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        void Update(ProductImage productImage);
    }
}
