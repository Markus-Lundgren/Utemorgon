namespace UtemorgonPOS.Core;

public class Product
{
	public int Id { get; set; }
	public ProductType TypeInfo { get; set; }
	public int ProductTypeId { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public int? Stock { get; set; }
	public bool IsArchived { get; set; }

	public void SetProductType()
	{
		TypeInfo = ProductTypeId switch
		{
			2 => ProductType.Rental_Old_VHS,
			3 => ProductType.Rental_New_VHS,
			4 => ProductType.Rental_Old_DVD,
			5 => ProductType.Rental_New_DVD,
			6 => ProductType.Old_VHS,
			7 => ProductType.New_VHS,
			8 => ProductType.Old_DVD,
			9 => ProductType.New_DVD,
			10 => ProductType.Rental_Game,
			11 => ProductType.Rental_Console,
			12 => ProductType.Game,
			13 => ProductType.Console,
			14 => ProductType.Snacks,
			_ => ProductType.None
		};
	}
}
