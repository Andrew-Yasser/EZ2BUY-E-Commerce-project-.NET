using Ez2Buy.DataAccess.Models;
using Ez2Buy.Services.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Services.Contracts
{
    public interface IProductService
	{
		IEnumerable<ProductListItemDto> GetAllProducts();
		ProductDetailDto GetProductById(int id);
		void AddProduct(CategorytInsertDto product);
		void UpdateProduct(ProductUpdateDto product);
		void DeleteProduct(int id);


	}
}
