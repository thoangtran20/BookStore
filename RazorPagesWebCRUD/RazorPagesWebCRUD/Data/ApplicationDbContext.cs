using Microsoft.EntityFrameworkCore;
using RazorPagesWebCRUD.Model;

namespace RazorPagesWebCRUD.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
    }

}
