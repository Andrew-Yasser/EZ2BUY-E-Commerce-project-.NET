using Ez2Buy.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ez2BuyWeb.ViewModels
{
	public class ProductVM
	{
        public Product Product { get; set; }  // Product object to hold product details(single product)
        [ValidateNever]
		public IEnumerable<SelectListItem> CategoryList { get; set; }
	}
}
