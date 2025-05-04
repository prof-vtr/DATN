using DoAnNguyenVanTruong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnNguyenVanTruong.ViewModel
{
    public class OrderDetail2Model
    {
        public IEnumerable<Brand> Brands { get; set; }

        public IEnumerable<Slide> Slides { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public Order Order { get; set; }
    }
}