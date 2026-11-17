using System.Collections.Generic;
using System.Text;

namespace UtemorgonPOS.Core;

public class OrderRow
{
	public int Id { get; set; }
	public int OrderRowId { get; set; }
	public int OrderId { get; set; }

	/// <summary>13 character barcode. May include letters and symbols.</summary>
	public required string Barcode { get; set; }
	/// <summary>Name of product at point of sale.</summary>
	public required string Name { get; set; }
	/// <summary>Price of product at point of sale.</summary>
	public decimal Price { get; set; }

	public List<RowPriceModifier> PriceModifiers { get; set; } = [];

	/// <summary>The amount of products bought at point of sale.</summary>
	public virtual int Quantity { get; set; }

	public decimal GetTotalPrice()
	{
		decimal agg = Price * Quantity;
		foreach (var pm in PriceModifiers)
			agg += pm.PriceDelta;

		return agg;
	}

	public override string ToString()
	{
		const int LEFT = -42;
		const int RIGHT = 10;
		StringBuilder sb = new($"{Name,LEFT} {Price,RIGHT:N2} SEK\n");
		foreach (var pm in PriceModifiers)
			sb.AppendLine($"  {pm.Reason,LEFT + 2} {pm.PriceDelta,RIGHT - 2} SEK");
		if (PriceModifiers.Count > 0)
			sb.AppendLine($"{"Summa",LEFT} {GetTotalPrice(),RIGHT:N2} SEK");

		return sb.ToString();
	}
}