using DoAnNguyenVanTruong.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnNguyenVanTruong.ViewModel
{
    public class GridShopViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Slide> Slides { get; set; }
        public List<Category> Categories { get; set; }
        public List<Product> RecommendedProducts { get; set; }
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public String CategoryId { get; set; }

    }
}