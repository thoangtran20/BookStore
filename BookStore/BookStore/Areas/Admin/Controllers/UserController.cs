using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private IWebHostEnvironment _webHostEnvironment;
        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API_CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _dbContext.ApplicationUsers.Include(u => u.Company).ToList();
            return Json(new {data = objUserList });
        }

        // post
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
