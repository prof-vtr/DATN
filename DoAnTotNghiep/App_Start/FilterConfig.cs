using System.Web;
using System.Web.Mvc;

namespace DoAn_TranTheAnh_2020607026
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
