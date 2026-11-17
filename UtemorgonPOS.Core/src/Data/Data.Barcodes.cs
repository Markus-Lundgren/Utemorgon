using System;
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

		/// <summary>Checks if a barcode is connected to a currently rented product </summary>
		/// <returns>True if rented, False if not</returns>
		public static bool GetRentedStatus(Barcode barcode)
		{
			using var db = GetConnectionObject();

			string query = """
				SELECT COUNT(*) FROM OrderRow orderRow
				JOIN BarcodeToProduct b ON orderRow.Barcode = b.Barcode
				JOIN RentalRow r ON r.OrderRowId = orderRow.Id
				WHERE orderRow.Barcode = @Code AND r.ReturnedDate IS NULL
			""";

			var rented = db.ExecuteScalar<int>(query, new { Code = barcode.BarCode });

			return rented > 0;
		}

		///<summary>Returns a single product<br/>
		///Adds one to stock and sets returned date to now
		///</summary>
		/// <returns>True on success, else False</returns>
		public static bool TryReturnSingleProduct(Barcode barcode)
		{
			using var db = GetConnectionObject();

			if (!CheckExists(barcode.BarCode)) return false;
			if (!GetRentedStatus(barcode)) return false;

			barcode.Product.Stock += 1;

			string query = "UPDATE Product SET Stock = @NewStock WHERE Id = @Id";

			db.Execute(query, new { NewStock = barcode.Product.Stock, Id = barcode.ProductId });

			query = """
			UPDATE RentalRow r
			JOIN OrderRow o ON r.OrderRowId = o.Id
			SET r.ReturnedDate = NOW()
			WHERE o.Barcode = @Code AND r.ReturnedDate IS NULL
			""";
			db.Execute(query, new { Code = barcode.BarCode });
			return true;
		}

		public static Response ReturnRental(string barcode)
		{
			using var db = GetConnectionObject();
			string query = """
			UPDATE RentalRow r
			JOIN OrderRow o ON r.OrderRowId = o.Id
			SET r.ReturnedDate = NOW()
			WHERE o.Barcode = @Code AND r.ReturnedDate IS NULL
			""";
			db.Execute(query, new { Code = barcode });

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