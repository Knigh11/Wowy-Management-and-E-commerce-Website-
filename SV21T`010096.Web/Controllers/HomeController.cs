using Microsoft.AspNetCore.Mvc;

namespace SV21T1020096.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
