using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Services.DTOs.Product
{
    public class CategorytInsertDto
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal ListPrice { get; set; }  //original price
        public decimal Price { get; set; } //selling price
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
