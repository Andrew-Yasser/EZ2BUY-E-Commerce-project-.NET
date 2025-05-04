using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{

    /// Controller responsible for managing product categories in the admin area.
    /// Provides CRUD operations for product categories.
    /// Access is restricted to administrators and employees.
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {

        /// Repository unit of work for database operations
        private readonly IUnitOfWork _unitOfWork;
        
        /// Initializes a new instance of the CategoryController
        /// param name="unitOfWork">The unit of work providing access to repositories
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// Displays a list of all product categories
        /// returns>The Index view with a list of all categories
        public IActionResult Index()
        {
            // Retrieve all categories from the database
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);  // Pass all categories to the view
        }


        /// Displays the category creation form
        /// returns>The Create view for adding a new category
        public IActionResult Create()
        {
            return View();
        }


        /// Processes the category creation form submission
        /// param name="obj">The category object to create
        /// returns>Redirects to Index on success, or returns to the form with errors
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            // Custom validation: Ensure category name and display order are not identical
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Category Name and Display Order can't be same");
            }
            
            if (ModelState.IsValid)
            {
                // Add the category to the database
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                
                // Set success message for display on the next page
                TempData["success"] = "Category Created successfully";
                
                return RedirectToAction("Index");
            }
            else
            {
                // Return to the view with validation errors
                return View();
            }
        }


        /// Displays the category edit form for a specific category
        /// param name="id">The ID of the category to edit
        /// returns>The Edit view with the category data, or NotFound if invalid ID
        public IActionResult Edit(int? id)
        {
            // Validate the category ID
            if (id == null || id == 0)
            {
                return NotFound();
            }
            
            // Get the category by ID
            Category? CategoryFromDb = _unitOfWork.Category.GetById(u => u.Id == id);
            
            // Return NotFound if category doesn't exist
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            
            return View(CategoryFromDb);
        }


        /// Processes the category edit form submission
        /// param name="obj">The category object with updated data
        /// returns>Redirects to Index on success, or returns to the form with errors
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                // Update the category in the database
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                
                // Set success message for display on the next page
                TempData["success"] = "Category Updated successfully";
                
                return RedirectToAction("Index");
            }
            else
            {
                // Return to the view with validation errors
                return View();
            }
        }


        /// Displays the category deletion confirmation page
        /// param name="id">The ID of the category to delete
        /// returns>The Delete view with the category data, or NotFound if invalid ID
        public IActionResult Delete(int? id)
        {
            // Validate the category ID
            if (id == null || id == 0)
            {
                return NotFound();
            }
            
            // Get the category by ID
            Category? CategoryFromDb = _unitOfWork.Category.GetById(u => u.Id == id);
            
            // Return NotFound if category doesn't exist
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            
            return View(CategoryFromDb);
        }


        /// Processes the category deletion confirmation
        /// param name="id">The ID of the category to delete
        /// returns>Redirects to Index after deletion, or NotFound if invalid ID
        [HttpPost, ActionName("Delete")]   // The ActionName attribute maps this method to the "Delete" action name
        public IActionResult DeletePost(int? id)  // Method name differs to avoid naming conflict
        {
            // Get the category by ID
            Category? obj = _unitOfWork.Category.GetById(u => u.Id == id);
            
            // Return NotFound if category doesn't exist
            if (obj == null)
            {
                return NotFound();
            }
            
            // Delete the category from the database
            _unitOfWork.Category.Delete(obj);
            _unitOfWork.Save();
            
            // Set success message for display on the next page
            TempData["success"] = "Category Deleted successfully";
            
            return RedirectToAction("Index");
        }
    }
}