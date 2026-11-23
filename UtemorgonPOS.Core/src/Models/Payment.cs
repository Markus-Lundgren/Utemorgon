namespace UtemorgonPOS.Core;

public class Payment
{
	public decimal Amount { get; set; }
	public PaymentMethod Method { get; set; }

	public Payment() { }

	public Payment(decimal amount, PaymentMethod method)
	{
		Amount = amount;
		Method = method;
	}
}