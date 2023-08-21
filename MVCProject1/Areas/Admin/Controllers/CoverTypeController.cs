using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Models;
using MVCProject1.Utility;
using System.Data;

namespace MVCProject1.Areas.Admin.Controllers
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
            if (id == null)
                return View(coverType);
            var param = new DynamicParameters();
            param.Add("@id", id);
            coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>
                (SD.Proc_GetCoverType, param);
            //coverType=_unitOfWork.CoverType.Get(id.GetValueOrDefault());
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                var param = new DynamicParameters();
                param.Add("@name", coverType.Name);
                if (coverType.Id == 0)
                    _unitOfWork.SP_CALL.Execute(SD.Proc_CreateCoverType, param);
                //_unitOfWork.CoverType.Add(coverType);
                else
                {
                    param.Add("@id", coverType.Id);
                    _unitOfWork.SP_CALL.Execute(SD.Proc_UpdateCoverType, param);
                }
                //_unitOfWork.CoverType.Update(coverType);
                //_unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
                return View(coverType);
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypeList = _unitOfWork.SP_CALL.List<CoverType>
                (SD.Proc_GetAllCoverTypes);
            return Json(new { data = coverTypeList });
            //return Json(new {data=_unitOfWork.CoverType.GetAll()});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("@id", id);
            var coverTypeInDb = _unitOfWork.SP_CALL.OneRecord<CoverType>
                (SD.Proc_GetCoverType, param);
            //var coverTypeInDb = _unitOfWork.CoverType.Get(id);
            if (coverTypeInDb == null)
                return Json(new { success = false, message = "Error while delete data !!!" });
            _unitOfWork.SP_CALL.Execute(SD.Proc_DeleteCoverType, param);
            //_unitOfWork.CoverType.Remove(coverTypeInDb);
            //_unitOfWork.Save();
            return Json(new { success = true, message = "data delete successfully !!!" });
        }
        #endregion

    }
}
