using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodDelivery.Models;
using OnlineFoodDelivery.Repository;
using OnlineFoodDelivery.Web.ViewModels;
using SQLitePCL;
using System.Security.Claims;

namespace OnlineFoodDelivery.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ItemSuggestion()
        {
            return View(null);
        }
        [HttpGet]
        public IActionResult Index(int? categoryId)
        {
            IQueryable<Item> query = _context.Items.Include(x => x.Category).Include(y => y.SubCategory);

            if (categoryId.HasValue)
            {
                query = query.Where(item => item.CategoryId == categoryId);
            }

            var viewModel = new ItemCategoryViewModel
            {
                Items = query.ToList(),
                Categories = _context.Categories.ToList()
            };

            return View(viewModel);
        }
        //public IActionResult Index()
        //{
        //    var items = _context.Items.Include(x => x.Category).Include(y => y.SubCategory)
        //        .Select(model => new ItemViewModel()
        //        {
        //            Id = model.Id,
        //            Title = model.Title,
        //            Description = model.Description,
        //            Price = model.Price,
        //            CategoryId = model.CategoryId,
        //            SubCategoryId = model.SubCategoryId,
        //            Category = model.Category,
        //            SubCategory = model.SubCategory,
        //            Image = model.Image
        //        }).ToList();

        //    return View(items);
        //}
        [HttpGet]
        public IActionResult Details(int id)
        {
            var item = _context.Items
                                 .Include(i => i.Category)
                                 .Include(i => i.SubCategory)
                                 .FirstOrDefault(i => i.Id == id);
            string base64Image = Convert.ToBase64String(item.Image);
            ViewBag.ImageData = base64Image;
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Details(Cart cart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                var cartfromDb =  _context.Carts.Where(x=>x.ApplicationUserId == cart.ApplicationUserId
                && x.ItemId == cart.ItemId).FirstOrDefaultAsync();
                if(cartfromDb == null)
                {
                    _context.Carts.Add(cart);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
