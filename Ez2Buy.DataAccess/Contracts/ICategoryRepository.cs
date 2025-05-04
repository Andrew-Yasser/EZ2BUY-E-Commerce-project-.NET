
using Ez2Buy.DataAccess.Models; // Imports the Category model class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
    // Interface for Category repository operations, extending the generic IRepositoryBase with Category-specific functionality
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        // Defines a method to update an existing Category entity in the database
        // This extends the basic CRUD operations inherited from IRepositoryBase
        void Update(Category obj);
    }
}