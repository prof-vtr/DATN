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
    public class CartApiController : ApiController
    {

        BaseAppDbContext db = new BaseAppDbContext();

        [HttpGet()]
        [Route("getall")]
        public IEnumerable<CartItem> GetAllCart()
        {
            var result = db.CartItems.ToList();
            return result;
        }
    }
}
