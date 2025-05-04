using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Repositories
{

    /// Implements the Unit of Work pattern to coordinate operations across multiple repositories
    /// and ensure data consistency for transactions that affect multiple entity types.
    public class UnitOfWork : IUnitOfWork
    {

        /// The database context used for all repository operations
        private ApplicationDbContext _db;
        

        /// Repository for Category entity operations
        public ICategoryRepository Category { get; private set; }


        /// Repository for Product entity operations
        public IProductRepository Product { get; private set; }


        /// Repository for ShoppingCart entity operations
        public IShoppingCartRepository ShoppingCart { get; private set; }


        /// Repository for AppUser entity operations
        public IAppUserRepository AppUser { get; private set; }
        

        /// Repository for OrderDetail entity operations
        public IOrderDetailRepository OrderDetail { get; private set; }
        

        /// Repository for OrderHeader entity operations
        public IOrderHeaderRepository OrderHeader { get; private set; }

 
        /// Initializes a new instance of the UnitOfWork class and creates all required repositories.

        ///param name="db">The database context to be used for all data operations</param>
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            
            // Initialize all repositories with the same database context
            // to ensure they all participate in the same transaction
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            AppUser = new AppUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
        }


        /// Saves all changes made through the repositories to the database.
        /// This method commits all changes as a single transaction.
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}