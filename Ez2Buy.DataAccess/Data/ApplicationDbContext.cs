using Ez2Buy.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ez2Buy.DataAccess.Data
{

    /// Represents the database context for the Ez2Buy application.
    /// Extends IdentityDbContext to include authentication and authorization capabilities.
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {

        /// Initializes a new instance of the ApplicationDbContext class.
        /// param name="options">The options to be used by the DbContext.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        /// Gets or sets the Categories DbSet for accessing and managing Category entities.
        public DbSet<Category> Categories { get; set; }
        

        /// Gets or sets the Products DbSet for accessing and managing Product entities.
        public DbSet<Product> Products { get; set; }
        

        /// Gets or sets the ShoppingCarts DbSet for accessing and managing ShoppingCart entities.
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        

        /// Gets or sets the AppUsers DbSet for accessing and managing AppUser entities.
        public DbSet<AppUser> AppUsers { get; set; }
        

        /// Gets or sets the OrderHeaders DbSet for accessing and managing OrderHeader entities.
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        

        /// Gets or sets the OrderDetails DbSet for accessing and managing OrderDetail entities.
        public DbSet<OrderDetail> OrderDetails { get; set; }


        /// Configures the model that was discovered by convention from the entity types
        /// and seeds the database with initial data for Categories and Products.
        /// param name="modelBuilder">The builder being used to construct the model for this context.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base implementation to set up Identity tables
            base.OnModelCreating(modelBuilder);

            // Seed initial Category data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Fashion", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Home & Kitchen", DisplayOrder = 3 }
            );
            
            // Seed initial Product data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 14 Pro",
                    Description = "The latest iPhone model with cutting-edge technology, featuring a powerful A15 Bionic chip, a stunning 6.1-inch Super Retina XDR display, and a pro camera system.",
                    ListPrice = 1199.99m,
                    Price = 1099.99m,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S23",
                    Description = "The Samsung Galaxy S23 offers a sleek design with powerful performance, featuring a 6.1-inch AMOLED display, and the latest Snapdragon chipset for speed and efficiency.",
                    ListPrice = 999.99m,
                    Price = 899.99m,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Name = "Adidas Ultraboost 22",
                    Description = "Adidas Ultraboost 22 running shoes combine exceptional comfort with innovative design, featuring responsive Boost cushioning and a supportive Primeknit upper for a snug fit.",
                    ListPrice = 180.00m,
                    Price = 159.99m,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Name = "Basic Cotton T-shirt",
                    Description = "A soft and breathable cotton t-shirt that is perfect for everyday casual wear. Available in a variety of colors and fits for all sizes.",
                    ListPrice = 20.00m,
                    Price = 15.00m,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 5,
                    Name = "Electric Kettle",
                    Description = "A fast-heating electric kettle with an automatic shut-off feature for safety, ideal for boiling water quickly for tea, coffee, and other beverages.",
                    ListPrice = 24.99m,
                    Price = 19.99m,
                    CategoryId = 3,
                    ImageUrl = ""
                }
            );
        }
    }
}