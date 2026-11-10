using System;
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

			return products.ToArray();
		}
		public static void AddProduct(Product product)
		{
			using var db = GetConnectionObject();
			string query = """
			INSERT INTO Product(Name, PriceOverride, Stock, IsRemoved) 
				VALUES(@Name, @PriceOverride, @Stock, False);
			""";
			db.Execute(query, new
			{
				Name = product.Name,
				PriceOverride = product.Price,
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

			//p.Category = db.QueryFirstOrDefault<ProductCategory>("SELECT * FROM Category WHERE Id = @Id", new { Id = p.CategoryId }); ;
			return p;
		}

		public static void ProductTransaction(int id, int amount)
		{
			using var db = GetConnectionObject();
			var p = db.QueryFirstOrDefault<Product>("SELECT * FROM Product WHERE Id = @Id", new { Id = id });

			if (p == null)
			{
				Console.WriteLine("Denna produkt finns inte");
				return;
			}
			if (amount < 1)
			{
				Console.WriteLine("Du måste välja minst en");
				return;
			}
			if (p.Stock < 1 || p.Stock == null)
			{
				Console.WriteLine($"{p.Name} är tyvärr slut i lagret!");
				return;
			}
			if (p.Stock < amount)
			{
				Console.WriteLine("Det finns inte så många av denna produkt i lagret");
				return;
			}

			db.Execute("UPDATE Product SET Stock = @Stock WHERE Id = @Id", new { Stock = p.Stock - amount, Id = id });
			p.Stock -= amount;
		}

		public static void ChangePrice(int id, decimal amount)
		{
			using var db = GetConnectionObject();
			db.Execute("UPDATE Product SET Price = @Amount WHERE Id = @Id", new { Amount = amount, Id = id });
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
