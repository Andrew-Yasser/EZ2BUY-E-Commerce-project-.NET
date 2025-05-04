using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{

    /// Controller responsible for the administrative dashboard.
    /// Provides a summary view of key metrics for the Ez2Buy application.
    /// Access is restricted to administrators and employees.
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class DashboardController : Controller
    {

        /// Repository unit of work for database operations
        private readonly IUnitOfWork _unitOfWork;


        /// Initializes a new instance of the DashboardController
        /// param name="unitOfWork">The unit of work providing access to repositories
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// Displays the main dashboard with key business metrics including:
        /// - Total order count
        /// - Total product count
        /// - Total category count
        /// - Total user count
        /// - Total revenue from approved orders
        /// returns>The Index view with dashboard metrics
        public IActionResult Index()
        {
            // Create a view model with all dashboard metrics
            DashboardVM dashboardVM = new()
            {
                // Total number of orders in the system
                OrderCount = _unitOfWork.OrderHeader.GetAll().Count(),
                
                // Total number of products in the catalog
                ProductCount = _unitOfWork.Product.GetAll().Count(),
                
                // Total number of product categories
                CategoryCount = _unitOfWork.Category.GetAll().Count(),
                
                // Total number of registered users
                UserCount = _unitOfWork.AppUser.GetAll().Count(),
                
                // Total revenue from approved orders (orders with confirmed payment)
                Balance = (double)_unitOfWork.OrderHeader.GetAll()
                    .Where(i => i.PaymentStatus == SD.PaymentStatusApproved)
                    .Select(i => i.OrderTotal)
                    .Sum()
            };
            
            return View(dashboardVM);
        }
    }
}