using Ez2Buy.DataAccess.Models;

namespace Ez2BuyWeb.ViewModels
{
    public class ShoppingCartVM         //we make this vm bec we want to pass the order header and the shopping cart list to the view(data from 2 models)
	{
        public IEnumerable<ShoppingCart> shoppingCartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
