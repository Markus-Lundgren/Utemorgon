using System;
using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Barcodes
	{
		/// <summary>
		/// Return a product from the barcode
		/// </summary>
		/// <param name="barcode"></param>
		/// <returns></returns>
		public static Product GetProductFromBarcode(string barcode)
		{
			using var db = GetConnectionObject();
			string query = """
			SELECT * 
			FROM Product p 
			JOIN BarcodeToProduct b ON b.ProductId = p.Id
			WHERE Barcode = @Barcode
			""";

			Product p = db.QueryFirstOrDefault<Product>(query, new { Barcode = barcode });

			if (p == null)
			{
				Console.WriteLine("Hittade ingen produkt");
				p = new();
			}

			return p;
		}

		public static Barcode[] GetAllProductsWithBarcode()
		{
			using var db = GetConnectionObject();
			string query = """
						SELECT b.*, p.* 
						FROM Product p 
						JOIN BarcodeToProduct b ON b.ProductId = p.Id
						""";

			var barcodes = db.Query<Barcode, Product, Barcode>(
				query,
				(barcode, product) =>
				{
					barcode.Product = product;
					return barcode;
				},
				splitOn: "Id"
			).AsList().ToArray();

			return barcodes;
		}

		//ProductID kanske ska tillåta att vara null
		/// <summary>
		/// Add a barcode to a product
		/// </summary>
		/// <param name="product"></param>
		/// <param name="code"></param>
		public static void AddBarcodeToProduct(Product product, string code)
		{
			using var db = GetConnectionObject();
			var exist = db.QueryFirstOrDefault<Barcode>("SELECT * FROM BarcodeToProduct WHERE Barcode = @Code", new { Code = code });
			if (exist != null)
			{
				var existProduct = db.QueryFirstOrDefault<Product>("SELECT Name FROM Product WHERE Id = @Id", new { Id = exist.ProductId });
				Console.WriteLine($"Koden är redan registrerad till: {existProduct.Name}");
				return;
			}

			int? stock = product.Stock ?? 0;
			var barcodeCount = db.ExecuteScalar<int>("SELECT COUNT(*) FROM BarcodeToProduct WHERE ProductId = @Id", new { Id = product.Id });

			if (barcodeCount >= stock)
			{
				Console.WriteLine("Alla dessa produkter har redan en barcode");
				return;
			}

			string query = """
			INSERT INTO BarcodeToProduct(ProductId, Barcode, IsArchived) VALUES(@ProductId, @BarCode, False);
			""";
			db.Execute(query, new { ProductId = product.Id, BarCode = code });
		}
	}
	/*
	 - Create barcode (R-Code using sequence ID. Intermediate / Hard!)
	 - Get product from barcode - FINITO
	 - Add barcode to product
	 - Remove barcode from product
	 */
}