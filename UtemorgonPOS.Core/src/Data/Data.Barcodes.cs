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
		public static Product? GetProductFromBarcode(string barcode)
		{
			using var db = GetConnectionObject();
			string query = """
			SELECT * 
			FROM Product p 
			JOIN BarcodeToProduct b ON b.ProductId = p.Id
			WHERE Barcode = @Barcode
			""";

			Product? p = db.QueryFirstOrDefault<Product>(query, new { Barcode = barcode });

			return p;
		}

		/// <summary>
		/// Fetches all barcodes and their products
		/// </summary>
		/// <returns></returns>
		public static Barcode[] GetAllProductsWithBarcode()
		{
			using var db = GetConnectionObject();
			string query = """
						SELECT b.*, p.* 
						FROM Product p 
						JOIN BarcodeToProduct b ON b.ProductId = p.Id
						ORDER BY p.Id
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
			//var exist = db.QueryFirstOrDefault<Barcode>("SELECT * FROM BarcodeToProduct WHERE Barcode = @Code", new { Code = code });
			if (CheckExists(code))
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

		/// <summary>
		/// Checks if barcode exists
		/// </summary>
		/// <param name="code"></param>
		/// <returns>True if it exists, False if not</returns>
		public static bool CheckExists(string code)
		{
			using var db = GetConnectionObject();

			var exist = db.QueryFirstOrDefault<Barcode>("SELECT * FROM BarcodeToProduct WHERE Barcode = @Code", new { Code = code });
			if (exist != null)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks if a barcode is connected to a currently rented product
		/// </summary>
		/// <param name="barcode"></param>
		/// <returns></returns>
		public static bool GetRentedStatus(Barcode barcode)
		{
			using var db = GetConnectionObject();

			string query = """
				SELECT COUNT(*) FROM OrderRow or
				JOIN BarcodeToProduct b ON or.Barcode = b.Barcode
				JOIN RentalRow r ON r.OrderRowId = or.Id
				WHERE or.Barcode = @Code AND r.ReturnedDate IS NULL
			""";

			var rented = db.ExecuteScalar<int>(query, new { Code = barcode.BarCode });

			return rented > 0;
		}

		///<summary>
		/// Returns a single product
		/// Adds one to stock
		/// NOTE: NEEDS to know if product is rented
		/// Currently only adds one to stock
		///</summary>
		public static void ReturnSingleProduct(Barcode barcode)
		{
			using var db = GetConnectionObject();

			barcode.Product.Stock += 1;

			string query = "UPDATE Product SET Stock = @NewStock WHERE Id = @Id";

			db.Execute(query, new { NewStock = barcode.Product.Stock, Id = barcode.ProductId });
		}
	}
	/*
	 - Create barcode (R-Code using sequence ID. Intermediate / Hard!)
	 - Get product from barcode - FINITO
	 - Add barcode to product - FINISHED
	 - Remove barcode from product
	 */
}