// Importing the OrderHeader model from the Ez2Buy.DataAccess.Models namespace
using Ez2Buy.DataAccess.Models;

// Importing basic system libraries for general programming functionalities
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Defining the namespace for data access contract interfaces
namespace Ez2Buy.DataAccess.Contracts
{
	// Defining an interface for order header repository, inheriting from a generic repository base for OrderHeader
	public interface IOrderHeaderRepository : IRepositoryBase<OrderHeader>
	{
		// Method to update an existing OrderHeader object in the database
		void Update(OrderHeader obj);

		// Method to update the status of an order based on its ID
		// orderStatus is mandatory, paymentStatus is optional (can be null)
		// This allows changing order status independently from payment status
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null); 

		// Method to update Stripe-specific payment information (Session ID and Payment Intent ID) for an order
		// Needed because after creating a payment session and intent, we must store these identifiers in the order record
		void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
	}
}
