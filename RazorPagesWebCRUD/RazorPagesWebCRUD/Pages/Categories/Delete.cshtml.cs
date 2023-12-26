using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebCRUD.Data;
using RazorPagesWebCRUD.Model;

namespace RazorPagesWebCRUD.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category category { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db; 
        }
        public void OnGet(int id)
        {
            category = _db.Categories.Find(id);
            //category = _db.Categories.SingleOrDefault(a => a.Id == id);
        }

        public async Task<IActionResult> OnPost(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("category.Name", "Name must not same display order");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                TempData["success"] = "Delete category successfully";
                return RedirectToPage("index");
            }
            return Page();
        }
    }
}
