using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; // Provides Expression<T> type for LINQ queries
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
    // Generic repository interface for all entity types (T)
    // Constrains T to be a class (reference type) to ensure it can be an entity
    public interface IRepositoryBase<T> where T : class
    {
        //T - Category, Product, Order,etc
        //we remove update method bec Different Update Strategies
        
        // Retrieves all entities of type T with optional filtering and related entity inclusion
        // filter: Optional lambda expression to filter results
        // includeProperties: Optional comma-separated string of navigation properties to include (eager loading)
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        
        // Retrieves a single entity matching the provided filter criteria
        // filter: Lambda expression defining the search conditions
        // includeProperties: Optional comma-separated string of navigation properties to include
        // tracked: Determines if Entity Framework should track the returned entity
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        
        // Retrieves an entity by its identifier using a filter expression
        // filter: Lambda expression that typically targets the primary key
        // includeProperties: Optional comma-separated string of navigation properties to include
        T GetById(Expression<Func<T, bool>> filter, string? includeProperties = null);
        
        // Adds a new entity of type T to the repository
        void Add(T model);
        
        // Removes an existing entity from the repository
        void Delete(T model);
        
        // Removes multiple entities from the repository in a single operation
        void DeleteRange(IEnumerable<T> model);
    }
}