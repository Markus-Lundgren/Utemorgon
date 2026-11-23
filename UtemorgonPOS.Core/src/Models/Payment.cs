using System;

namespace UtemorgonPOS.Core;

public class Payment
{
	public decimal Amount { get; set; }
	public PaymentMethod Method { get; set; }
	public DateTime Received { get; set; }

	public Payment() { }

	public Payment(decimal amount, PaymentMethod method, DateTime received)
	{
		Amount = amount;
		Method = method;
		Received = received;
	}
}