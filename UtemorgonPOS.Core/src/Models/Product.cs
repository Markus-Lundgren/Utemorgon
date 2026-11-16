using System;

namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public int ProductTypeId { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public int? Stock { get; set; }
	public bool IsArchived { get; set; }

	public ProductType ProductType
	{
		get
		{
			if (!Enum.IsDefined(typeof(ProductType), ProductTypeId))
				return ProductType.None;

			return (ProductType)ProductTypeId;
		}
		set => ProductTypeId = (int)value;
	}
}
