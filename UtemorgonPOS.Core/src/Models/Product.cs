using System;

namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public ProductType ProductType { get; set; }
	public int ProductTypeId { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public int? Stock { get; set; }
	public bool IsArchived { get; set; }

	public void SetProductType(int typeId)
	{
		if (typeId < 0 || typeId >= Enum.GetValues<ProductType>().Length)
			ProductType = ProductType.None;
		else
			ProductType = (ProductType)typeId;
	}
}
