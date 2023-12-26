using BookStore.DataAccess.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _dbContext.Categories.ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }

        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Name must not same DisplayOrder");
            }
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(obj);
                _dbContext.SaveChanges();
                TempData["Success"] = "Category Create Successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) 
            { 
                return NotFound();
            }
            var categoryFromDB = _dbContext.Categories.Find(id);

            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Name must not same DisplayOrder");
            }
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Update(obj);
                _dbContext.SaveChanges();
                TempData["Success"] = "Category Update Successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDB = _dbContext.Categories.Find(id);

            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        // post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _dbContext.Categories.Find(id);
            
            if (obj == null) 
            { 
                return NotFound();
            }
            else
            {
                _dbContext.Categories.Remove(obj);
                _dbContext.SaveChanges();
                TempData["Success"] = "Category Delete Successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }
    }
}
