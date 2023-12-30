using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll();
            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();
            productVM.product = new Product();
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(
                    u => new SelectListItem()
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }
                );

            productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                    u => new SelectListItem()
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }
                );

            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(
            //        u => new SelectListItem()
            //        {
            //            Text = u.Name,
            //            Value = u.Id.ToString()
            //        }
            //    );

            //IEnumerable<SelectListItem> CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
            //       u => new SelectListItem()
            //       {
            //           Text = u.Name,
            //           Value = u.Id.ToString()
            //       }
            //   );

            if (id == null || id == 0)
            {
                // Create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            }
            else
            {
                // Update product
                productVM.product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            }
            //var productFromDB = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            //if (productFromDB == null)
            //{
            //    return NotFound();
            //}
            return View(productVM);
        }

        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj.product);
                _unitOfWork.Save();
                TempData["Success"] = "Product Update Successfully";
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
            var productFromDB = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (productFromDB == null)
            {
                return NotFound();
            }
            return View(productFromDB);
        }

        // post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.Product.Remove(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Product Delete Successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }
    }
}
