using OnlineFoodDelivery.Models;

namespace OnlineFoodDelivery.Web.ViewModels
{
    public class CartOrderViewModel
    {
        public List<Cart> ListofCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
