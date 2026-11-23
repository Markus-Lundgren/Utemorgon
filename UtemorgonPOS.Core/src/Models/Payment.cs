using System;

namespace UtemorgonPOS.Core;

public class Payment
{
	public decimal Amount { get; set; }
	public PaymentMethod Method { get; set; }
	public int PaymentMethodId => (int)Method;
	public DateTime ReceivedDate { get; set; }

	public Payment() { }

	public Payment(decimal amount, PaymentMethod method, DateTime receivedDate)
	{
		Amount = amount;
		Method = method;
		ReceivedDate = receivedDate;
	}
}