﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Services.DTOs.Product
{
	public class ProductDetailDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal ListPrice { get; set; }
		public decimal Price { get; set; }
		public string ImageUrl { get; set; }
		public int CategoryId { get; set; }

		public string CategoryName { get; set; }
	}
}
