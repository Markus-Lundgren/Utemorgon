using System.Diagnostics.CodeAnalysis;

namespace UtemorgonPOS.Core;

public class OrderPriceModifier
{
	public int Id { get; set; }
	public int OrderId { get; set; }
	public decimal PriceDelta { get; set; }
	public required string Reason { get; set; }

	public OrderPriceModifier() { }

	[SetsRequiredMembers]
	public OrderPriceModifier(string reason, decimal priceDelta)
	{
		Reason = reason;
		PriceDelta = priceDelta;
	}
}