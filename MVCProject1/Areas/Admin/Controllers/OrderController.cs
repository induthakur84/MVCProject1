using Google.Api;
using Google.Rpc;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Models;
using MVCProject1.Models.ViewModels;
using MVCProject1.Utility;
using System.Buffers.Text;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using static Google.Protobuf.WellKnownTypes.Field.Types;
using static NuGet.Packaging.PackagingConstants;
using static MVCProject1.Models.ViewModels.OrderVM;
using static MVCProject1.Utility.SD;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCProject1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // lockin user
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork; // we intialize unit of work
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork) // we make constructor
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetAll();
            return View(orderHeader);
        }

        public IActionResult Pending()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetAll().Where(u => u.OrderStatus == SD.PaymentStatusPending).ToList();
            return View(orderHeader);
        }


        public IActionResult Approved()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetAll().Where(u => u.OrderStatus == SD.PaymentStatusApproved).ToList();
            return View(orderHeader);
        }

        public IActionResult ViewDetail(int id)
        {
            var orderheader = _unitOfWork.OrderHeader.FirstOrDefult(e => e.Id == id);
            return View(orderheader);
        }
        public IActionResult OrderDate(DateTime? startDate, DateTime? endDate)
        {
            var results = _unitOfWork.OrderHeader.GetAll()
               .Where(item => item.OrderDate >= startDate && item.OrderDate <= endDate)
              .ToList();
            return View(results);

        }
        //[HttpPost] //it helps to fetch user data and save in database.
        //[ValidateAntiForgeryToken]
        
        //public IActionResult OrderDate(OrderVM orderVM)
        //{
           

        //}

    }
}
    
       //     var orders = _unitOfWork.OrderHeader.GetAll();
              

            //if (string.IsNullOrEmpty(status))
            //{
            //    if (status != "Approved")
            //    {
            //        orders = _unitOfWork.OrderHeader.GetAll().Where(u => u.OrderStatus == SD.PaymentStatusApproved).ToList();
                   
            //    }
            //    else (status !="Pending")
            //            {
            //        orders = _unitOfWork.OrderHeader.GetAll().Where(u => u.OrderStatus == SD.PaymentStatusPending).ToList();
            //    }
            //    orders = _unitOfWork.OrderHeader.GetAll()
            //  .Where(item => item.OrderDate >= startDate && item.OrderDate <= endDate)
            //  .ToList();
            //}

    //        if (startDate.HasValue)
    //        {
    //            orders = orders.Where(o => o.OrderDate >= startDate.Value);
    //        }

    //        if (endDate.HasValue)
    //        {
    //            orders = orders.Where(o => o.OrderDate <= endDate.Value);
    //        }

    //        return View(orders.ToList());
    //    }
    //}
    //    {
    //        ViewData["OrderStatusOptions"] = new List<SelectListItem>
    //{
    //    new SelectListItem { Value = "Approved", Text = "Approved" },
    //    new SelectListItem { Value = "Pending", Text = "Pending" },
    //    new SelectListItem { Value = "AllOrder", Text = "AllOrder" }
    //};

    //        var results = _unitOfWork.OrderHeader.GetAll()
    //            .Where(item => item.OrderDate >= startDate && item.OrderDate <= endDate)
    //       .ToList();
    //        return View();

    //    }


    //public IActionResult OrderDates(string status, DateTime? startDate, DateTime? endDate)
    //        {
    //        var orders = _unitOfWork.OrderHeader.GetAll();

    //            if (!string.IsNullOrEmpty(status))
    //            {
    //                if (status != "All")
    //                {
    //                    orders = orders.Where(o => o.OrderStatus == status);
    //                }
    //            }

    //            if (startDate.HasValue)
    //            {
    //                orders = orders.Where(o => o.OrderDate >= startDate.Value);
    //            }

    //            if (endDate.HasValue)
    //            {
    //                orders = orders.Where(o => o.OrderDate <= endDate.Value);
    //            }

    //            return View(orders.ToList());
    //        }
    //    }


    //public IActionResult SearchApproved(DateTime startDate, DateTime endDate)
    //{
    //    var results = _unitOfWork.OrderHeader.GetAll()
    //       .Where(item => item.OrderDate >= startDate && item.OrderDate <= endDate)
    //       .ToList();
    //    return View(results);
    //    var desiredDates = _unitOfWork.OrderHeader.GetAll()
    //   .Where(item => item.OrderStatus == StatusApproved).Select(item => item.OrderDate >= startDate && item.OrderDate <= endDate)


    //   .ToList();

    //    return Ok(desiredDates);

    //}

    //public IActionResult SearchPending(string searchString, DateTime startDate, DateTime endDate)
    //{
    //    //var results = _unitOfWork.OrderHeader.GetAll()
    //    //   .Where(item => item.OrderDate >= startDate && item.OrderDate <= endDate)
    //    //   .ToList();
    //    //return View(results);
    //    var desiredDates = _unitOfWork.OrderHeader.GetAll()
    //   .Where(item => item.OrderStatus == StatusPending)
    //   .Select(order => order.OrderDate)
    //   .ToList();

    //    return Ok(desiredDates);







