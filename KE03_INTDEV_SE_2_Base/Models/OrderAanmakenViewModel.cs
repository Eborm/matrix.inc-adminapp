using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class ProductAantal
    {
        public int ProductId { get; set; }
        public string ProductNaam { get; set; } = string.Empty;
        public int Aantal { get; set; }
    }

    public class OrderAanmakenViewModel
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public List<ProductAantal> Producten { get; set; } = new List<ProductAantal>();
    }
} 