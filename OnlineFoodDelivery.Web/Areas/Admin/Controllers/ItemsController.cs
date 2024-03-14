using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineFoodDelivery.Models;
using OnlineFoodDelivery.Repository;
using OnlineFoodDelivery.Web.ViewModels;

namespace OnlineFoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _webHostEnvironment;

        public ItemsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var items = _context.Items.Include(x => x.Category).Include(y => y.SubCategory)
                .Select(model=> new ItemViewModel()
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    SubCategoryId = model.SubCategoryId,
                    Category = model.Category,
                    SubCategory = model.SubCategory,
                    Image=model.Image
                }).ToList();

            return View(items);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ItemViewModel vm = new ItemViewModel();
            ViewBag.Category = new SelectList(_context.Categories,"Id", "Title");
            ViewBag.SubCategory = new SelectList(_context.SubCategories, "Id", "Title");

            return View();
        }
        [HttpGet]
        public IActionResult GetSubCategory(int categoryId)
        {
            var subCategory = _context.SubCategories.Where(x=>x.CategoryId == categoryId).FirstOrDefault();


            return Json(subCategory);
        }
        [HttpPost]
        public IActionResult Create(ItemViewModel vm)
        {
            Item model = new Item();
            if (ModelState.IsValid)
            {
                var files = Request.Form.Files;
                byte[] photo = null;
                using (var filestream = files[0].OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        filestream.CopyTo(memoryStream);
                        photo = memoryStream.ToArray();
                    }
                }
                vm.Image= photo;
                model.Price = vm.Price;
                model.Description = vm.Description;
                model.Title = vm.Title;
                model.CategoryId = vm.CategoryId;
                model.SubCategoryId = vm.SubCategoryId; 
                model.Image = vm.Image;
                _context.Items.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            

            return View(vm);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            //var item = _context.Items.Where(x => x.Id == id).FirstOrDefault();
            //ItemViewModel viewModel = new ItemViewModel();
            var viewModel = _context.Items.Where(x => x.Id == id).Select(x => new ItemViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Image = x.Image,
                Price = x.Price
            }).FirstOrDefault();
            ViewBag.Category = new SelectList(_context.Categories, "Id", "Title", viewModel.CategoryId);
            ViewBag.SubCategory = new SelectList(_context.SubCategories, "Id", "Title",viewModel.SubCategoryId);


            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(ItemViewModel vm)
        {
            Item model = _context.Items.Where(x => x.Id == vm.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] photo = null;
                    using (var filestream = files[0].OpenReadStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            filestream.CopyTo(memoryStream);
                            photo = memoryStream.ToArray();
                        }
                    }
                    model.Image= photo;
                }
                //var model = _context.Items.FirstOrDefault(x => x.Id == vm.Id);
                model.Title = vm.Title;
                model.Description= vm.Description;
                model.Price = vm.Price;
                model.CategoryId = vm.CategoryId;
                model.SubCategoryId = vm.SubCategoryId;
                model.Image = vm.Image;
                _context.Items.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _context.Items.Where(x => x.Id == id).FirstOrDefault();
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var item = _context.Items
                                .Include(i => i.Category)
                                .Include(i => i.SubCategory)
                                .FirstOrDefault(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }
            string base64Image = Convert.ToBase64String(item.Image);
            ViewBag.ImageData = base64Image;
            return View(item);
        }
    }
}
