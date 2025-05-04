using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Repositories
{

    /// Generic repository base class that implements the repository pattern for data access operations.
    /// Provides common CRUD operations for any entity type.

    /// typeparam name="T">The entity type this repository will manage</typeparam>
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {

        /// The database context used for data operations

        private readonly ApplicationDbContext _db;
        

        /// The DbSet for the specific entity type being managed

        internal DbSet<T> dbSet;
        

        /// Initializes a new instance of the RepositoryBase class

        /// param name="db">The database context to be used for data operations</param>
        public RepositoryBase(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();  // Gets the DbSet for type T (e.g., _db.Categories)
            
            // Note: This line appears to be specific to Products and should likely be moved elsewhere
            // as it's not generic and will cause issues for non-Product repositories
            _db.Products.Include(u => u.Category); // Includes the Category navigation property in Product queries
        }

        /// Adds a new entity to the database
        /// param name="model">The entity to add</param>
        public void Add(T model)
        {
            _db.Add(model);
        }


        /// Retrieves all entities matching the specified filter with optional related entities
        /// param name="filter">Optional expression to filter results</param>
        /// param name="includeProperties">Comma-separated list of navigation properties to include</param>
        /// returns>Collection of entities matching the criteria</returns>
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            
            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            // Include related entities if specified
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            
            return query.ToList();
        }


        /// Retrieves the first entity matching the specified filter with optional related entities
        /// param name="filter">Expression to filter results</param>
        /// param name="includePorperties">Comma-separated list of navigation properties to include</param>
        /// returns>First entity matching the criteria or default value if none found</returns>
        public T GetById(Expression<Func<T, bool>> filter, string? includePorperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            
            // Include related entities if specified
            if (!string.IsNullOrEmpty(includePorperties))
            {
                foreach (var includeProp in includePorperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            
            return query.FirstOrDefault();
        }


        /// Removes the specified entity from the database
        /// param name="model">The entity to delete</param>
        public void Delete(T model)
        {
            dbSet.Remove(model);
        }


        /// Removes multiple entities from the database in a single operation
        /// param name="model">Collection of entities to delete</param>
        public void DeleteRange(IEnumerable<T> model)
        {
            dbSet.RemoveRange(model);
        }
        

        /// Retrieves the first entity matching the specified filter with optional related entities and tracking control
        /// param name="filter">Expression to filter results</param>
        /// param name="includeProperties">Comma-separated list of navigation properties to include</param>
        /// param name="tracked">Whether to track the retrieved entity in the change tracker</param>
        /// returns>First entity matching the criteria or default value if none found</returns>
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            
            // Control change tracking behavior
            if (tracked)
            {
                query = dbSet; // Entity will be tracked
            }
            else
            {
                query = dbSet.AsNoTracking(); // Entity will not be tracked
            }

            query = query.Where(filter);
            
            // Include related entities if specified
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            
            return query.FirstOrDefault();
        }
    }
}