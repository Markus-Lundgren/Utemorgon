using System;
using Dapper;

namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Products
	{
		/// <summary>
		/// Get an array of all Product items from database
		/// </summary>
		/// <returns>Product[]</returns>
		public static Product[] GetProducts()
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM Product p";

			var products = db.Query<Product>(query).AsList();

			foreach (var p in products)
			{
				p.SetProductType();
			}

			return products.ToArray();
		}
		/// <summary>
		/// Get an array of all Product items from database with the given name
		/// </summary>
		/// <returns>Product[]</returns>
		public static Product[] GetProducts(string name)
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM Product p WHERE Name = @Name";

			var products = db.Query<Product>(query, new { Name = name }).AsList();

			foreach (var p in products)
			{
				p.SetProductType();
			}

			return products.ToArray();
		}

		/// <summary>
		/// Get an array of all Product items from database
		/// Filters by archived status
		/// </summary>
		/// <returns>Product[]</returns>
		public static Product[] GetProducts(bool isArchived)
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM Product p WHERE IsArchived = @IsArchived";

			var products = db.Query<Product>(query, new { IsArchived = isArchived }).AsList();

			foreach (var p in products)
			{
				p.SetProductType();
			}

			return products.ToArray();
		}
		/// <summary>
		/// Add product to database using an object of Product
		/// </summary>
		/// <param name="product"></param>
		public static void AddProduct(Product product)
		{
			using var db = GetConnectionObject();
			string query = """
			INSERT INTO Product(ProductTypeId, Name, Price, Stock, IsArchived) 
				VALUES(@ProductTypeId, @Name, @Price, @Stock, False);
			""";
			db.Execute(query, new
			{
				ProductTypeId = (int)product.TypeInfo,
				Name = product.Name,
				Price = product.Price,
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

		/// <summary>
		/// Soft delete a product from the database
		/// </summary>
		/// <param name="id"></param>
		public static void DeleteProduct(int id)
		{
			using var db = GetConnectionObject();
			string query = "UPDATE Product Set IsArchived = True WHERE Id = @Id;";
			db.Execute(query, new { Id = id });
		}

		/// <summary>
		/// Get product from database using Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Product object</returns>
		public static Product? GetProductFromId(int id)
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM Product p WHERE Id = @Id";

			var p = db.QueryFirstOrDefault<Product>(query, new { Id = id });

			// if (p == null)
			// {
			// 	Console.WriteLine("Something went wrong");
			// }
			// Use outside method instead...p.SetProductType();
			return p;
		}

		/// <summary>
		/// Product transaction, either rent or buy. Will subtract from stock.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="amount"></param>
		/// <returns>Numerical code
		/// 0: Success -
		/// 1: Product does not exist -
		/// 2: Amount too low -
		/// 3: Product out of stock -
		/// 4: Product stock is lower than amount
		/// </returns>
		public static int ProductTransaction(int id, int amount)
		{
			using var db = GetConnectionObject();
			var p = db.QueryFirstOrDefault<Product>("SELECT * FROM Product WHERE Id = @Id", new { Id = id });

			if (p == null)
			{
				//Console.WriteLine("Denna produkt finns inte");
				return 1;
			}
			if (amount < 1)
			{
				//Console.WriteLine("Du måste välja minst en");
				return 2;
			}
			if (p.Stock < 1 || p.Stock == null)
			{
				//Console.WriteLine($"{p.Name} är tyvärr slut i lagret!");
				return 3;
			}
			if (p.Stock < amount)
			{
				//Console.WriteLine("Det finns inte så många av denna produkt i lagret");
				return 4;
			}

			db.Execute("UPDATE Product SET Stock = @Stock WHERE Id = @Id", new { Stock = p.Stock - amount, Id = id });
			p.Stock -= amount;
			return 0;
		}

		/// <summary>
		/// Update price on a product using Id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="amount"></param>
		public static void ChangePrice(int id, decimal amount)
		{
			using var db = GetConnectionObject();
			if (amount < 1)
			{
				Console.WriteLine($"Du kan inte sätta {amount} som pris");
				return;
			}
			db.Execute("UPDATE Product SET Price = @Amount WHERE Id = @Id", new { Amount = amount, Id = id });
		}
		/// <summary>
		/// Update name of a product using id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		public static void ChangeName(int id, string name)
		{
			using var db = GetConnectionObject();
			if (string.IsNullOrWhiteSpace(name))
			{
				Console.WriteLine($"Name can't be empty");
				return;
			}
			db.Execute("UPDATE Product SET Name = @Name WHERE Id = @Id", new { Name = name, Id = id });
		}
		/// <summary>
		/// Change product type of a product using id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		public static void ChangeProductType(int id, ProductType type)
		{
			using var db = GetConnectionObject();

			db.Execute("UPDATE Product SET ProductTypeId = @TypeId WHERE Id = @Id", new { TypeId = (int)type, Id = id });
		}

		/// <summary>
		/// Update price for a product category using the id of the category
		/// </summary>
		/// <param name="categoryId"></param>
		/// <param name="amount"></param>
		public static void UpdateProductCategoryPrice(int categoryId, int amount)
		{
			using var db = GetConnectionObject();

			db.Execute("UPDATE Product SET Price = @Amount WHERE ProductTypeId = @CategoryId",
						new { categoryId = categoryId, Amount = amount });
		}

		/*
		- Add product - FINISHED
		-   - Must specify product type etc
		- Create barcode (R-Code using sequence ID. Intermediate / Hard!)
		- Get product from barcode
		- Check stock - FINISHED
		- Update ProductCategory price - FINISHED
		- Add barcode to product
		- Rent or buy product - FINISHED
		- UPDATE PRODUCT
		-   - Name - FINISHED
		-   - Price - FINISHED
		-   - Category (type) - FINISHED
		-   - Stock - FINISHED
		- Remove (soft-delete) - FINISHED
		- Remove barcode from product
		*/
	}
}
