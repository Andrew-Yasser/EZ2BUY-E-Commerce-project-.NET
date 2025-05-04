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

    /// Repository implementation for managing Product entities.
    /// Extends the generic RepositoryBase to provide specific functionality
    /// for Product data operations.
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {

        /// Reference to the application database context
        private ApplicationDbContext _db;

        /// Initializes a new instance of the ProductRepository class
        /// param name="db">The database context to be used for data operations</param>
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        /// Updates an existing Product entity in the database.
        /// This custom implementation selectively updates only specific properties
        /// and preserves the ImageUrl if a new one is not provided.
        /// This method is implemented at the repository level rather than in the generic
        /// RepositoryBase because it requires custom property mapping for the Product entity.

        /// param name="obj">The Product entity with updated values
        public void Update(Product obj)
        {
            // Fetch the existing product from the database
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            
            // Only update properties if the product exists
            if (objFromDb != null) {
                // Update basic properties
                objFromDb.Name = obj.Name;
                objFromDb.Description = obj.Description;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price = obj.Price;
                objFromDb.CategoryId = obj.CategoryId;
                
                // Only update the ImageUrl if a new one is provided
                // This prevents overwriting existing images with null values
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}