using Ez2Buy.DataAccess.Models; // Imports the ShoppingCart model class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
    // Interface for ShoppingCart repository operations, extending the generic IRepositoryBase with ShoppingCart-specific functionality
    public interface IShoppingCartRepository : IRepositoryBase<ShoppingCart>
    {
        // Defines a method to update an existing ShoppingCart entity in the database
        // This extends the basic CRUD operations inherited from IRepositoryBase
        void Update(ShoppingCart obj);
    }
}
