using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineFoodDelivery.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Image {  get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public SubCategory SubCategory { get; set; }

    }
}
