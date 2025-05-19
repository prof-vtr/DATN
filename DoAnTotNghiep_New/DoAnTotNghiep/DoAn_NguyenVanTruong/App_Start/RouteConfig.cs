using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DoAnNguyenVanTruong
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Default route – chỉ định namespace để tránh trùng controller
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Page", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "DoAnNguyenVanTruong.Controllers" } // namespace bạn đang dùng
            );

            // GridShop route
            routes.MapRoute(
                name: "GridShop",
                url: "shop/category/{categoryId}",
                defaults: new { controller = "Page", action = "GridShop" },
                namespaces: new[] { "DoAnNguyenVanTruong.Controllers" }
            );

            // DetailProduct route
            routes.MapRoute(
                name: "DetailProduct",
                url: "product/{productId}",
                defaults: new { controller = "Page", action = "DetailProduct" },
                namespaces: new[] { "DoAnNguyenVanTruong.Controllers" }
            );
        }
    }
}
