using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Models.ViewModels;
using MVCProject1.Models;
using MVCProject1.Utility;
using System.Security.Claims;
using Stripe;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Twilio;
using Microsoft.Extensions.Options;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Clients;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Google.Type;
using Twilio.Types;
using NuGet.Packaging.Signing;
using System.Net;
using System.Net.Mail;
using NuGet.Protocol.Plugins;

namespace MVCProject1.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    //private readonly ISmsSender _smsSender;

    //public CartController(ISmsSender smsSender)
    //{
    //    _smsSender = smsSender;
    //}
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITwilioRestClient _client;


        public CartController(IUnitOfWork unitOfWork, IEmailSender emailsender, UserManager<IdentityUser> userManager,
           ITwilioRestClient client)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailsender;
            _userManager = userManager;
            _client = client;

        }
        [BindProperty] // in which we can't give parameter.
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);


            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.
                GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()

            };

            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUser.FirstOrDefult
                (au => au.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,
                    list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                list.Product.Discription = SD.ConvertToRawHtml(list.Product.Discription);
                if (list.Product.Discription.Length > 100)
                {
                    list.Product.Discription = list.Product.Discription.Substring(0, 99) + ".....";
                }
            }
            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been send kindly verify your email!";
                ViewBag.EmailCSS = "text-success";
                isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email Must be confirem for authorize customer";
                ViewBag.EmailCSS = "text-danger";
            }
            return View(ShoppingCartVM);

        }
        public IActionResult plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefult(sc => sc.Id == cartId);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefult(sc => sc.Id == cartId);
            cart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult delete(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefult(sc => sc.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            //session update
            var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId
            == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartSession, count);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary(string ids)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (ids == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    OrderHeader = new OrderHeader(),
                    ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId ==
                    claim.Value, includeProperties: "Product")
                };
            }
            else {
                ShoppingCartVM = new ShoppingCartVM()
                {

                    ListCart = _unitOfWork.ShoppingCart.GetAll(Sc => ids.Contains(Sc.Id.ToString()), includeProperties: "Product"),
                    OrderHeader = new OrderHeader()

                };

            }

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.
                FirstOrDefult(u => u.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,
                    list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                list.Product.Discription = SD.ConvertToRawHtml(list.Product.Discription);
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken, string ids)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; //it used to match the user id.
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefult(u => u.Id == claim.Value);
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product").
                Where(x => ids.Contains(x.Id.ToString()));

            //ShoppingCartVM.OrderHeader.ApplicationUser =
            //    _unitOfWork.ApplicationUser.FirstOrDefult
            //    (u => u.Id == claim.Value);

            //ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll
            //    (sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);

            _unitOfWork.Save();

            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = SD.GetPriceBasedOnQuantity(item.Count, item.Product.Price,
                    item.Product.Price50, item.Product.Price100);
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += orderDetail.Price * orderDetail.Count;
                _unitOfWork.OrderDetail.Add(orderDetail);
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);


            #region Stripe Payment
            if (stripeToken == null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = System.DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            else
            {
                //Payment Process
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "Order Id : " + ShoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };
                //Payment
                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.Carrier = "";
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = System.DateTime.Now;
                }
            }
            _unitOfWork.Save();
            #endregion
            //Session it helps to store the data in the cookie
            HttpContext.Session.SetInt32(SD.ssShoppingCartSession, 0);

            return RedirectToAction("OrderConfirmation", "Cart",
                new { Id = ShoppingCartVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult OrderConfirmation(int Id)
        {
          
            return View(Id);
        }
     



        }
    }
    

