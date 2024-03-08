using Microsoft.AspNetCore.Mvc;

namespace OnlineFoodDelivery.Web.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
