using System.Collections.Generic;

namespace UtemorgonPOS.Core;

public class OrderRow
{
    public int Id { get; set; }
    public int COrderId { get; set; }
    public int Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public ICollection<COrder> Orders { get; set; }

}