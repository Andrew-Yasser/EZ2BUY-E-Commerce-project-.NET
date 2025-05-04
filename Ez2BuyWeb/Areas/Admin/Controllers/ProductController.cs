using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Services.Contracts;
using Ez2Buy.Services.Services;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{

    /// Controller responsible for product management in the admin area.
    /// Handles operations such as creating, editing, deleting products and managing product images.
    /// Access is restricted to administrators and employees.
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {

        /// Repository unit of work for database operations
        private readonly IUnitOfWork _unitOfWork;
        

        /// Web host environment to access file system paths for image storage
        private readonly IWebHostEnvironment _WebhostEnvironment;
        

        /// Initializes a new instance of the ProductController
        /// param name="unitOfWork">The unit of work providing access to repositories
        /// param name="webHostEnvironment">The web host environment for file operations
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _WebhostEnvironment = webHostEnvironment;
        }
        

        /// Displays a list of all products
        /// <returns>The Index view with a list of products
        public IActionResult Index()
        {
            // Get all products from database, including their associated categories
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);  // Passing all products to view
        }


        /// Displays the product creation form
        /// returns>The Create view with an empty product form and category dropdown
        public IActionResult Create()
        {
            // Create a view model with category dropdown data
            ProductVM productVM = new()
            {
                // Create dropdown list of categories
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Product = new Product() // Initialize empty product
            };

            return View(productVM);
        }


        /// Processes the product creation form submission
        /// param name="productVM">The product view model containing the new product data
        /// param name="file">Optional image file for the product
        /// returns>Redirects to Index on success, or returns to the form with errors
        [HttpPost]
        public IActionResult Create(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload if a file was submitted
                if (file != null)
                {
                    // Get path to wwwroot folder
                    string wwwRootPath = _WebhostEnvironment.WebRootPath;
                    
                    // Generate a random filename to prevent duplicates
                    string fileName = Guid.NewGuid().ToString();
                    
                    // Build the path to the product images folder
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    
                    // Get the file extension (e.g., .png, .jpg)
                    var extension = Path.GetExtension(file.FileName);
                    
                    // Create the file on disk
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    // Save the relative URL path to the product model
                    productVM.Product.ImageUrl = @"\images\product\" + fileName + extension;
                }

                // Add the product to the database
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                
                // Set success message
                TempData["success"] = "Product created successfully";
                
                return RedirectToAction("Index");
            }

            // If model state is invalid, repopulate the category dropdown
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            // Return to the view with validation errors
            return View(productVM);
        }


        /// Displays the product edit form for a specific product
        /// param name="id">The ID of the product to edit
        /// returns>The Edit view with the product data and category dropdown
        public IActionResult Edit(int id)
        {
            // Get the product by ID
            var product = _unitOfWork.Product.GetById(p => p.Id == id);
            if (product == null) return NotFound();

            // Create a view model with product data and category dropdown
            ProductVM productVM = new()
            {
                Product = product,
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            return View(productVM);
        }


        /// Processes the product edit form submission
        /// param name="productVM">The product view model containing the updated product data
        /// param name="file">Optional new image file for the product
        /// returns>Redirects to Index on success, or returns to the form with errors
        [HttpPost]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload if a file was submitted
                if (file != null)
                {
                    // Get path to wwwroot folder
                    string wwwRootPath = _WebhostEnvironment.WebRootPath;
                    
                    // Generate a random filename to prevent duplicates
                    string fileName = Guid.NewGuid().ToString();
                    
                    // Build the path to the product images folder
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    
                    // Get the file extension (e.g., .png, .jpg)
                    var extension = Path.GetExtension(file.FileName);

                    // Delete the old image file if it exists
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // Get the file path of the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        // Delete the old image if it exists
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Create the new file on disk
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    
                    // Save the relative URL path to the product model
                    productVM.Product.ImageUrl = @"\images\product\" + fileName + extension;
                }

                // Update the product in the database
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                
                // Set success message
                TempData["success"] = "Product updated successfully";
                
                return RedirectToAction("Index");
            }

            // If model state is invalid, repopulate the category dropdown
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            // Return to the view with validation errors
            return View(productVM);
        }

        /* Note: The following commented-out Upsert method would combine Create and Edit into a single action
        that determines whether to create or update based on the presence of an ID. This approach is shown
        as an alternative to having separate Create and Edit actions. */

        //------------------------------------------
        // API CALLS
        //------------------------------------------
        #region API CALLS


        /// API endpoint to retrieve all products
        /// returns>JSON data containing all products with their categories
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        /// <summary>
        /// API endpoint to delete a product
        /// </summary>
        /// <param name="id">The ID of the product to delete
        /// <returns>JSON result indicating success or failure of the deletion
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // Get the product to be deleted
            var productToBeDeleted = _unitOfWork.Product.GetById(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete the product image if it exists
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(_WebhostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Delete the product from the database
            _unitOfWork.Product.Delete(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted successfully" });
        }

        #endregion
    }
}