using Ez2Buy.DataAccess.Models;

namespace Ez2BuyWeb.ViewModels
{
	public class OrderVM
	{
		public OrderHeader OrderHeader { get; set; }
		public IEnumerable<OrderDetail> OrderDetail { get; set; }
	}
}
