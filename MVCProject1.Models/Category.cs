using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCProject1.Models
{
    public class Category
    {
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }

        //in which we use Data annotation
         // in this model we provide some speaciality to some colunms.
    }
}
