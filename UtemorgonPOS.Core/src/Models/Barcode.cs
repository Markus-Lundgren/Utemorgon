namespace UtemorgonPOS.Core;

public class Barcode
{
	public int Id { get; set; }
	public Product product;
	public int ProductId { get; set; }
	public bool IsArchived { get; set; }
}