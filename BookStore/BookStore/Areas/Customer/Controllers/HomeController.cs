using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string searchString, int? categoryId, string sortOrder)
        {
            IEnumerable<Product> ProductList;

            if (!string.IsNullOrEmpty(searchString) || categoryId.HasValue)
            {
                ProductList = _unitOfWork.Product.GetAll(
                    filter: p => p.Title.Contains(searchString) || p.Author.Contains(searchString)
                        || (!categoryId.HasValue || p.CategoryId == categoryId),
                    includeProperties: "Category,CoverType,ProductImages"
                );
            }
            else
            {
                ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType,ProductImages");
            }

            // Sorting logic
            switch (sortOrder)
            {
                case "price_asc":
                    ProductList = ProductList.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    ProductList = ProductList.OrderByDescending(p => p.Price);
                    break;
                case "title_asc":
                    ProductList = ProductList.OrderBy(p => p.Title);
                    break;
                case "title_desc":
                    ProductList = ProductList.OrderByDescending(p => p.Title);
                    break;
                case "author_asc":
                    ProductList = ProductList.OrderBy(p => p.Author);
                    break;
                case "author_desc":
                    ProductList = ProductList.OrderByDescending(p => p.Author);
                    break;
                case "category_asc":
                    ProductList = ProductList.OrderBy(p => p.Category);
                    break;
                case "category_desc":
                    ProductList = ProductList.OrderByDescending(p => p.Category);
                    break;
                default:
                    // Default sorting
                    ProductList = ProductList.OrderBy(p => p.Id);
                    break;
            }

            // Fetch all categories
            var categories = _unitOfWork.Category.GetAll();

            // Pass categories and selected category to the view
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.SelectedCategory = categoryId?.ToString();

            // Set ViewBag for sorting links
            ViewBag.PriceSortParam = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewBag.TitleSortParam = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewBag.PriceSortParam = sortOrder == "author_asc" ? "author_desc" : "author_asc";
            ViewBag.PriceSortParam = sortOrder == "category_asc" ? "category_desc" : "category_asc";

            return View(ProductList);

            
        }
        public IActionResult Details(int productId)
        {
            Product product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType,ProductImages");

            if (product == null)
            {
                // Handle the case where the product is not found.
                return NotFound();
            }

            ShoppingCart cartObj = new ShoppingCart
            {
                Product = product,
                Count = 1,
                ProductId = productId
            };

            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartObj = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == claim.Value && 
                    u.ProductId == shoppingCart.ProductId
                );

            if (cartObj == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, 
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartObj, shoppingCart.Count);
                _unitOfWork.Save();
            }
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}