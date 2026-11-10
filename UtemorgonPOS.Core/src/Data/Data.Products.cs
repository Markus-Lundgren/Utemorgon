using Dapper;

namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Products
	{
		public static Product[] GetProducts()
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Product>("SELECT * FROM Product;")];
		}
		public static void AddProduct(ProductCategory category, ProductType type, string name, int? priceOverride, int stock)
		{
			using var db = GetConnectionObject();
			string query = """
			INSERT INTO Product(CategoryId, ProductTypeId, Name, PriceOverride, Stock, IsRemoved) 
				VALUES(@CategoryId, @ProductId, @Name, @PriceOverride, @Stock, False);
			""";
			db.Execute(query);
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
			string query = "UPDATE Product Set IsRemoved = True WHERE Id = @Id";
			db.Execute(query, new { Id = id });
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
