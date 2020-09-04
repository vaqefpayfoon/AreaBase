using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AreaBase.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام منو")]
        public string Name { get; set; }
        [Display(Name = "شرح")]
        public string Description { get; set; }
        [Display(Name = "نوع")]
        public string Spicyness { get; set; }
        public enum ESpicy { NA = 0, NotSpicy = 1, Spicy = 2, VerySpicy = 3 }
        [Display(Name = "تصویر")]
        public string Image { get; set; }

        [Display(Name = "فهرست")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "زیرگروه")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = " Price should be greater than ${1}")]
        [Display(Name = "قیمت")]
        public double Price { get; set; }


    }
}
