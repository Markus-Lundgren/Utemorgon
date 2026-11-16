using System;

namespace UtemorgonPOS.Core;

public class RentalRow
{

	public int Id { get; set; }
	public int OrderRowId { get; set; }
	public DateTime ExpectedReturnDate { get; set; }
	public DateTime ReturnedDate { get; set; }

	// Explicit OrderRow injection into the RentalRow object.
	// Note: These values originate from OrderRow.

	/// <summary>13 character barcode. May include letters and symbols.</summary>
	public string Barcode { get; set; }
	/// <summary>Name of product at point of sale.</summary>
	public string Name { get; set; }
	/// <summary>Price of product at point of sale.</summary>
	public decimal Price { get; set; }

	/// <summary>The amount of products bought at point of sale.</summary>
	/// <remarks>On a rental product, this is implityly always 1.</remarks>
	public readonly int Quantity = 1;
}