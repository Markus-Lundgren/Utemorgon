using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtemorgonPOS.Core;

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public DateTime CreationDate { get; set; }

	public List<Payment> Payments { get; set; } = [];
	public List<OrderPriceModifier> PriceModifiers { get; set; } = [];
	public List<OrderRow> OrderRows { get; set; } = [];

	// This is very allocating, not very memory aware.
	public decimal TotalCost => PriceModifiers.Sum(pm => pm.PriceDelta) + OrderRows.Sum(odr => odr.GetTotalPrice());
	public decimal TotalPaid => Payments.Sum(p => p.Amount);
	public bool IsOutstanding => TotalPaid < TotalCost;


	public string AsCard()
	{
		StringBuilder sb = new();

		sb.AppendLine($"Order ID: {Id}");
		sb.AppendLine($"- MODIFIERS");
		foreach (var mod in PriceModifiers)
			sb.AppendLine($"\"{mod.Reason}\" {mod.PriceDelta}");

		sb.AppendLine($"- ITEMS");
		foreach (var row in OrderRows)
			sb.AppendLine($"{row}");

		decimal tc = TotalCost;
		decimal tp = TotalPaid;
		sb.AppendLine($"Total: {tp} / {tc} (diff: {tc - tp})");

		return sb.ToString().TrimEnd();
	}

}
