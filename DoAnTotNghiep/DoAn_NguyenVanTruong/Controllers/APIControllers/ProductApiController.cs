using DoAnNguyenVanTruong.ViewModel;
using DoAnNguyenVanTruong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DoAnNguyenVanTruong.Controllers.APIControllers
{
    [RoutePrefix("api/product")]
    public class ProductApiController : ApiController
    {

        BaseAppDbContext db = new BaseAppDbContext();

        [HttpGet()]
        [Route("getall")]
        public IEnumerable<ProductViewModel> GetAllProducts()
        {
            var result = db.Products.ToList();
            return result.Select(p => new ProductViewModel
            {
                ProductID = p.ProductID,
                Description = p.Description,
                Images = p.Images,
                Price = p.Price,
                ProductName = p.ProductName,
                SaleOff = p.SaleOff
            });
        }

        [HttpGet]
        [Route("get/{id}")]
        public ProductViewModel GetProductById(int id)
        {
            var result = db.Products.Where(p => p.ProductID == id).FirstOrDefault();
            return new ProductViewModel
            {
                ProductID = result.ProductID,
                Description = result.Description,
                Images = result.Images,
                Price = result.Price,
                ProductName = result.ProductName,
                SaleOff = result.SaleOff
            };  
        }


    }
}
