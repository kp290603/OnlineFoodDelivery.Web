using OnlineFoodDelivery.Models;

namespace OnlineFoodDelivery.Web.ViewModels
{
    public class BillingViewModel
    {
        public List<CartItemViewModel> Cart { get; set; }
        public double TotalAmount { get; set; }
        public ApplicationUser User { get; set; }
    }
}
