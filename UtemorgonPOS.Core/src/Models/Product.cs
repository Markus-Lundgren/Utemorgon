namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public int CategoryId { get; set; }
	public int ProductTypeId { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public int Stock { get; set; }
	public bool IsRemoved { get; set; }
}
