using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.DbInitializer
{
    // responsible for creating admin user and roles of our website
    public interface IDbInitializer
    {
        void Initialize();
    }
}
