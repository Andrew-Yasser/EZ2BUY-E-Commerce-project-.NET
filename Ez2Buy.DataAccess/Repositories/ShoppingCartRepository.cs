using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Repositories
{
	
    /// Repository implementation for managing ShoppingCart entities.
    /// Extends the generic RepositoryBase to provide specific functionality
    /// for ShoppingCart data operations.
    public class ShoppingCartRepository : RepositoryBase<ShoppingCart>, IShoppingCartRepository
    {

        /// Reference to the application database context
        private ApplicationDbContext _db;
        

        /// Initializes a new instance of the ShoppingCartRepository class

        /// param name="db">The database context to be used for data operations</param>
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        /// Updates an existing ShoppingCart entity in the database.
        /// This implementation uses the EF Core Change Tracker to update all properties.
        /// param name="obj">The ShoppingCart entity with updated values</param>
        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }
    }
}