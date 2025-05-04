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
    //contains the order information like : where the order is being shipped to, 
    //payment status, the order date, payment id, tracking info,etc.
    public class OrderHeader
    {
        // Primary key for the OrderHeader entity
        public int Id { get; set; }
        
        // Foreign key to identify the user who placed the order
        public string AppUserId { get; set; }
        
        [ForeignKey("AppUserId")] // Specifies the foreign key relationship with AppUser
        [ValidateNever] // Excludes this navigation property from model validation
        public AppUser AppUser { get; set; } // Navigation property to access the related user data
        
        // Date when the order was created/placed
        public DateTime OrderDate { get; set; }
        
        // Date when the order was shipped to the customer
        public DateTime ShippingDate { get; set; }
        
        // Total amount of the order including all items
        public decimal OrderTotal { get; set; }
        
        // Current status of the order in the fulfillment process
        public string? OrderStatus { get; set; } //pending, approved, shipped, delivered, cancelled
        
        // Current status of the payment for this order
        public string? PaymentStatus { get; set; } //pending, approved, declined
        
        // Tracking number assigned by the shipping carrier
        public string? TrackingNumber { get; set; }
        
        // Name of the shipping company handling the delivery
        public string? Carrier { get; set; } //the shipping company that will ship the order
        
        // Date when the payment was processed
        public DateTime PaymentDate { get; set; }
        
        //data that we need to collect from the user when he is placing an order
        [Required] // Makes this field mandatory
        public string Name { get; set; } //name of the person who will receive the order
        
        [Required] // Makes this field mandatory
        public string PhoneNumber { get; set; } 
        
        [Required] // Makes this field mandatory
        public string StreetAddress { get; set; } 
        
        [Required] // Makes this field mandatory
        public string City { get; set; } 
        
        [Required] // Makes this field mandatory
        public string Governorate { get; set; } 
        
        //for stripe payment
        // Unique session identifier from Stripe's checkout process
        public string? SessionId { get; set; }        //using stripe checkout session to charge the customer
        
        // Unique payment intent identifier from Stripe
        public string? PaymentIntentId { get; set; } //means that Stripe creates generate id for the payment 
                                                     //when we about to charge a customer, we create a Payment Intent first by stripe.
                                                     //useful for:Confirm payment status(pending, succeeded, failed)
                                                     //Track the payment related to a specific order or user.
    }
}