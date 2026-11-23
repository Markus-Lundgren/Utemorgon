using System;
using System.Collections.Generic;
using System.Linq;

namespace UtemorgonPOS.Core;

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public DateTime CreationDate { get; set; }

	public List<OrderPriceModifier> PriceModifiers = [];
	public List<OrderRow> OrderRows = [];

	public decimal TotalCost => PriceModifiers.Sum(pm => pm.PriceDelta) + OrderRows.Sum(odr => odr.GetTotalPrice());

}
