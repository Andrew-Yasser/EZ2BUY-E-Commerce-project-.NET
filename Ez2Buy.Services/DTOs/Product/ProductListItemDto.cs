using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Services.DTOs.Product
{
    public class ProductListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; } //selling price
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
