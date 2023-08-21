using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Models;
using MVCProject1.Utility;
using System.Data;
namespace MVCProject1.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
                return View(company);
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            return View(company);
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                    _unitOfWork.Company.Add(company);
                else
                    _unitOfWork.Company.Update(company);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Company.GetAll() });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyInDb = _unitOfWork.Company.Get(id);
            if (companyInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete datat !!" });
            _unitOfWork.Company.Remove(companyInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data successfully deleted  !!" });

        }
        #endregion


    }
}
