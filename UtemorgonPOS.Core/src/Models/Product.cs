namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Price { get; set; }
	public int Stock { get; set; }
	public bool IsRemoved { get; set; }
}
