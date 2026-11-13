using System;

namespace UtemorgonPOS.Core;

public class RentalRow
{

    public int Id { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime ReturnedDate { get; set; }
    public OrderRow[] OrderRows;
}