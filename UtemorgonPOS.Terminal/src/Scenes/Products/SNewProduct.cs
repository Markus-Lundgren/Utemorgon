using System;
using System.Text;
using RiverScenes;
using UtemorgonPOS.Core;

class SNewProduct : IScene
{
	StringBuilder _accumulativeMenu = new();

	public void Enter()
	{
		_accumulativeMenu = new("=====~* UTEMORGON *~=====\nProducts > New product\n");
	}

	public void Exit()
	{
	}

	public SceneTransition Run()
	{

		Console.Clear();
		Console.Write(_accumulativeMenu.ToString());

		Product p = new();
		do
		{
			Console.Write("Product name: ");
			p.Name = Console.ReadLine() ?? "";
		}
		while (string.IsNullOrEmpty(p.Name));

		_accumulativeMenu.AppendLine($"Product name: {p.Name}");

		var types = Enum.GetValues<ProductType>();
		do
		{
			Console.Clear();
			Console.WriteLine("Available product types");

			for (int i = 1; i < types.Length; i++)
				Console.WriteLine($"{i}: {types[i]}");

			Console.Write("Product type: ");
			if (int.TryParse(Console.ReadLine(), out int ar))
				p.ProductTypeId = ar;
			else
				p.ProductTypeId = -1;
		}
		while (p.ProductType == ProductType.None);

		_accumulativeMenu.AppendLine($"Product type: : {p.ProductType} ({p.ProductTypeId})");

		Console.Clear();
		Console.Write(_accumulativeMenu.ToString());

		decimal? price;
		do
		{
			Console.Write("Product price: ");
			if (decimal.TryParse(Console.ReadLine(), out decimal dr))
				price = dr;
			else
				price = null;
		}
		while (price is null);

		p.Price = (decimal)price;
		_accumulativeMenu.AppendLine($"Product price: : {p.Price} SEK");

		if (p.ProductType.ToString().StartsWith("Rental_", StringComparison.InvariantCultureIgnoreCase) ||
		p.ProductType.ToString().StartsWith("Service_", StringComparison.InvariantCultureIgnoreCase))
			p.Stock = null;
		else
		{
			do
			{
				Console.Clear();
				Console.Write(_accumulativeMenu.ToString());
				Console.Write("Stock: ");
				if (int.TryParse(Console.ReadLine(), out int st))
					p.Stock = st;

			} while (p.Stock is null);
		}

		// TODO: Add error code returns from product, so we can accurately
		// send error messages to frontend.
		Data.Products.AddProduct(p);

		Console.WriteLine("\nPress B to append a barcode, or any other key to continue.");
		string? barcode = null;
		if (Console.ReadKey(intercept: true).Key == ConsoleKey.B)
		{
			while (string.IsNullOrEmpty(barcode))
			{
				Console.Write("Scan or enter barcode: ");
				barcode = Console.ReadLine() ?? "";

				if (barcode.Length != 13)
				{
					barcode = null;
					continue;
				}
			}

			Data.Barcodes.AddBarcodeToProduct(p.Id, barcode);
		}

		Console.Clear();
		Console.WriteLine("======= NEW PRODUCT ADDED =======");
		Console.WriteLine($"{p.Name} ({p.ProductType})");
		Console.WriteLine($"Price: {p.Price} SEK");
		Console.WriteLine($"Stock: {p.Stock}");
		if (!string.IsNullOrEmpty(barcode))
			Console.WriteLine($"Barcode: {barcode}");
		Console.WriteLine("=================================");

		Console.WriteLine("\nPress any key to exit.");
		Console.ReadKey();

		return new SceneTransition.Pop();
	}
}