using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{

    /// Controller responsible for managing orders in the admin area.
    /// Handles order processing, status updates, payments, and cancellations.
    [Area("Admin")]
    public class OrderController : Controller
    {

        /// Repository unit of work for database operations
        private readonly IUnitOfWork _unitOfWork;
        

        /// The Order view model bound to the view for automatic data binding.
        /// This enables automatic mapping of form data to the model.
        [BindProperty]  // Binds the properties of the model to the view, automating processing of data from view to controller and vice versa
        public OrderVM OrderVM { get; set; }
        

        /// Initializes a new instance of the OrderController
        /// param name="unitOfWork">The unit of work providing access to repositories
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// Displays the main order management page
        /// returns>The Index view
        public IActionResult Index()
        {
            return View();
        }


        /// Displays the details of a specific order
        /// param name="orderId">The ID of the order to display
        /// returns>The Details view with the order information
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                // Retrieve the order header with the associated user information
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "AppUser"),
                // Retrieve all order details with product information
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }


        /// Updates the order details for an existing order
        /// returns>Redirects to the Details action with the updated order
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            // Get the order header from the database
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            
            // Update the order header with the new values
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.Governorate = OrderVM.OrderHeader.Governorate;
            
            // Only update carrier if provided
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            
            // Only update tracking number if provided
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            // Update the order in the database
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            
            // Add success message to TempData to be displayed on the next page
            TempData["success"] = "Order Details Updated Successfully";
            
            // Redirect to the details page of the order, passing the order ID
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }


        /// Changes the order status to "In Process"
        /// returns>Redirects to the Details action with the updated order
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            // Update the order status to "In Process"
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            
            TempData["success"] = "Order Status Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        /// Changes the order status to "Shipped" and updates shipping information
        ///returns>Redirects to the Details action with the updated order
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            // Get the order header from the database
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            
            // Update shipping details
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        /// Marks an order as delivered if it was previously in the shipped status
        /// returns>Redirects to the Details action with the updated order
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult MarkAsDelivered()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            
            // Only mark as delivered if the order was shipped
            if (orderHeader.OrderStatus == SD.StatusShipped)
            {
                _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusDelivered);
                _unitOfWork.Save();
                TempData["success"] = "Order Delivered successfully!";
            }
            else
            {
                TempData["error"] = "Cannot mark this order as Delivered. It must be in Shipped status.";
            }
            
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        /// Cancels an order and issues a refund if payment was already processed.
        /// This action is for admin and employee use.
        /// returns>Redirects to the Details action with the updated order
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            // Get the order header from the database
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            
            // If payment was approved, issue a refund using Stripe
            if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                // Configure the refund request
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                // Create and process the refund
                var service = new RefundService();
                Refund refund = service.Create(options);

                // Update both order status and payment status
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                // No payment to refund, just mark as cancelled
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        /// Cancels an order by a customer if payment hasn't been made yet.
        /// This action is for customer use only.
        /// returns>Redirects to the Details action with the updated order
        [HttpPost]
        [Authorize(Roles = SD.Role_Customer)]
        public IActionResult CancelOrderUser()
        {
            // Get the order header from the database
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            
            // Only allow cancellation if the order is pending and payment is pending
            if (orderHeader.OrderStatus == SD.StatusPending && orderHeader.PaymentStatus == SD.PaymentStatusPending)
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                _unitOfWork.Save();
                TempData["success"] = "Order Cancelled Successfully";
            }
            else
            {
                TempData["error"] = "Cannot cancel this order. Please contact technical support.";
            }
            
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        /// Processes payment for an order using Stripe Checkout.
        /// This is triggered when a customer pays for an order from the details page.
        /// returns>Redirects to Stripe Checkout
        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPayNow()
        {
            // Load the order with full details
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "AppUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            // Set up Stripe Checkout session
            // Build the domain URL dynamically (e.g., https://Ez2Buy.com/)
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            
            var options = new SessionCreateOptions
            {
                // Redirect URLs for success and cancel scenarios
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment", // One-time payment
            };

            // Add each order item to the Stripe session
            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // Convert to cents (e.g., $20.50 => 2050)
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name // Product name shown in Stripe UI
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }
            
            // Create the Stripe session
            var service = new SessionService();
            Session session = service.Create(options);
            
            // Update the order with Stripe session information
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            
            // Redirect to the Stripe checkout page
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); // 303 is the status code for redirect
        }


        /// Handles the return from Stripe after successful payment.
        /// Verifies payment status and updates the order accordingly.
        /// param name="orderHeaderId">The ID of the order that was paid
        /// returns>The PaymentConfirmation view
        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            
            // Only process if payment was pending
            if(orderHeader.PaymentStatus == SD.PaymentStatusPending)
            {
                // Verify payment with Stripe
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    // Update order with payment confirmation
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            return View(orderHeaderId);
        }

        //------------------------------------------
        // API CALLS
        //------------------------------------------
        #region API CALLS

        /// <summary>
        /// API endpoint to retrieve orders based on their status.
        /// Filters results by user role and status parameter.
        /// </summary>
        /// <param name="status">The status filter to apply (e.g., "pending", "approved", "shipped")
        /// <returns>JSON data containing the filtered orders
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;

            // Access control: Admin and employees can see all orders
            // Regular users can only see their own orders
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            }
            else {
                // Get user ID from claims
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Filter to show only the logged-in user's orders
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.AppUserId == userId, includeProperties: "AppUser");
            }

            // Apply status filtering
            switch (status)
            {
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusApproved && 
                                                               u.OrderStatus != SD.StatusInProcess && 
                                                               u.OrderStatus != SD.StatusShipped && 
                                                               u.OrderStatus != SD.StatusDelivered);
                    break;
                case "shipped":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped && 
                                                               u.OrderStatus != SD.StatusCancelled && 
                                                               u.OrderStatus != SD.StatusDelivered);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusDelivered);
                    break;
                case "cancelled":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusCancelled);
                    break;
                default:
                    break;
            }

            // Return the filtered data as JSON
            return Json(new { data = objOrderHeaders });
        }

        #endregion
    }
}