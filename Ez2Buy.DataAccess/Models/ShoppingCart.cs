using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Provides validation control attributes like ValidateNever
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Provides validation attributes like Range
using System.ComponentModel.DataAnnotations.Schema; // Provides database schema-related attributes like ForeignKey and NotMapped
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Models
{
    // ShoppingCart class represents items a user has added to their cart before checkout
    public class ShoppingCart
    {
        // Primary key for the ShoppingCart entity
        public int Id { get; set; }

        // Number of units of the product in the cart, restricted to values between 1 and 1000
        [Range(1,1000, ErrorMessage ="Please enter a Value between 1 and 1000")]
        public int Quantity { get; set; }

        // Foreign key to identify which product is in the cart
        public int ProductId { get; set; }
        
        [ForeignKey("ProductId")] // Specifies the foreign key relationship with Product
        [ValidateNever] // Excludes this navigation property from model validation
        public Product Product { get; set; } // Navigation property to access the related product data
        
        // Foreign key to identify which user owns this cart item
        public string AppUserId { get; set; }
        
        [ForeignKey("AppUserId")] // Specifies the foreign key relationship with AppUser
        [ValidateNever] // Excludes this navigation property from model validation
        public AppUser AppUser { get; set; } // Navigation property to access the related user data

        [NotMapped] // Indicates this property is not stored in the database
        public decimal Price { get; set; } //price of the product in the cart - calculated at runtime
    }
}