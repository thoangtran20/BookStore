using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null || id == 0)
            {
                // Create CoverType
                return View(coverType);
            }
            else
            {
                // Update CoverType
                coverType = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            }
            return View(coverType);
        }

        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitOfWork.CoverType.Add(obj);
                }
                else
                {
                    _unitOfWork.CoverType.Update(obj);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Cover Type Create Successfully";
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
            var coverTypeFromDB = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (coverTypeFromDB == null)
            {
                return NotFound();
            }
            return View(coverTypeFromDB);
        }

        #region API_CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypeList = _unitOfWork.CoverType.GetAll();
            return Json(new { data = coverTypeList });
        }

        // post
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.CoverType.Remove(obj);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successful" });
            }
        }
        #endregion
    }
}
