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
				rr.Id, rr.OrderRowId, rr.ExpectedReturnDate, rr.ReturnedDate, 
				odrr.COrderId as OrderId, odrr.Barcode, odrr.Name, odrr.Price,
				pm.Id, pm.OrderRowId, pm.PriceDelta, pm.Reason
			FROM RentalRow rr
				INNER JOIN OrderRow odrr ON rr.OrderRowId = odrr.Id
				LEFT JOIN OrderRowPriceModifier pm ON rr.OrderRowId = pm.OrderRowId
		""";

		public static RentalRow? GetRentalRow(int rentalRowId)
		{
			using var db = GetConnectionObject();

			var rentalRowDict = new Dictionary<int, RentalRow>();

			return db.Query<RentalRow, RowPriceModifier, RentalRow>(
				$"{ROOT_QUERY} WHERE rr.Id = @Id",
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
				new { Id = rentalRowId },
				splitOn: "Id"
			).FirstOrDefault();
		}
	}
}