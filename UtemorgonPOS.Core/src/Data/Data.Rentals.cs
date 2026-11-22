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
			JOIN `Order` o ON odrr.OrderId = o.Id
			JOIN Customer c ON o.CustomerId = c.Id
		""";

		public static RentalProduct[]? GetAllRentalData(string? append = null)
		{
			using var db = GetConnectionObject();

			//This could easily be reused with the appended query as well.
			// Example: pass WHERE condition as string append
			var rentalArray = db.Query<Product, Order, Customer, RentalRow, RentalProduct>(
				$"{ROOT_QUERY} {append};",
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

		public static RentalProduct[]? GetAllRentalDataForCustomer(int customerId)
			=> GetAllRentalData($"WHERE c.Id = {customerId}");

		public static RentalProduct[]? GetAllRentalDataFromOrder(int orderId)
			=> GetAllRentalData($"WHERE o.Id = {orderId}");

		public static RentalProduct[]? GetAllRentalDataForProduct(int productId)
			=> GetAllRentalData($"WHERE p.Id = {productId}");
	}
}