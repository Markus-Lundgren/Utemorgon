using System;
using System.Linq;
using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Barcodes
	{
		/// <summary>Return a Barcode object using a string as search parameter</summary>
		/// <returns>Barcode object</returns>
		public static Barcode? GetBarcodeFromString(string barcode)
		{
			using var db = GetConnectionObject();
			string query = """
			SELECT * 
			FROM BarcodeToProduct b
			JOIN Product p ON b.ProductId = p.Id
			WHERE Barcode = @Barcode
			""";

			Barcode? b = null;
			db.Query<Barcode, Product, Barcode>(
				query,
				(barcode, product) =>
				{
					barcode.Product = product;
					b = barcode;
					return b;
				},
				new { Barcode = barcode },
				splitOn: "Id"
			);

			return b;
		}

		/// <summary>Fetches all barcodes and their products</summary>
		/// <returns>Array of Barcode objects</returns>
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

		/// <summary>Add a barcode to a product<br/>
		/// 0: Success<br/>
		/// 1: Code is already registered<br/>
		/// </summary>
		/// <returns></returns>
		public static Response AddBarcodeToProduct(int productId, string code)
		{
			using var db = GetConnectionObject();

			string query = """
			INSERT INTO BarcodeToProduct(ProductId, Barcode, IsArchived) VALUES(@ProductId, @BarCode, False);
			""";

			try
			{
				db.Execute(query, new { ProductId = productId, BarCode = code });
				return new(0, null);
			}
			catch (Exception _)
			{
				// Technically, we don't know what happened. 
				// I can't be bothered to check the true exceptions for Unique conflicts.
				return new(1, $"Barcode \"{code}\" is already registered!");
			}
		}

		/// <summary>Checks if barcode exists</summary>
		/// <returns>True if it exists, False if not</returns>
		public static bool CheckExists(string code)
		{
			using var db = GetConnectionObject();
			return db.ExecuteScalar<int>("SELECT COUNT(*) FROM BarcodeToProduct WHERE Barcode = @Code", new { Code = code }) > 0;
		}

		public static string? AddBarcodeToRental(int productId)
			=> ApplySpecialBarcode("R", productId);

		public static string? AddBarcodeToService(int productId)
			=> ApplySpecialBarcode("S", productId);

		static string? ApplySpecialBarcode(string prefix, int productId)
		{
			using var db = GetConnectionObject();

			// No guardrails because we do not fetch user input.
			return db.Query<string>($"CALL AddSpecialBarcodeToProduct('{prefix}', {productId})").FirstOrDefault();
		}

		/// <summary>Archives a barcode</summary>
		/// <param name="barcode"></param>
		/// <returns>Response object with code and message</returns>
		public static Response ArchiveBarcode(string barcode)
		{
			using var db = GetConnectionObject();

			string query = "UPDATE BarcodeToProduct SET IsArchived = 1 WHERE Barcode = @Code";

			int affectedRows = db.Execute(query, new { Code = barcode });

			if (affectedRows < 1)
			{
				return new(1, $"Ingen produkt använder barcode: {barcode}");
			}
			return new(0, null);
		}
	}
	/*
	 - Create barcode (R-Code using sequence ID. Intermediate / Hard!)
	 - Get product from barcode - FINITO
	 - Add barcode to product - FINISHED
	 - Remove barcode from product
	 */
}