﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineFoodDelivery.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Title { get; set; }   
        public string Type { get; set; }
        public double Discount { get; set; }
        public double MinimumAmmount { get; set; }
        public byte[] CouponPicture { get; set; }
        public bool IsActive { get; set; }
    }
    public enum CouponType
    {
        Percent = 0,
        Curreny = 1
    }
}
