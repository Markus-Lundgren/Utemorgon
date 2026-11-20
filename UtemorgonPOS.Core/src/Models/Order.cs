using System;

namespace UtemorgonPOS.Core;

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public DateTime CreationDate { get; set; }

	public OrderPriceModifier[] PriceModifiers;
	public OrderRow OrderRow;

}
