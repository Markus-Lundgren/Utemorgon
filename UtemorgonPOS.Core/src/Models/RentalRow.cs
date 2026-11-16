using System;

namespace UtemorgonPOS.Core;

public class RentalRow : OrderRow
{
	public int RentalRowId { get; set; }
	public DateTime ExpectedReturnDate { get; set; }
	public DateTime? ReturnedDate { get; set; }

	/// <summary>The amount of products bought at point of sale.</summary>
	/// <remarks>On a rental product, this is always exactly 1.</remarks>
	public override int Quantity
	{
		get => 1;
		set { /* Leaving accessible because Dapper needs it, but do nothing for Rentals! */ }
	}
}