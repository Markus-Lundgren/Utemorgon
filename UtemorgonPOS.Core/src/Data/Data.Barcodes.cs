using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Barcodes
	{
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


	}
	/*
	 - Create barcode (R-Code using sequence ID. Intermediate / Hard!)
	 - Get product from barcode
	 - Add barcode to product
	 - Remove barcode from product
	 */
}