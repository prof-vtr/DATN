using DoAnNguyenVanTruong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using System.Drawing.Printing;
using System.Web.UI;
using DoAnNguyenVanTruong.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics;
using System.Web.UI.WebControls;
using DoAnNguyenVanTruong.Service;

namespace DoAnNguyenVanTruong.Controllers
{
    public class PageController : Controller
    {
        private BaseAppDbContext db = new BaseAppDbContext();
        private readonly ProductRecommendationService _recommendationService;

        public PageController()
        {
            _recommendationService = new ProductRecommendationService(db);
        }

        // GET: Page
       
        public ActionResult Index()
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();
            var products = db.Products.ToList();

            // Lấy danh sách sản phẩm bán chạy nhất
            var bestSellers = db.Products
                .OrderByDescending(p => p.TotalRevenue)
                .Take(10)
                .ToList();

            List<Product> recommendedProducts = new List<Product>();
            if (Session["UserID"] != null)
            {
                int userId = (int)Session["UserID"];
                recommendedProducts = _recommendationService.GetRecommendedProducts(userId);
            }
            else
            {
                recommendedProducts = null;
            }

            IndexViewModel result = new IndexViewModel()
            {
                Products = products.AsEnumerable(),
                Brands = brands,
                Slides = slides,
                Categories = categories,
                BestSellers = bestSellers,
                RecommendedProducts = recommendedProducts
            };

            return View("Index", result);
        }

        public ActionResult Header()
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();

            IndexViewModel result = new IndexViewModel()
            {
                Brands = brands,
                Slides = slides,
                Categories = categories
            };

            return View("Header", result);
        }

        public ActionResult DetailProduct(int productId)
        {
            Product product = db.Products.Where(prod => prod.ProductID == productId).FirstOrDefault();

            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();
            var relatedProducts = db.Products.Where(p => p.CategoryID == product.CategoryID).ToList();

            List<Product> recommendedProducts = new List<Product>();
            if (Session["UserID"] != null)
            {
                int userId = (int)Session["UserID"];
                recommendedProducts = _recommendationService.GetRecommendedProducts(userId);
            }
            else
            {
                recommendedProducts = null;
            }

            ProductDetailViewModel result = new ProductDetailViewModel()
            {
                Product = product,
                Brands = brands,
                Slides = slides,
                Categories = categories,
                RelatedProducts = relatedProducts,
                RecommendedProducts = recommendedProducts
            };

            return View("DetailProduct", result);

        }

        public ActionResult Login()
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();

            IndexViewModel result = new IndexViewModel()
            {
                Brands = brands,
                Slides = slides,
                Categories = categories
            };

            return View("Login", result);
        }

        public ActionResult Register()
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();

            IndexViewModel result = new IndexViewModel()
            {
                Brands = brands,
                Slides = slides,
                Categories = categories
            };

            return View("Register", result);
        }

        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();
            var products = db.Products.ToList();

            IndexViewModel result = new IndexViewModel()
            {
                Products = products.AsEnumerable(),
                Brands = brands,
                Slides = slides,
                Categories = categories
            };

            // SQL injection
            string query = "SELECT * FROM Users WHERE Email = '" + Email + "' AND Password = '" + Password + "'";
            User item = db.Users.SqlQuery(query).FirstOrDefault();

            if (item != null)
            {
                Session["UserID"] = item.UserID;
                Session["UserEmail"] = item.Email;
                Session["Role"] = item.RoleID;
                Session["Cart"] = null;
                if (item.RoleID!= 1)
                {
                    return RedirectToAction("Index", "Page");
                }

            }
            else
            {
                ViewBag.erorr = "Sai tài khoản hoặc mật khẩu!";
            }
            return View(result);

        }


        [HttpPost]
        public ActionResult Register(FormCollection form)
        {

            User user = new User();

            if (form["password"] != form["confirmpassword"])
            {
                ViewBag.errorpassword = "Xác nhận password chưa đúng!";
                return View();
            }
            else
            {
                user.Username = form["firstname"] + " " + form["lastname"];
                user.Email = form["email"];
                user.Password = form["password"];
                user.Gender = form["Gender"];
                user.RoleID = 0;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login", "Page");
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("UserID");
            Session.Remove("Admin");
            Session.Remove("Cart");
            return RedirectToAction("Index", "Page");
        }

        public ActionResult GetSortedProducts(int categoryId, string sortOrder, int page = 1)
        {
            int pageSize = 6; // Số sản phẩm mỗi trang
            var products = db.Products.Where(p => p.CategoryID == categoryId);

            // Sắp xếp theo yêu cầu
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.newprice(p.Price, p.SaleOff));
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.newprice(p.Price, p.SaleOff));
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductID);
                    break;
            }

            // Phân trang
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return PartialView("_ProductListPartial", pagedProducts);
        }

        public ActionResult ListproductCategory(int id, int? page, int? PageSize)
        {
            List<Product> list = null;
            
            if (page == null)
            {
                page = 1;
            }
            if (PageSize == null)
            {

                PageSize = 8;
            }
            list = db.Products.Where(m => m.CategoryID == id).ToList();
            return View(list.ToPagedList((int)page, (int)PageSize));
        }

        public ActionResult GetProductsByBrand(int brandId, int? page, int? pageSize)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 8;
            }

            var products = db.Products.Where(p => p.BrandID == brandId).ToList(); // Lọc sản phẩm theo categoryId
            return PartialView("_ProductList", products.ToPagedList((int)page, (int)pageSize)); // Trả về PartialView
        }

        public ActionResult GridShop(int? page, int? pagesize, string categoryId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pagesize == null)
            {
                pagesize = 8;
            }

            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();
            var products = db.Products.Where(product => product.Category.CategoryName.Equals(categoryId)).ToList();

            List<Product> recommendedProducts = new List<Product>();
            if (Session["UserID"] != null)
            {
                int userId = (int)Session["UserID"];
                recommendedProducts = _recommendationService.GetRecommendedProducts(userId);
            }
            else
            {
                recommendedProducts = null;
            }

            GridShopViewModel result = new GridShopViewModel()
            {
                Products = products.ToPagedList((int)page, (int)pagesize),
                Brands = brands,
                Slides = slides,
                Categories = categories,
                RecommendedProducts = recommendedProducts
            };

            return View("GridShop", result);
        }


        public ActionResult CheckOut()
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();

            var orderDetails = Session["Cart"] as List<OrderDetail>;
            return View("CheckOut", new OrderDetailViewModel
            {
                Brands = brands,
                Slides = slides,
                Categories = categories,
                OrderDetails = orderDetails
            });
        }

        public ActionResult OrderDetail(int orderId)
        {
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();
            var result = db.Orders.Where(order => order.OrderID == orderId).FirstOrDefault();
            return View("OrderDetail", new OrderDetail2Model
            {
                Brands = brands,
                Slides = slides,
                Categories = categories,
                Order = result
            });
        }

        public PartialViewResult slider()
        {
            List<Slide> listslide = db.Slides.ToList();
            return PartialView(listslide);
        }
        public PartialViewResult ListCategory()
        {
            List<Category> listcategory = db.Categories.ToList();
            return PartialView(listcategory);
        }
        public PartialViewResult listcategory_menu()
        {
            List<Category> listcategory = db.Categories.ToList();
            return PartialView(listcategory);
        }
        [HttpPost]
        public ActionResult Search(FormCollection form, int? page, int? PageSize)
        {
           
                if (page == null)
                {
                    page = 1;
                }
                if (PageSize == null)
                {

                    PageSize = 8;
                }

            string str = form["SearchString"];
            var sanphams = db.Products.Where(s => s.ProductName.Contains(str)).ToList();
            return View(sanphams.ToPagedList((int)page, (int)PageSize));
         
            
           
        }
        
        [HttpPost]
        public ActionResult RateProduct(FormCollection form)
        {
            int rateValue = int.Parse(form["rate"]);
            int ProductId = int.Parse(form["ProductID"]);
            string Comment = form["evaluate_content"];
            var item = db.Products.SingleOrDefault(s => s.ProductID == ProductId);
            int userid = (int)Session["UserID"];
            var user = db.Users.SingleOrDefault(s=>s.UserID == userid);
            if (item != null && rateValue != null && Comment != null && user !=null)
            {
                Rate rate = new Rate();
                rate.RateValue = rateValue;
                rate.Product = item;
                rate.DateRate = DateTime.Now;
                rate.User = user;
                db.Rates.Add(rate);
                db.SaveChanges();
            }
            return RedirectToAction("DetailProduct",new {id=ProductId});
        }
        
        public ActionResult Account()
        {
            int id = (int)Session["UserID"];
            var user = db.Users.FirstOrDefault(s => s.UserID == id);
            string role = null;
            if (user.RoleID == 0)
            {
                role = "Khách hàng";
            }
            else if (user.RoleID == 1)
            {
                role = "Admin";
            }
            ViewBag.role = role;
            return View(user);
        }
        public ActionResult My_order(int? page, int? pagesize)
        {
            int id = (int)Session["UserID"];
           
            if (page == null)
            {
                page = 1;
            }
            if (pagesize == null)
            {

                pagesize = 4;
            }
            List<Order> orders = db.Orders.Where(s => s.UserID == id).ToList();
            return View(orders.ToPagedList((int)page, (int)pagesize));
        }
        public ActionResult Detail_Order()
        {
            /*            List<OrderDetail> orderdetails = db.OrderDetails.Where(s => s.OrderID == id).ToList();
                        Order order = db.Orders.FirstOrDefault(s => s.OrderID == id);
            
            ViewBag.code = order;
            ViewBag.timegiao = order.OrderDate.AddDays(3);
            */

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
        public JsonResult Cancel_Order(int id)
        {
            Order order = db.Orders.FirstOrDefault(s=>s.OrderID == id);
            order.OrderStatus = "5";
            List<OrderDetail> orderDetail = db.OrderDetails.Where(s => s.OrderID == order.OrderID).ToList();
            foreach(var item in orderDetail)
            {
                Product_Size product_Size = db.Product_Size.FirstOrDefault(s => s.ProductID == item.Product_Size.ProductID && s.SizeID == item.Product_Size.SizeID);
                product_Size.Quantity += item.OrderQuantity;
            }
            db.Orders.Attach(order);
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                message = "Huỷ đơn hàng thành công"
            });
        }
        public ActionResult Received_Goods(int id)
        {
            Order order = db.Orders.FirstOrDefault(s => s.OrderID == id);
            order.OrderStatus = "4";
            db.Orders.Attach(order);
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("My_order", "Page");
        }

        public ActionResult UserListOrders()
        {
            int userId = (int)Session["UserID"];
            if (userId <= 0) 
            {
                return View("Login", "Page");
            }
            var brands = db.Brands.ToList();
            var slides = db.Slides.ToList();
            var categories = db.Categories.ToList();
            List<Order> orderList = db.Orders.Where(order => order.UserID == userId).ToList();
            return View("UserListOrders", new OrderListViewModel
            {
                Brands = brands,
                Slides = slides,
                Categories = categories,
                Orders = orderList,
            });
        }

    }
}