using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVCProject1.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Discription { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1, 10000)]
        public double ListPrice { get; set; } //manin prize
        [Required]
        [Range(1, 10000)]
        public double Price50 { get; set; }// why buy 50 books or above then prize 50
        [Required]
        [Range(1, 10000)]
        public double Price100 { get; set; } // why buy books 100 or above then
        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }
        [Display(Name = "Image Url")]
        public string ImageURL { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } // it is navigation property it used automatically make foreign key. it automatic sense
        [Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; } 
        [ForeignKey("CoverTypeId")] // it helps to fetch the data by ID.
        public CoverType CoverType { get; set; }
    }

}
