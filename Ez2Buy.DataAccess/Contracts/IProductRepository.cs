using Ez2Buy.DataAccess.Models; // Imports the Product model class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
    // Interface for Product repository operations, extending the generic IRepositoryBase with Product-specific functionality
    public interface IProductRepository : IRepositoryBase<Product>
    {
        // Defines a method to update an existing Product entity in the database
        // This extends the basic CRUD operations inherited from IRepositoryBase
        void Update(Product obj);
    }
}