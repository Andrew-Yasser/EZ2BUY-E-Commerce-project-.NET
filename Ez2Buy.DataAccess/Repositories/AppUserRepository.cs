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
    /// <summary>
    /// Repository implementation for managing AppUser entities.
    /// This class extends the generic RepositoryBase to provide specific functionality 
    /// for AppUser data operations.
    /// </summary>
    public class AppUserRepository : RepositoryBase<AppUser>, IAppUserRepository
    {

        /// Reference to the application database context.
        private ApplicationDbContext _db;
        

        /// Initializes a new instance of the AppUserRepository class.
        /// param name="db">The database context to be used for data operations.</param>
        public AppUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        
        // Note: Currently this repository does not implement any custom methods
        // beyond what is inherited from RepositoryBase<AppUser>.
        // Additional AppUser-specific data operations can be added here as needed.
    }
}