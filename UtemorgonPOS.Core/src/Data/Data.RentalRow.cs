using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace UtemorgonPOS.Core;


public static partial class Data
{
	public static class RentalRows
	{

		/// <summary>
		/// Note, when using this we must also map the PriceModifers and RentalRows.
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
	}
}