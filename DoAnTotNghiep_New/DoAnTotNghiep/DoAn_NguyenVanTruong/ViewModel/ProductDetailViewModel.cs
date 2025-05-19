using DoAnNguyenVanTruong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnNguyenVanTruong.ViewModel
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Slide> Slides { get; set; }
        public List<Category> Categories { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<Product> RecommendedProducts { get; set; }
    }
}