using DoAnNguyenVanTruong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnNguyenVanTruong.ViewModel
{
    public class OrderListViewModel
    {
        public IEnumerable<Brand> Brands { get; set; }

        public IEnumerable<Slide> Slides { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public List<Order> Orders { get; set; }
    }
}