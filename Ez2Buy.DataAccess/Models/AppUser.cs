using Microsoft.AspNetCore.Identity; // Provides the IdentityUser class for user management
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Provides validation attributes like [Required]
using System.ComponentModel.DataAnnotations.Schema; // Provides database schema-related attributes like [NotMapped]
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Models
{
    // AppUser class extends IdentityUser to add custom properties for the e-commerce application
    public class AppUser : IdentityUser
    {
        [Required] // Indicates this property must have a value
        public string Name { get; set; } // User's full name
        
        public string? StreetAddress { get; set; } // Optional property for user's street address
        public string? City { get; set; } // Optional property for user's city
        public string? Governorate { get; set; } // Optional property for user's governorate/state/province
        
        [NotMapped] // Indicates this property will not be mapped to a database column
        public string Role { get; set; } // Temporary property to store user role information in memory only
    }
}