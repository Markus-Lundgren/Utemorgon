using System.Text;

namespace UtemorgonPOS.Core;

public class Customer
{
	public int Id { get; set; }
	public string? SocialNumber { get; set; }
	public string FName { get; set; } = string.Empty;
	public string LName { get; set; } = string.Empty;
	public string? Address { get; set; }
	public string? PhoneNumber { get; set; }

	public string FullName => $"{FName} {LName}";

	public string SSNFormated => string.IsNullOrEmpty(SocialNumber) || SocialNumber.Length < 4
		? ""
		: $"{SocialNumber[..^4]}-{SocialNumber[^4..]}";

	public string AsCard()
	{
		StringBuilder sb = new();

		sb.AppendLine($"Customer ID: {Id}");
		sb.AppendLine($"Name: {FullName}");
		sb.AppendLine($"Personal number: {SocialNumber ?? "Not provided"}");
		sb.AppendLine($"Address: {Address}");
		sb.AppendLine($"Phone number: {PhoneNumber}");

		return sb.ToString();
	}
}
