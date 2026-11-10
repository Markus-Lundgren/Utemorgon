namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public ProductType ProductType { get; set; }
	public ProductCategory Category { get; set; }
	public string Name { get; set; }
	public decimal? PriceOverride { get; set; }
	public int? Stock { get; set; }
	public bool IsRemoved { get; set; }

	public decimal GetPrice()
	{
		if (PriceOverride != null)
		{
			return (decimal)PriceOverride;
		}
		return Category.Price;
	}
}
