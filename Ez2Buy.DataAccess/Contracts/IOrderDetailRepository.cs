using Ez2Buy.DataAccess.Models; // Imports the OrderDetail model class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
    // Interface for OrderDetail repository operations, extending the generic IRepositoryBase with OrderDetail-specific functionality
    public interface IOrderDetailRepository : IRepositoryBase<OrderDetail>
    {
        // Defines a method to update an existing OrderDetail entity in the database
        // This extends the basic CRUD operations inherited from IRepositoryBase
        void Update(OrderDetail obj);
    }
}