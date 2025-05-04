using System.ComponentModel; // Provides attributes for UI display like DisplayName
using System.ComponentModel.DataAnnotations; // Provides validation attributes like Required, Key, Range

namespace Ez2Buy.DataAccess.Models
{
    // Category class to represent product categories in the e-commerce application
    public class Category
    {
        [Key] // Identifies this property as the primary key in the database
        public int Id { get; set; } // Unique identifier for each category
        
        [Required] // Makes this field mandatory
        [DisplayName("Category Name")] // Sets the display label in UI forms and views
        public string Name { get; set; } // Stores the name of the category
        
        [DisplayName("Display Order")] // Sets the display label in UI forms and views
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100.")] // Restricts values between 1-100 with custom error message
        public int DisplayOrder { get; set; } // Controls the order in which categories are displayed
    }
}