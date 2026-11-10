using System;
using System.Linq;
using Dapper;

namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Products
	{
		public static Product[] GetProducts()
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM Product p";

			var products = db.Query<Product>(query).AsList();
			foreach (var p in products)
			{
				p.Category = db.QueryFirstOrDefault<ProductCategory>("SELECT * FROM Category WHERE Id = @Id", new { Id = p.CategoryId });
				p.SetType();
			}

			return products.ToArray();
		}
		public static void AddProduct(Product product)
		{
			using var db = GetConnectionObject();
			string query = """
			INSERT INTO Product(CategoryId, ProductTypeId, Name, PriceOverride, Stock, IsRemoved) 
				VALUES(@CategoryId, @ProductTypeId, @Name, @PriceOverride, @Stock, False);
			""";
			db.Execute(query, new
			{
				CategoryId = product.Category.Id,
				ProductTypeId = 0,
				Name = product.Name,
				PriceOverride = product.PriceOverride,
				Stock = product.Stock
			});
		}
		///<summary>Returns the stock of a single product using Id as search parameter</summary>
		public static int? CheckStock(int id)
		{
			using var db = GetConnectionObject();
			string query = "SELECT p.Stock FROM Product WHERE Id = @Id;";
			int? stock = db.QueryFirstOrDefault<int?>(query, new { Id = id });
			return stock;
		}

		public static void DeleteProduct(int id)
		{
			using var db = GetConnectionObject();
			string query = "UPDATE Product Set IsRemoved = True WHERE Id = @Id;";
			db.Execute(query, new { Id = id });
		}

		public static Product CreateProductFromId(int id)
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM Product p WHERE Id = @Id";

			var p = db.QueryFirstOrDefault<Product>(query, new { Id = id });

			if (p == null)
			{
				Console.WriteLine("Something went wrong");
			}

			p.Category = db.QueryFirstOrDefault<ProductCategory>("SELECT * FROM Category WHERE Id = @Id", new { Id = p.CategoryId }); ;
			return p;
		}

		/*
		- Add product
		-   - Must specify product type etc
		- Create barcode (R-Code using sequence ID. Intermediate / Hard!)
		- Get product from barcode
		- Check stock
		- Update ProductCategory price
		- Add barcode to product
		- UPDATE PRODUCT
		-   - Name
		-   - PriceOverride
		-   - Category (type)
		-   - Stock
		- Remove (soft-delete)
		- Remove barcode from product
		*/
	}
}
