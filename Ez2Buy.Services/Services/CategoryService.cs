using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.DataAccess.Repositories;
using Ez2Buy.Services.Contracts;
using Ez2Buy.Services.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Services.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _uow;
		public CategoryService(IUnitOfWork unitOfWork)
		{
			_uow = unitOfWork;
		}
		public void AddCategory(CategoryInsertDto CategoryDto)
		{
			var Category = new Category
			{
				Name = CategoryDto.Name,
				DisplayOrder = CategoryDto.DisplayOrder
			};
			_uow.Category.Add(Category);
			_uow.Save();
		}

		public void DeleteCategory(int id)
		{
			var category = _uow.Category.GetById(c => c.Id == id);
			if (category != null)
			{
				_uow.Category.Delete(category);
				_uow.Save();
			}
		}

		public IEnumerable<CategoryListDto> GetAllCategories()
		{
			var categories = _uow.Category.GetAll();
			return categories.Select(c => new CategoryListDto
			{
				Id = c.Id,
				Name = c.Name,
				DisplayOrder = c.DisplayOrder
			}).ToList();
		}
	

		public CategoryListDto GetCategoryById(int id)
		{
			var category = _uow.Category.GetById(c => c.Id == id);
			if (category == null)
			{
				return null;
			}
			return new CategoryListDto
			{
				Id = category.Id,
				Name = category.Name,
				DisplayOrder = category.DisplayOrder
			};
		}

		public void UpdateCategory(CategoryUpdateDto CategoryDto)
		{
			var Category = _uow.Category.GetById(c => c.Id == CategoryDto.Id);
			if (Category != null)
			{
				Category.Name = CategoryDto.Name;
				Category.DisplayOrder = CategoryDto.DisplayOrder;
				_uow.Category.Update(Category);
				_uow.Save();
			}
			else
			{
				throw new Exception("Category not found");
			}
		}
	}
}
