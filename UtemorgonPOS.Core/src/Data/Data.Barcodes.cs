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


	}
	/*
	 - Create barcode (R-Code using sequence ID. Intermediate / Hard!)
	 - Get product from barcode - FINITO
	 - Add barcode to product
	 - Remove barcode from product
	 */
}