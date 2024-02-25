//using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModel;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Extensions.Hosting;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll();
            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();
            productVM.Product = new Product();
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

            if (id == null || id == 0)
            {
                // Create product
                return View(productVM);
            }
            else
            {
                // Update product
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                   foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath)) 
                            Directory.CreateDirectory(finalPath);

                        using (var fileStreams = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStreams);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);
                    }
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();

                    //if (obj.Product.ImageUrl != null)
                    //{
                    //    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    //    if (System.IO.File.Exists(oldImagePath))
                    //    {
                    //        System.IO.File.Delete(oldImagePath);
                    //    }
                    //}


                    //obj.Product.ImageUrl = @"\images\products\" + fileName + extension;

                }

                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.GetFirstOrDefault(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                        imageToBeDeleted.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully!!!";
            }
            return RedirectToAction(nameof(Upsert), new { id = productId });
        }

        #region API_CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new {data = productList});
        }

        // post
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            var productToBeDelteted = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (productToBeDelteted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                //if (  .ImageUrl != null)
                //{
                //    string wwwRootPath = _webHostEnvironment.WebRootPath;
                //    var oldImagePath = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));
                //    if (System.IO.File.Exists(oldImagePath))
                //    {
                //        System.IO.File.Delete(oldImagePath);
                //    }
                //}

                string productPath = @"images\products\product-" + id;
                string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

                if (Directory.Exists(finalPath))
                {
                    string[] filePaths = Directory.GetFiles(finalPath);
                    foreach (string filePath in filePaths) 
                    { 
                        System.IO.File.Delete(filePath);
                    }
                    Directory.Delete(finalPath);  
                }

                _unitOfWork.Product.Remove(productToBeDelteted);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Delete Successful" });
            }
        }
        #endregion
    }
}
