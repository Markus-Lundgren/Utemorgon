namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public ProductType TypeInfo { get; private set; }
	public int CategoryId { get; set; }
	public int ProductTypeId { get; set; }
	public string Name { get; set; }
	public decimal? PriceOverride { get; set; }
	public int? Stock { get; set; }
	public bool IsRemoved { get; set; }
	public ProductCategory Category { get; set; }
	public decimal GetPrice()
	{
		if (PriceOverride != null)
		{
			return (decimal)PriceOverride;
		}
		return Category?.Price ?? 0m;
	}

	public void SetType()
	{
		TypeInfo = ProductTypeId switch
		{
			1 => ProductType.Ware,
			2 => ProductType.Rental,
			3 => ProductType.Service,
			_ => ProductType.None
		};
	}
}
