using DoAnNguyenVanTruong.Models.VN_Pay;
using DoAnNguyenVanTruong.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnNguyenVanTruong.ViewModel;

namespace DoAnNguyenVanTruong.Controllers
{
    public class CartbuyController : Controller
    {
        // GET: Cartbuy
        BaseAppDbContext db = new BaseAppDbContext();
        // GET: Cart

        public List<OrderDetail> getcart()
        {
            List<OrderDetail> orderDetails = Session["Cart"] as List<OrderDetail>;
            if (orderDetails == null || Session["Cart"] == null)
            {
                orderDetails = new List<OrderDetail>();
                Session["Cart"] = orderDetails;
            }
            return orderDetails;
        }

        public ActionResult Detail_Order()
        {
            var cart = Session["Cart"] as List<OrderDetail> ?? new List<OrderDetail>();
            return View(cart);
        }

        public ActionResult ShowCart()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Page");
            }
            else
            {

                if (Session["Cart"] == null)
                {
                    ViewBag.totalmoney = 0;
                    return View();

                }
                else
                {
                    List<OrderDetail> listcart = Session["Cart"] as List<OrderDetail>;
                    ViewBag.totalmoney = TotalMoney();
                    return View(listcart);
                }

            }

        }
        public double TotalMoney()
        {
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;

            double totalmoney = (double)carts.Sum(s => s.Product_Size.Product.newprice(s.Product_Size.Product.SaleOff, s.Product_Size.Product.Price) * s.OrderQuantity);

            return totalmoney;
        }

        [HttpPost]
        public JsonResult AddtoCart(int productID)
        {
            if (Session["User"] == null)
            {
                return Json(new { message = "user havent logged in"});
            }
            var cart = Session["Cart"] as List<OrderDetail> ?? new List<OrderDetail>();
            int sizeId = 1;
            var pro = db.Product_Size.SingleOrDefault(s => s.ProductID == productID && s.SizeID == sizeId);
            if (pro != null)
            {

                var pro_size = getcart().FirstOrDefault(s => s.Product_Size.ProductID == productID && s.Product_Size.SizeID == sizeId);
                if (pro_size == null)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.Product_Size = pro;
                    orderDetail.OrderQuantity = 1;
                    getcart().Add(orderDetail);
                }
                if (pro_size != null)
                {
                    pro_size.OrderQuantity += 1;
                }
                
            }
            return Json(new
            {
                cartItemCount = getcart().Count(),
                productQuantity = getcart().Count(),
            });
        }
        public JsonResult UpdateCartup(int id, int quantity)
        {
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;
            var item = carts.Find(s => s.Product_Size.ProductID == id);
            if (item != null)
            {
                item.OrderQuantity = quantity;

                var newSubtotal = item.OrderQuantity * item.Product_Size.Product.newprice(item.Product_Size.Product.Price, item.Product_Size.Product.SaleOff);

                var newTotal = carts.Sum(p => p.OrderQuantity * p.Product_Size.Product.newprice(p.Product_Size.Product.Price, p.Product_Size.Product.SaleOff));

                return Json(new
                {
                    newSubTotal = String.Format("{0:0,0 đ}", newSubtotal),
                    newTotal = String.Format("{0:0,0 đ}", newTotal)
                });
            }
            return Json(new { success = false, message = "Product not found or cart is empty." });
        }

        public JsonResult RemoveItemFromCart(int id)
        {
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;
            var item = carts?.Find(s => s.Product_Size.Product.ProductID == id);

            if (item != null)
            {
                carts.Remove(item);

                var newTotal = carts.Sum(p => p.OrderQuantity * p.Product_Size.Product.newprice(p.Product_Size.Product.Price, p.Product_Size.Product.SaleOff));

                Session["Cart"] = carts;

                return Json(new
                {
                    success = true,
                    newTotal = newTotal
                });
            }

            return Json(new { success = false, message = "Product not found or cart is empty." });
        }
            public ActionResult UpdateCartdown(int id)
        {
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;
            var item = carts.Find(s => s.Product_SizeID == id);
            if (item != null && item.Product_Size.Quantity > 0)
            {
                if (item.OrderQuantity >= 1)
                {
                    item.OrderQuantity -= 1;
                }

            }
            return RedirectToAction("ShowCart", "Cartbuy");
        }
        public PartialViewResult TotalQuantity()
        {
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;
            int total_quantity;
            if (carts == null)
            {
                total_quantity = 0;
            }
            else
            {
                total_quantity = (int) carts.Sum(s => s.OrderQuantity);

            }
            ViewBag.total_quantity = total_quantity;
            return PartialView("TotalQuantity");
        }
        public ActionResult Delete_Products(int id)
        {
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;
            var item = carts.Find(s => s.Product_SizeID == id);
            if (item != null)
            {
                carts.Remove(item);
            }
            return RedirectToAction("ShowCart", "Cartbuy");
        }
        public ActionResult CheckOut()
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();

            var orderDetails = Session["Cart"] as List<OrderDetail>;
            return View(new OrderDetailViewModel
            {
                Brands = brands,
                Slides = slides,
                Categories = categories,
                OrderDetails = orderDetails
            });
        }


        [HttpPost]
        public ActionResult CheckOut(FormCollection form)
        {
            int TypePayMent = int.Parse(form["TypePayment"]);
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;


            Order order = new Order();
            order.Address_Delivery = form["Address_Delivery"];
            order.PhoneCustomer = form["phone"];
            order.CustomerName = form["Name"];
            order.OrderDate = DateTime.Now;
            order.OrderTotalPrice = TotalMoney();
            order.TypePayment = TypePayMent;
            Random rd = new Random();
            order.OrderCode = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
            order.OrderStatus = "0";
            order.UserID = (int)Session["UserID"];
            db.Orders.Add(order);
            foreach (var item in carts)
            {
                OrderDetail orderdetail = new OrderDetail();
                int proid = item.Product_Size.Product.ProductID;
                int sizeid = 1;
                Product_Size pro_size = db.Product_Size.FirstOrDefault(s => s.Product.ProductID == proid && s.SizeID == sizeid);
                orderdetail.Product_SizeID = pro_size.Product_SizeID;
                orderdetail.OrderQuantity = item.OrderQuantity;
                orderdetail.TotalPrice = TotalMoney();
                orderdetail.OrderID = order.OrderID;
                Product foundedProd = db.Products.FirstOrDefault(prod => prod.ProductID == proid);
                foundedProd.TotalRevenue += (double)item.OrderQuantity * (double)foundedProd.Price;
                db.OrderDetails.Add(orderdetail);
            }
            db.SaveChanges();

            Session["OrderID"] = order.OrderID;

            //carts.Clear();
            string url = UrlPayment(order.OrderCode);
            if (TypePayMent > 0)
            {
                return Redirect(url);
            }
            else
            {
                return RedirectToAction("SuccessOrder", "Cartbuy");
            }

        }

        public ActionResult SuccessOrder()
        {
            List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;
            
            foreach (var item in carts)
            {
                int proid = item.Product_Size.Product.ProductID;
                int sizeid = 1;
                Product_Size pro_size = db.Product_Size.FirstOrDefault(s => s.Product.ProductID == proid && s.SizeID == sizeid);
                pro_size.Quantity -= item.OrderQuantity;
                db.Product_Size.Attach(pro_size);
                db.Entry(pro_size).State = System.Data.Entity.EntityState.Modified;
                
            }
            carts.Clear();
            db.SaveChanges();

            int orderId = (int)Session["OrderID"];

            return RedirectToAction("OrderDetail", "Page", new { orderId = orderId });
        }
        public string UrlPayment(string ordercode)
        {
            int orderId = (int)Session["OrderID"];
            //Get Config Info
            string vnp_Returnurl = "https://localhost:44374/Cartbuy/VnPay_Result"; //URL nhan ket qua tra ve 
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = "EKMX7WFQ"; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = "05JMFUBSH28ZXBS0JNLK5SCRDRYGC78Z"; //Secret Key



            //Save order to db
            Order item = db.Orders.FirstOrDefault(s => s.OrderID == orderId);
            var price = (long)item.OrderTotalPrice * 100;
            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (price).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "NCB");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang Mobile");
            vnpay.AddRequestData("vnp_OrderType", "VNPAY"); //default value: other
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", ordercode.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return paymentUrl;
        }

        public ActionResult VnPay_Result()
        {
            if (Request.QueryString.Count > 0)
            {
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    // Lấy tất cả dữ liệu từ querystring
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }

                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                bool checkSignature = vnpay.ValidateSignature(Request.QueryString["vnp_SecureHash"], "05JMFUBSH28ZXBS0JNLK5SCRDRYGC78Z");

                if (checkSignature && vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    // Xử lý đơn hàng thành công
                    var itemOrder = db.Orders.FirstOrDefault(x => x.OrderCode == orderCode);
                    if (itemOrder != null)
                    {
                        List<OrderDetail> carts = Session["Cart"] as List<OrderDetail>;
                        foreach (var item in carts)
                        {
                            // Cập nhật số lượng sản phẩm trong kho
                        }
                        carts.Clear();
                        itemOrder.OrderStatus = "1"; 
                        db.SaveChanges();
                    }

                    int orderId = (int)itemOrder.OrderID; // Lấy OrderID
                    return RedirectToAction("OrderDetail", "Page", new { orderId = orderId });
                }
            }
            return RedirectToAction("Index", "Page");
        }


        [HttpPost]
        public JsonResult AddItemToCart(int id)
        {
            if (Session["UserID"] == null)
            {
                return Json(new { message = "user havent logged in" });
            }
            var cart = Session["Cart"] as List<OrderDetail> ?? new List<OrderDetail>();

            var existingItem = cart.FirstOrDefault(c => c.Product_Size.Product.ProductID == id);
            if (existingItem != null)
            {
                existingItem.OrderQuantity++;
            }
            else
            {
                var product = db.Products.Where(p => p.ProductID == id).FirstOrDefault();
                cart.Add(new OrderDetail 
                { 
                    Product_Size = new Product_Size
                    {
                        ProductID = product.ProductID,
                        Product = product,
                    }, 
                    OrderQuantity = 1 
                });
            }

            // Lưu giỏ hàng vào session
            Session["Cart"] = cart;

            return Json(new
            {
                cartItemCount = cart.Count,
                productQuantity = existingItem != null ? existingItem.OrderQuantity : 1
            });
        }

        [HttpPost]
        public JsonResult AddItemToCartWithQuantity(int id, int qty)
        {
            if (Session["UserID"] == null)
            {
                return Json(new { message = "user havent logged in" });
            }
            var cart = Session["Cart"] as List<OrderDetail> ?? new List<OrderDetail>();

            var existingItem = cart.FirstOrDefault(c => c.Product_Size.Product.ProductID == id);
            if (existingItem != null)
            {
                existingItem.OrderQuantity += qty;
            }
            else
            {
                var product = db.Products.Where(p => p.ProductID == id).FirstOrDefault();
                cart.Add(new OrderDetail
                {
                    Product_Size = new Product_Size
                    {
                        ProductID = product.ProductID,
                        Product = product,
                    },
                    OrderQuantity = qty
                });
            }

            // Lưu giỏ hàng vào session
            Session["Cart"] = cart;

            return Json(new
            {
                cartItemCount = cart.Count,
                productQuantity = existingItem != null ? existingItem.OrderQuantity : 1
            });
        }
    }
}