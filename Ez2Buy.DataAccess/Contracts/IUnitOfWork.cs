using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
    // Interface defining the Unit of Work pattern for managing multiple repositories and database operations
    public interface IUnitOfWork
    {
        // Property to access the Category repository for category-related database operations
        ICategoryRepository Category { get; }
        
        // Property to access the Product repository for product-related database operations
        IProductRepository Product { get; }
        
        // Property to access the ShoppingCart repository for cart-related database operations
        IShoppingCartRepository ShoppingCart { get; }
        
        // Property to access the AppUser repository for user-related database operations
        IAppUserRepository AppUser { get; }
        
        // Property to access the OrderDetail repository for order item-related database operations
        IOrderDetailRepository OrderDetail { get; }
        
        // Property to access the OrderHeader repository for order-related database operations
        IOrderHeaderRepository OrderHeader { get; }
        
        // Method to save all changes made through the repositories to the database
        void Save();
    }
}