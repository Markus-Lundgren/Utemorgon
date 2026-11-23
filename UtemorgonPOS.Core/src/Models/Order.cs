using System;
using System.Collections.Generic;

namespace UtemorgonPOS.Core;

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public DateTime CreationDate { get; set; }

	public List<OrderPriceModifier> PriceModifiers;
	public List<OrderRow> OrderRows;

}
