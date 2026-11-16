using System.Collections.Generic;

namespace UtemorgonPOS.Core;

public class OrderRow
{
	public int Id { get; set; }
	public int COrderId { get; set; }

	/// <summary>13 character barcode. May include letters and symbols.</summary>
	public required string Barcode { get; set; }

	/// <summary>Name of product at point of sale.</summary>
	public required string Name { get; set; }

	/// <summary>Price of product at point of sale.</summary>
	public decimal Price { get; set; }

	/// <summary>The amount of products bought at point of sale.</summary>
	public int Quantity { get; set; }
}