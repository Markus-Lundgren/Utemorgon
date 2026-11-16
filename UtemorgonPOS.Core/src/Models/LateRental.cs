using System;

namespace UtemorgonPOS.Core;

public class LateRental
{
  public string FullName { get; set; }
  public string PhoneNumber { get; set; }
  public string MovieName { get; set; }
  public DateTime? ExpectedReturnDate { get; set; }
}