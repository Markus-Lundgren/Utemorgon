using System;
using System.Linq;
using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public class Rentals
	{
		const string ROOT_QUERY = """
			SELECT p.*, o.*, c.*, rr.*
			FROM RentalRow rr
			JOIN OrderRow odrr ON rr.OrderRowId = odrr.Id
			JOIN BarcodeToProduct btp ON odrr.Barcode = btp.Barcode
			JOIN Product p ON btp.ProductId = p.Id
			JOIN `COrder` o ON odrr.OrderId = o.Id
			JOIN Customer c ON o.CustomerId = c.Id
		""";
		//THIS METHOD IS HIGHLY EXPERIMENTAL, NOT MEANT FOR USE AS IS
		public static RentalProduct[]? GetAllRentalData()
		{
			using var db = GetConnectionObject();

			var rentalArray = db.Query<Product, Order, Customer, RentalRow, RentalProduct>(
				ROOT_QUERY,
				(product, order, customer, rentalRow) => new RentalProduct
				{
					Product = product,
					Order = order,
					Customer = customer,
					RentalRow = rentalRow
				},
				splitOn: "Id"
			).ToArray();

			return rentalArray ?? [];
		}
	}
}