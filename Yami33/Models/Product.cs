using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yami33.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(0.01, 1000)]
        public decimal Price { get; set; }
        public string? Description { get; set; }     
        public string? ImageUrl { get; set; }
    }
}
