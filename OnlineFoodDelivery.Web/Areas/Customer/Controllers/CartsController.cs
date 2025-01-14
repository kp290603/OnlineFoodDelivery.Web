﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineFoodDelivery.Models;
using OnlineFoodDelivery.Repository;
using OnlineFoodDelivery.Web.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace OnlineFoodDelivery.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        //[BindProperty]
        //public CartOrderViewModel details { get; set; }
        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{

        //    details = new CartOrderViewModel()
        //    {
        //        OrderHeader = new OrderHeader()
        //    };
        //    var claimsIdentity = (ClaimsIdentity)this.User.Identity;
        //    var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    details.ListofCart = _context.Carts.Where(x => x.ApplicationUserId == claims.Value)
        //        .ToList();
        //    if (details.ListofCart != null)
        //    {
        //        foreach (var cart in details.ListofCart)
        //        {
        //            details.OrderHeader.OrderTotal += (cart.Item.Price);
        //        }
        //    }
        //    return View(details);
        //}
        [Authorize]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetString("Cart");
            List<CartItemViewModel> Cart = cart != null ? JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart) : new List<CartItemViewModel>();

            return View(Cart);
        }

        public IActionResult AddToCart(int PId)
        {
            var cart = HttpContext.Session.GetString("Cart");
            List<CartItemViewModel> Cart;

            if (cart == null)
            {
                Cart = new List<CartItemViewModel>();
            }
            else
            {
                Cart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart);
            }

            bool found = false;

            foreach (var cartItem in Cart)
            {
                if (cartItem.Item.Id == PId)
                {
                    cartItem.Quantity++;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                CartItemViewModel item = new CartItemViewModel();
                item.Item = _context.Items.Find(PId);
                item.Quantity = 1;
                Cart.Add(item);
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(Cart));

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int PId, int Quantity)
        {
            var cart = HttpContext.Session.GetString("Cart");

            if (cart != null)
            {
                List<CartItemViewModel> Cart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart);

                var itemToUpdate = Cart.FirstOrDefault(item => item.Item.Id == PId);

                if (itemToUpdate != null)
                {
                    itemToUpdate.Quantity = Quantity;

                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(Cart));
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult RemoveFromCart(int PId)
        {
            var cart = HttpContext.Session.GetString("Cart");

            if (cart != null)
            {
                List<CartItemViewModel> Cart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart);

                var itemToRemove = Cart.FirstOrDefault(item => item.Item.Id == PId);

                if (itemToRemove != null)
                {
                    Cart.Remove(itemToRemove);

                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(Cart));
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetString("Cart");
            List<CartItemViewModel> Cart = cart != null ? JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart) : new List<CartItemViewModel>();

            double totalAmount = 0;
            foreach (var cartItem in Cart)
            {
                totalAmount += cartItem.Item.Price * cartItem.Quantity;
            }

            ViewData["TotalAmount"] = totalAmount;

            //var domain = "https://localhost:7080/";
            //var options = new SessionCreateOptions
            //{
            //    SuccessUrl = domain + $"Customer/Carts/OrderConfirmation",
            //    CancelUrl = domain + $"Homes/Home",
            //    LineItems = new List<SessionLineItemOptions>(),
            //    Mode="payment"
            //};
            //foreach (var item in Cart)
            //{
            //    var sessionListItem = new SessionLineItemOptions
            //    {
            //        PriceData = new SessionLineItemPriceDataOptions
            //        {
            //            UnitAmount = (long)(item.Item.Price*100),
            //            Currency = "usd",
            //            ProductData = new SessionLineItemPriceDataProductDataOptions
            //            {
            //                Name = item.Item.Title.ToString(),
            //            }
            //        },
            //        Quantity = item.Quantity
            //    };
            //    options.LineItems.Add(sessionListItem);
            //}

            //var service = new SessionService();
            //Session session = service.Create(options);
            //Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);
            return View(Cart);
        }
        public IActionResult PaymentGateWay()
        {
            var cart = HttpContext.Session.GetString("Cart");
            List<CartItemViewModel> Cart = cart != null ? JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart) : new List<CartItemViewModel>();
            var domain = "https://localhost:7080/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Customer/Carts/OrderConfirmation",
                CancelUrl = domain + $"Homes/Home",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };
            foreach (var item in Cart)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Item.Title.ToString(),
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionListItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        [HttpPost]
        [Authorize]
        public IActionResult ConfirmOrder()
        {
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult DownloadBillingDetails()
        {
            // Implement logic to get billing details (e.g., from session or database)
            var billingDetails = GetBillingDetailsFromSessionOrDatabase();

            // Generate the billing details content (e.g., in CSV format)
            string billingContent = GenerateBillingContent(billingDetails);

            // Convert the content to bytes
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(billingContent);

            // Return the bytes as a downloadable file
            return File(bytes, "text/plain", "billing_details.txt");
        }
        private string GenerateBillingContent(List<CartItemViewModel> cartItems)
        {
            StringBuilder csvContent = new StringBuilder();

            // Header row
            csvContent.AppendLine("Item Name, Price, Quantity, Total");

            // Rows for each item in the cart
            foreach (var item in cartItems)
            {
                double total = item.Item.Price * item.Quantity;
                csvContent.AppendLine($"{item.Item.Title}, {item.Item.Price}, {item.Quantity}, {total}");
            }

            return csvContent.ToString();
        }
        private List<CartItemViewModel> GetBillingDetailsFromSessionOrDatabase()
        {
            // Check if billing details are stored in session
            var cart = HttpContext.Session.GetString("Cart");
            if (cart != null)
            {
                // Deserialize the cart from session
                return JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart);
            }
            else
            {
                // If billing details are not found in session, you might fetch them from the database or another source
                // For example:
                // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // var userCartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
                // return userCartItems;

                // Since you're directly downloading from the session, you can return an empty list or handle it as appropriate
                return new List<CartItemViewModel>();
            }
        }
    }
}
