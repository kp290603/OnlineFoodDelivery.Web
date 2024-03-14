using OnlineFoodDelivery.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineFoodDelivery.Web.ViewModels
{
    public class CartItemViewModel
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
