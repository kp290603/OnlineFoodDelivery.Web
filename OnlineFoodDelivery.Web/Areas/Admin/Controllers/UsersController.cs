using Microsoft.AspNetCore.Mvc;
using OnlineFoodDelivery.Repository;
using System.Security.Claims;

namespace OnlineFoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var x = _context.ApplicationUsers.Where(x => x.Id != claim.Value).ToList();
            return View(x);

        }
    }
}
