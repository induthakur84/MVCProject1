using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MVCProject1.Utility.SD;

namespace MVCProject1.Models.ViewModels
{
    public class OrderVM // it helps to make the details of the order.
    {


       
            public DateTime? SelectedOrderDate { get; set; }
            public DateTime? SelectedPendingDate { get; set; }
            public DateTime? SelectedApprovedDate { get; set; }
        public IEnumerable<SelectListItem>? OrderHeaderList { get; set; }
    }

    }
       
    

