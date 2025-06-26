using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class OrderRegel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }
    }
} 