namespace UtemorgonPOS.Core;

public class RowPriceModifier
{
	public int Id { get; set; }
	public int OrderRowId { get; set; }
	public decimal PriceDelta { get; set; }
	public required string Reason { get; set; }
}