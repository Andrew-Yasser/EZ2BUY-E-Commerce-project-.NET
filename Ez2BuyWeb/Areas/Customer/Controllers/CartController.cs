using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ez2BuyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        [BindProperty]  //bind the properties of the model to the view
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            //retrive userid then the shopping cart of user from the database
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                if (cart.Product != null)
                {
                    cart.Price = cart.Product.Price;
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Quantity);
                }
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            //retrive the shopping cart of user from the database
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUser.Get(u => u.Id == userId); //populate the AppUser in OrderHeader

            //populate the OrderHeader with the AppUser details
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.AppUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.AppUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.AppUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.AppUser.City;
            ShoppingCartVM.OrderHeader.Governorate = ShoppingCartVM.OrderHeader.AppUser.Governorate;


            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                if (cart.Product != null)
                {
                    cart.Price = cart.Product.Price;
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Quantity);
                }
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()         //triggered when user clicks "Place Order" button.
        {
            //retrive the shopping cart of user from the database
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM.shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.AppUserId = userId;      //set the AppUserId in OrderHeader to the current user id


            AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == userId); //populate the AppUser in OrderHeader

            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                if (cart.Product != null)
                {
                    cart.Price = cart.Product.Price;
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Quantity);
                }
            }
            //create the OrderHeader in the database
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            //create the OrderDetail in the database
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Quantity = cart.Quantity
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";  //This line builds the base URL of your website dynamically
                                                                             //Example result: https://Ez2Buy.com/
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",  //redirect to the order confirmation page after payment success
                CancelUrl = domain + "customer/cart/index",                                                   //redirect to the cart page after payment cancel
                LineItems = new List<SessionLineItemOptions>(),                                               //The actual items the customer is buying
                Mode = "payment",                                                                             //payment tells Stripe this is a one-time payment
            };

            foreach (var item in ShoppingCartVM.shoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name                            //product Name shown in the Stripe UI
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();        //This creates the Stripe session using the options defined above
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId); //update the session id and payment intent id in the order header bec we need them to verify the payment status
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url); //redirect to the stripe checkout page
            return new StatusCodeResult(303); //303 is the status code for redirect

        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "AppUser");

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId); //get the session from stripe using the session id
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            var orderDetails = _unitOfWork.OrderDetail
                .GetAll(u => u.OrderHeaderId == id, includeProperties: "Product")
                .Select(od => new ShoppingCart
                {
                    Product = od.Product,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList();

            ShoppingCartVM = new()
            {
                OrderHeader = orderHeader,
                shoppingCartList = orderDetails
            };
            HttpContext.Session.Clear();
            //send email to the user after successful payment
            _emailSender.SendEmailAsync(
                orderHeader.AppUser.Email,
            "Thank You for Your Order - Ez2Buy",
            $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        background-color: #007bff;
                        color: #ffffff;
                        padding: 20px;
                        text-align: center;
                        border-radius: 8px 8px 0 0;
                    }}
                    .content {{
                        padding: 20px;
                        line-height: 1.6;
                        color: #333333;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 10px 20px;
                        margin: 20px 0;
                        background-color: #007bff;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                    }}
                    .footer {{
                        text-align: center;
                        padding: 20px;
                        font-size: 12px;
                        color: #777777;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Thank You for Your Order, {orderHeader.AppUser.Name}!</h1>
                    </div>
                    <div class='content'>
                        <p>We’re thrilled to let you know that your order has been successfully placed with Ez2Buy!</p>
                        <p><strong>Order ID:</strong> {orderHeader.Id}</p>
                        <p>You can track your order or view details anytime by visiting your account.</p>
                        <p>If you have any questions, feel free to contact our support team at <a href='mailto:support@ez2buy.tech'>support@ez2buy.tech</a>.</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; {DateTime.Now.Year} Ez2Buy. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>
    ");
            //_emailSender.SendEmailAsync(orderHeader.AppUser.Email, "New Order - Ez2Buy",
            //    $"<h1>Thank You for Your Order, {orderHeader.AppUser.Name}!</h1>" +
            //    $"<p>New Order Created. Order ID: {orderHeader.Id}</p>");
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.AppUserId == orderHeader.AppUserId).ToList();
            _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
            _unitOfWork.Save();
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Quantity += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            if (cartFromDb.Quantity <= 1)
            {
                //remove from Cart
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                   .GetAll(u => u.AppUserId == cartFromDb.AppUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Delete(cartFromDb);
            }
            else
            {
                cartFromDb.Quantity -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            if (cartFromDb != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.AppUserId == cartFromDb.AppUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Delete(cartFromDb);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
