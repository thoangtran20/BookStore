using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebCRUD.Data;
using RazorPagesWebCRUD.Model;

namespace RazorPagesWebCRUD.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category category { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db; 
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("category.Name", "Name must not same display order");
            } 
            if (ModelState.IsValid)
            {
                await _db.Categories.AddAsync(category);
                await _db.SaveChangesAsync();
                return RedirectToPage("index");
            }
            return Page();
        }
    }
}
