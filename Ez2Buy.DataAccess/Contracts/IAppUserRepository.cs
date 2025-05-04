// Importing the AppUser model from the Ez2Buy.DataAccess.Models namespace
using Ez2Buy.DataAccess.Models;

// Importing basic system libraries for general programming functionalities
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Defining the namespace for data access contract interfaces
namespace Ez2Buy.DataAccess.Contracts
{
	// Defining an interface for the AppUser repository, inheriting from a generic repository base for AppUser entities
	public interface IAppUserRepository : IRepositoryBase<AppUser>
	{
		// No additional methods are declared here;
		// the interface relies entirely on the base repository's functionalities for AppUser entities
	}
}
