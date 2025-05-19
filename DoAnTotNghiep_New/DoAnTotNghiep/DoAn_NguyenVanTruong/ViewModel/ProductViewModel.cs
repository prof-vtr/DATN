using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnNguyenVanTruong.ViewModel
{
    public class ProductViewModel
    {
        public string ProductName { get; set; }

        public string Description { get; set; } 

        public string Images { get; set; }

        public double SaleOff { get; set; }

        public double Price { get; set; }

        public int ProductID { get; set; }

    }
}