using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCProject1.DataAccess.Repository.Repository;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Models;
using MVCProject1.Models.ViewModels;
using MVCProject1.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace MVCProject1.Areas.Customer.Controllers
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

        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll
               (includeProperties: "Category,CoverType");
            var claimIdentity = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll
                    (s => s.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartSession, count);
            }


            return View(productList);
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
        public IActionResult Details(int id)
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartSession,count);
            }

            var productInDb = _unitOfWork.Product.FirstOrDefult
                (p => p.Id == id, includeProperties: "Category,CoverType");
            if (productInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Details(ShoppingCart shoppingCartObj)
        {
            shoppingCartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)(User.Identity);
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCartObj.ApplicationUserId = claim.Value;
                var shoppingCartFromDb = _unitOfWork.ShoppingCart.FirstOrDefult
                    (s => s.ApplicationUserId == claim.Value &&
                    s.ProductId == shoppingCartObj.ProductId,
                    includeProperties: "Product");
                if (shoppingCartFromDb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCartObj);
                else
                    shoppingCartFromDb.Count += shoppingCartObj.Count;
                _unitOfWork.Save();
                //Session
                var count = _unitOfWork.ShoppingCart.GetAll
                    (u => u.ApplicationUserId == claim.Value).
                    ToList().Count;

                HttpContext.Session.SetInt32(SD.ssShoppingCartSession, count);

                //***
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefult
               (p => p.Id == shoppingCartObj.ProductId,
               includeProperties: "Category,CoverType");
                var shoppingCart = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(shoppingCart);
            }
        }
    }
}
            
   