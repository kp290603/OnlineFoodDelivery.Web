using System.Collections.Generic;
using OnlineFoodDelivery.Models;

namespace OnlineFoodDelivery.Web.ViewModels
{
    public class ItemCategoryViewModel
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
