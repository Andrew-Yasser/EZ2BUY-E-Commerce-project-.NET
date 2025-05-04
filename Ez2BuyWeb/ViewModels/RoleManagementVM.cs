using Ez2Buy.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ez2BuyWeb.ViewModels
{
    public class RoleManagementVM
    {
        public AppUser AppUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
