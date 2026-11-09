namespace UtemorgonPOS.Core;

public class Customer
{
	public int Id { get; set; }
	public string SocialNumber { get; set; }
	public string FName { get; set; }
	public string LName { get; set; }
	public string Address { get; set; }
	public string PhoneNumber { get; set; }

	public string FullName => $"{FName} {LName}";
	public string SSNFormated => $"{SocialNumber[..^4]} - {SocialNumber[^4..]}";
}
