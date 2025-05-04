using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Provides validation related attributes like ValidateNever
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Provides validation attributes like Required
using System.ComponentModel.DataAnnotations.Schema; // Provides database schema-related attributes like ForeignKey
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Models
{
    //contain details about items in the order like : product id, quantity, price, etc. 
    public class OrderDetail
    {
        // Primary key for the OrderDetail entity
        public int Id { get; set; }
        
        [Required] // Makes this field mandatory
        public int OrderHeaderId { get; set; } //foreign key to the OrderHeader
        
        [ForeignKey("OrderHeaderId")] // Specifies the foreign key relationship with OrderHeader
        [ValidateNever] // Excludes this navigation property from model validation
        public OrderHeader OrderHeader { get; set; } // Navigation property to access the related OrderHeader
        
        [Required] // Makes this field mandatory
        public int ProductId { get; set; }  //product that is being ordered
        
        [ForeignKey("ProductId")] // Specifies the foreign key relationship with Product
        [ValidateNever] // Excludes this navigation property from model validation
        public Product Product { get; set; } // Navigation property to access the related Product
        
        // Stores the quantity of the product ordered
        public int Quantity { get; set; }
        
        // Stores the price of the product at the time of order
        public decimal Price { get; set; } //price of the product at the time of order won't change later with the changes of the product prices
    }
}