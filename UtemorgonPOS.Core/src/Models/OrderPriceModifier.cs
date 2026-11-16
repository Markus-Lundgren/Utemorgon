namespace UtemorgonPOS.Core;

public class OrderPriceModifier
{


    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
}