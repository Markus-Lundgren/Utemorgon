using System;

namespace UtemorgonPOS.Core;

public class COrder
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreationDate { get; set; }

    public OrderPriceModifier[] PriceModifiers;

}
