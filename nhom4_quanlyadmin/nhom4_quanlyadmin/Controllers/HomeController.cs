using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace nhom4_quanlyadmin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Error(int statusCode, string message)
        {
            ViewBag.StatusCode = statusCode;
            ViewBag.ErrorMessage = message ?? "Có lỗi xảy ra trong quá trình xử lý yêu cầu.";
            return View();
        }
    }
}
