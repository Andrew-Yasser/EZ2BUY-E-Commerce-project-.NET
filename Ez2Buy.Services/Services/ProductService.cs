using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.DataAccess.Repositories;
using Ez2Buy.Services.Contracts;
using Ez2Buy.Services.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Services.Services
{
    public class ProductService : IProductService
	{
		private readonly IUnitOfWork _uow;
		public ProductService(IUnitOfWork unitOfWork)
		{
			_uow = unitOfWork;
		}

		public IEnumerable<ProductListItemDto> GetAllProducts()
		{
			var products = _uow.Product.GetAll();
			return products.Select(p => new ProductListItemDto
			{
				Id = p.Id,
				Name = p.Name,
				Price = p.Price,
				ImageUrl = p.ImageUrl,
				CategoryId = p.CategoryId
			}).ToList();
		}

		public ProductDetailDto GetProductById(int id)
		{
			var product = _uow.Product.GetById(p => p.Id == id);
			if (product == null) return null;

			return new ProductDetailDto
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				ListPrice = product.ListPrice,
				Price = product.Price,
				ImageUrl = product.ImageUrl,
				CategoryId = product.CategoryId,
				CategoryName = product.Category?.Name // Assuming navigation property is included
			};
		}

		public void AddProduct(CategorytInsertDto productDto)
		{
			var product = new Product
			{
				Name = productDto.Name,
				Description = productDto.Description,
				ListPrice = productDto.ListPrice,
				Price = productDto.Price,
				ImageUrl = productDto.ImageUrl,
				CategoryId = productDto.CategoryId
			};
			_uow.Product.Add(product);
			_uow.Save();
		}

		public void UpdateProduct(ProductUpdateDto productDto)
		{
			var product = _uow.Product.GetById(p => p.Id == productDto.Id);
			if (product != null)
			{
				product.Name = productDto.Name;
				product.Description = productDto.Description;
				product.ListPrice = productDto.ListPrice;
				product.Price = productDto.Price;
				product.ImageUrl = productDto.ImageUrl;
				product.CategoryId = productDto.CategoryId;
				_uow.Product.Update(product);
				_uow.Save();
			}
		}

		public void DeleteProduct(int id)
		{
			var product = _uow.Product.GetById(p => p.Id == id);
			if (product != null)
			{
				_uow.Product.Delete(product);
				_uow.Save();
			}
		}
	}
}
