namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public ProductType ProductType { get; set; }
	public ProductCategory Category { get; set; }
	public string Name { get; set; }
	public int? PriceOverride { get; set; }
	public int Stock { get; set; }

	//TODO: Fastställ hur kategori och product type sätts in
	public bool IsRemoved { get; set; }
}
