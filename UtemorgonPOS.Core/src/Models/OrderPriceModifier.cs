namespace UtemorgonPOS.Core;

public class OrderPriceModifier
{
	public int Id { get; set; }
	public int OrderId { get; set; }
	public decimal PriceDelta { get; set; }
	public required string Reason { get; set; }
}