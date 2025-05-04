using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Services.Contracts;
using Ez2Buy.Services.Services;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{

    /// Controller responsible for user management in the admin area.
    /// Provides functionality for viewing users, managing roles, and locking/unlocking accounts.
    /// Access to this controller is restricted to administrators only.

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {

        /// Database context for direct database access to tables such as roles
        private readonly ApplicationDbContext _db;
        

        /// Identity user manager for user-related operations like role assignments
        private readonly UserManager<IdentityUser> _userManager;
        

        /// Initializes a new instance of the UserController class
        /// param name="db">The application database context</param>
        /// param name="userManager">The Identity user manager</param>
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        

        /// Displays the main user management page
        /// returns>The Index view
        public IActionResult Index() 
        {
            return View();
        }


        /// Displays the role management page for a specific user
        /// param name="userId">The ID of the user whose role is being managed
        /// returnsThe RoleManagement view with user and role information
        public IActionResult RoleManagement(string userId)
        {
            // Get the role ID of the user
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            
            // Prepare the view model with user information and available roles
            RoleManagementVM RoleVM = new()
            {
                // Get the user by ID
                AppUser = _db.AppUsers.FirstOrDefault(u => u.Id == userId),
                
                // Create a dropdown list of all available roles
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                })
            };
            
            // Set the current role of the user
            RoleVM.AppUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
            
            return View(RoleVM);
        }

        /// Processes the role change request for a user

        /// param name="roleManagementVM">The view model containing updated role information
        ///Redirects to the Index action after processing
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            // Get the current role ID of the user
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.AppUser.Id).RoleId;
            
            // Get the name of the current role
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
            
            // Check if the role has been changed
            if (!(roleManagementVM.AppUser.Role == oldRole))
            {
                // Role was updated - get the user from the database
                AppUser appUser = _db.AppUsers.FirstOrDefault(u => u.Id == roleManagementVM.AppUser.Id);
                _db.SaveChanges();
                
                // Remove the user from the old role
                _userManager.RemoveFromRoleAsync(appUser, oldRole).GetAwaiter().GetResult();
                
                // Add the user to the new role
                _userManager.AddToRoleAsync(appUser, roleManagementVM.AppUser.Role).GetAwaiter().GetResult();
            }
            
            return RedirectToAction("Index");
        }

        //------------------------------------------
        // API CALLS
        //------------------------------------------
        #region API CALLS


        /// API endpoint to retrieve all users with their assigned roles
        ///JSON data containing all users and their roles
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get all users from the database
            List<AppUser> objUserList = _db.AppUsers.ToList();
            
            // Get all user role mappings
            var userRoles = _db.UserRoles.ToList();
            
            // Get all available roles
            var roles = _db.Roles.ToList();
            
            // Enrich each user with their role name
            foreach (var user in objUserList)
            {
                // Get the role ID for the current user
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                
                // Assign the role name to the user object
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            
            return Json(new { data = objUserList });
        }


        /// API endpoint to lock or unlock a user account
        /// param name="id">The ID of the user to lock/unlock
        /// JSON result indicating success or failure of the operation
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id) // FromBody specifies that parameter is coming from the body of the request (AJAX call)
        {
            // Get the user by ID
            var objFromDb = _db.AppUsers.FirstOrDefault(u => u.Id == id);
            
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            
            string message = "";
            
            // Check if the user is currently locked
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // User is locked - unlock them
                objFromDb.LockoutEnd = DateTime.Now; // Set lockout end to now (unlocking the user)
                message = "User account unlocked successfully";
            }
            else
            {
                // User is not locked - lock them
                objFromDb.LockoutEnd = DateTime.Now.AddYears(500); // Lock the user for 500 years (effectively permanent)
                message = "User account locked successfully";
            }
            
            _db.SaveChanges();
            return Json(new { success = true, message = message });
        }

        #endregion
    }
}