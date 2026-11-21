namespace UtemorgonPOS.Core;

public class RowPriceModifier
{
	public int Id { get; set; }
	public int OrderRowId { get; set; }
	public decimal PriceDelta { get; set; }
	public string Reason { get; set; }

	public RowPriceModifier(string reason, decimal priceDelta)
	{
		Reason = reason;
		PriceDelta = priceDelta;
	}
}