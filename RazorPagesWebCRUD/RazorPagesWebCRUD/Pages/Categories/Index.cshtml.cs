using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebCRUD.Data;
using RazorPagesWebCRUD.Model;

namespace RazorPagesWebCRUD.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IEnumerable<Category> categories { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;   
        }
        public void OnGet()
        {
            categories = _db.Categories;
        }
    }
}
