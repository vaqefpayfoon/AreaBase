﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AreaBase.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "تاریخ")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "مبلغ اصلی")]
        public double OrderTotalOriginal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name ="مبلغ پرداخت")]
        public double OrderTotal { get; set; }

        [Required]
        [Display(Name = "زمان ارسال")]
        public DateTime PickUpTime { get; set; }

        [Required]
        [NotMapped]
        public DateTime PickUpDate { get; set; }

       [Display(Name ="کد کوپن")]
        public string CouponCode { get; set; }
        [Display(Name = "تخفیف کوپن")]
        public double CouponCodeDiscount { get; set; }
        [Display(Name = "وضعیت")]
        public string Status { get; set; }
        [Display(Name = "وضعیت پرداخت")]
        public string PaymentStatus { get; set; }
        [Display(Name = "نظرات")]
        public string Comments { get; set; }


        [Display(Name = "نام ارسال")]
        public string PickupName { get; set; }

        [Display(Name = "تلفن")]
        public string PhoneNumber { get; set; }

        public string TransactionId { get; set; }




    }
}
