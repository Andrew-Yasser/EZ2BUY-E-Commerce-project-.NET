using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Provides validation control attributes like ValidateNever
using System.ComponentModel.DataAnnotations; // Provides validation attributes like Required, Range, Key
using System.ComponentModel; // Provides UI-related attributes like DisplayName
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // Provides database schema-related attributes like ForeignKey

namespace Ez2Buy.DataAccess.Models
{
    // Product class represents merchandise items available for sale in the e-commerce application
    public class Product
    {
        [Key] // Identifies this property as the primary key in the database
        public int Id { get; set; } // Unique identifier for each product
        
        [Required] // Makes this field mandatory
        [DisplayName("Product Name")] // Sets the display label in UI forms and views
        public string Name { get; set; } // Stores the name of the product
        
        public string? Description { get; set; } // Optional detailed description of the product
        
        [Required] // Makes this field mandatory
        [DisplayName("List Price")] // Sets the display label in UI forms and views
        [Range(1, 200000, ErrorMessage = "Price must be between 1 and 200000.")] // Restricts values with custom error message
        public decimal ListPrice { get; set; }  //original price
        
        [Required] // Makes this field mandatory
        [Range(1, 200000, ErrorMessage = "Price must be between 1 and 200000.")] // Restricts values with custom error message
        public decimal Price { get; set; } // selling price (may differ from ListPrice for discounts, sales, etc.)
        
        [ValidateNever] // Excludes this property from model validation
        public string? ImageUrl { get; set; } // Path or URL to the product image
        
        public int CategoryId { get; set; } // Foreign key to identify the category this product belongs to
        
        [ForeignKey("CategoryId")] // Specifies the foreign key relationship with Category
        [ValidateNever] // Excludes this navigation property from model validation
        public Category? Category { get; set; } // Navigation property to access the related category data
    }
}