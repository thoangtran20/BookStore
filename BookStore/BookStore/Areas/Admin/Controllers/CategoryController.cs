using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]

    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if (id == null || id == 0)
            {
                // Create Category
                return View(category);
            }
            else
            {
                // Update Category
                category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            }
            return View(category);
        }

        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Name must not same DisplayOrder");
            }
            if (_unitOfWork.Category.IsDisplayOrderExists(obj.DisplayOrder))
            {
                ModelState.AddModelError("DisplayOrder", "DisplayOrder already exists");
            }
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitOfWork.Category.Add(obj);
                }
                else
                {
                    _unitOfWork.Category.Update(obj);
                }

                _unitOfWork.Save();
                TempData["Success"] = "Category created/updated Successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

        #region API_CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.Category.GetAll();
            return Json(new { data = categoryList });
        }

        // post
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.Category.Remove(obj);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successful" });
            }
        }
        #endregion
    }
}
