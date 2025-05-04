using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Repositories
{

    /// Repository implementation for managing OrderHeader entities.
    /// Extends the generic RepositoryBase to provide specific functionality
    /// for OrderHeader data operations, particularly those related to payment processing and status updates.
    public class OrderHeaderRepository : RepositoryBase<OrderHeader>, IOrderHeaderRepository
    {

        /// Reference to the application database context
        private ApplicationDbContext _db;
        

        /// Initializes a new instance of the OrderHeaderRepository class
        /// param name="db">The database context to be used for data operations</param>
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        /// Updates an existing OrderHeader entity in the database.
        /// This implementation uses the EF Core Change Tracker to update all properties.
        /// param name="obj">The OrderHeader entity with updated values</param>
        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }


        /// Updates the order status and optionally the payment status of an order.
        /// param name="id">The ID of the order to update</param>
        /// param name="orderStatus">The new order status to set</param>
        /// param name="paymentStatus">Optional new payment status to set, if provided</param>
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            // Retrieve order header from database based on id
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            
            if (orderFromDb != null)
            {
                // Update the order status
                orderFromDb.OrderStatus = orderStatus;
                
                // Update the payment status only if a value is provided
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }


        /// Updates the Stripe payment information for an order.
        /// This method is used to track payment processing through Stripe's payment gateway.
        /// param name="id">The ID of the order to update</param>
        /// param name="sessionId">The Stripe session ID associated with the payment</param>
        /// param name="paymentIntentId">The Stripe payment intent ID that confirms payment completion</param>
        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            
            // Update the session ID if provided
            if(!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            
            // Update payment intent ID and payment date if provided
            // The payment intent ID confirms that payment has been successfully processed
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now; // Record the payment timestamp
            }
        }
    }
}