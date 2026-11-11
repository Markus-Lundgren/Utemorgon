namespace UtemorgonPOS.Core;

public class Barcode
{
	public int Id { get; set; }
	public Product Product;

	public string BarCode { get; set; }
	public int ProductId { get; set; }
	public bool IsArchived { get; set; }
}