using System.Collections.Generic;
using Dapper;

namespace UtemorgonPOS.Core;

public static partial class Data
{
	public class OrderRows
	{
		/// <summary>
		/// Note, when using this we must also map the PriceModifers.
		/// </summary>
		const string ROOT_QUERY = """ 
			SELECT 
				odrr.Id, odrr.COrderId AS OrderId, odrr.Barcode, odrr.Name, odrr.Price, odrr.Quantity,
				rr.Id AS RentalRowId, rr.ExpectedReturnDate, rr.ReturnedDate, 
				pm.Id, pm.OrderRowId, pm.PriceDelta, pm.Reason
			FROM OrderRow odrr
				LEFT JOIN RentalRow rr ON rr.OrderRowId = odrr.Id
				LEFT JOIN OrderRowPriceModifier pm ON rr.OrderRowId = pm.OrderRowId
		""";

		public static OrderRow[] GetAllRowsForOrder(int orderId)
		{
			using var db = GetConnectionObject();
			var rowDict = new Dictionary<int, OrderRow>();

			db.Query<OrderRow, RentalRow, RowPriceModifier, OrderRow>(
				$"{ROOT_QUERY} WHERE odrr.COrderId = @OrderId",
				(orderRow, rentalRow, modifier) =>
				{
					// This is a little convoluted at first glance, but the logic is quite simple:
					// 		Check if this row is in the cached dictionary (so we don't have to run Distinct and easier can add modifiers)
					// 		If this is a rental row, manually merge the properties from the orderRow into the rentalRow.
					// 		Then set the "currentRow" to either the orderRow object or rentalRow object (if it exists)
					// 		This hides the RentalRow object inside its parent, so we can use "if(row is RentalRow rr)" later.

					if (!rowDict.TryGetValue(orderRow.Id, out var currentRow))
					{
						// If rental row, hydrate the properties manually
						if (rentalRow is not null)
						{
							rentalRow.Id = orderRow.Id;
							rentalRow.OrderId = orderRow.OrderId;
							rentalRow.Barcode = orderRow.Barcode;
							rentalRow.Name = orderRow.Name;
							rentalRow.Price = orderRow.Price;
							// DO NOT ADD QUANTITY HERE! IT IS HARD SET TO 1 ANYWAY!

							currentRow = rentalRow;
						}
						else
							currentRow = orderRow;

						// Add the row (note that this can hide a RentalRow, which is what we want!)
						rowDict.Add(currentRow.Id, currentRow);
					}

					if (modifier is not null)
						currentRow.PriceModifiers.Add(modifier);

					return currentRow;
				},
				new { OrderId = orderId },
				splitOn: "RentalRowId, Id"
			);

			return [.. rowDict.Values];
		}

		public static RentalRow[] GetLateRentalRows()
		{
			using var db = GetConnectionObject();

			var rentalRowDict = new Dictionary<int, RentalRow>();

			// Note: This query runs, and uses the map methoid to collect unique rows.
			// This eliminates the need to run distinct, or to even cache the results.
			db.Query<RentalRow, RowPriceModifier, RentalRow>(
				$"{ROOT_QUERY} WHERE rr.ReturnedDate IS NULL AND rr.ExpectedReturnDate < CURDATE()",
				(rentalRow, modifier) =>
				{
					// If this row is not accounted for, add it.
					if (!rentalRowDict.TryGetValue(rentalRow.Id, out var currentRentalRow))
					{
						currentRentalRow = rentalRow;
						rentalRowDict.Add(currentRentalRow.Id, currentRentalRow);
					}
					// If a modifier exists, meaning the LEFT JOIN is not full of nulls, we add it.
					if (modifier is not null)
						currentRentalRow.PriceModifiers.Add(modifier);

					return currentRentalRow;
				},
				splitOn: "Id"
			);

			// Since we already mapped all of the rentalrows into the dictionary, we can just return all the objects.
			// Weird mix of confusing and kinda self explanatory...
			return [.. rentalRowDict.Values];
		}

		// TODO: If time allows, check if return was successfull

		///<summary>Returns rented product
		///</summary>
		/// <returns>Response object</returns>
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
}