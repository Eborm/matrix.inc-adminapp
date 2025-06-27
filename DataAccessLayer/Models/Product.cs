using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Product
    {        
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        public bool IsDiscountActive =>
            Discount > 0 &&
            DiscountStartDate.HasValue &&
            DiscountEndDate.HasValue &&
            DiscountStartDate.Value.Date <= DateTime.Now.Date &&
            DiscountEndDate.Value.Date >= DateTime.Now.Date;

        public decimal DiscountedPrice =>
            IsDiscountActive ? Math.Round(Price * (1 - Discount / 100), 2) : Price;

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Part> Parts { get; } = new List<Part>();

        public ICollection<OrderRegel> OrderRegels { get; set; } = new List<OrderRegel>();
    }
}
