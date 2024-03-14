using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineFoodDelivery.Models;
using OnlineFoodDelivery.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OnlineFoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var users = _context.ApplicationUsers.ToList();
 

            return View(users);
        }
    }
}
