using Ez2Buy.DataAccess.Models;

namespace Ez2BuyWeb.ViewModels
{
    public class PagedProductVM
    {
        public IEnumerable<Product> Products { get; set; }  // A collection of products
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
