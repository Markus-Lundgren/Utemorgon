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
		public static void AddProduct(string name, int stock)
		{
			using var db = GetConnectionObject();
			string query = "INSERT INTO Product(CategoryId, ProductTypeId, Name, PriceOverride, Stock, IsRemoved) VALUES();";
			db.Execute(query);
		}
	}
}
