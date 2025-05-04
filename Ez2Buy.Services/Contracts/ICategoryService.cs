
using Ez2Buy.Services.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Services.Contracts
{
	public interface ICategoryService
	{
		IEnumerable<CategoryListDto> GetAllCategories();
		CategoryListDto GetCategoryById(int id);
		void AddCategory(CategoryInsertDto category);
		void UpdateCategory(CategoryUpdateDto category);
		void DeleteCategory(int id);
	}
}
