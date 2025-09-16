using System.ComponentModel.DataAnnotations;

namespace SmallEcommerece.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string ProductCode { get; set; } 

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int MinimumQuantity { get; set; }

        public double DiscountRate { get; set; }

        public string ImagePath { get; set; }   
    }
}
