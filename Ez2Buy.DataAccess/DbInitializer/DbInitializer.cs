using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// this method is used to seed the database with initial data (create the database and apply the migrations)
        public void Initialize()
        {


            //migrations if there are not applied
            try
            {
                //check if the database exists
                if (_db.Database.GetPendingMigrations().Count() > 0) 
                {
                    _db.Database.Migrate();   //apply the migrations
                }

            }catch (Exception ex)
            {
            }

            //create roles if they do not exist
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                //create admin user if Roles are not created
                _userManager.CreateAsync(new AppUser
                {
                    UserName = "admin@ez2buy.com",
                    Email = "admin@ez2buy.com",
                    Name = "Admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 Admin Street",
                    City = "Maadi",
                    Governorate = "Cairo",

                }, "123@Admin").GetAwaiter().GetResult();

                //retrive the user object we created above from db
                AppUser user = _db.AppUsers.FirstOrDefault(u => u.Email == "admin@ez2buy.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;  //if roles are already created then return back to application
        }
    }
}
