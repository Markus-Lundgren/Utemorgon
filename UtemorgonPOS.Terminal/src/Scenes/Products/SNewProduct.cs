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

		Console.WriteLine("\nB: Append a barcode\nEnter: Finalize.");
		ConsoleKey k = ConsoleKey.None;

		while (k != ConsoleKey.B && k != ConsoleKey.Enter)
			k = Console.ReadKey(intercept: true).Key;

		string? barcode = null;
		bool error = false;
		if (k == ConsoleKey.B)
		{
			if (p.Stock is not null)
			{

				barcode = ScannerTUI.GetBarcode();
				var response = Data.Barcodes.AddBarcodeToProduct(p.Id, barcode);
				if (response.Code != 0)
				{
					Console.Clear();
					Console.WriteLine(response.Message);
					error = true;
				}
			}
			else
			{
				if (p.ProductType.ToString().StartsWith("Rental_", StringComparison.InvariantCultureIgnoreCase))
				{
					var bc = Data.Barcodes.AddBarcodeToRental(p.Id);
					if (bc is not null)
						Printer.PrintBarcode(bc);
				}
				if (p.ProductType.ToString().StartsWith("Service_", StringComparison.InvariantCultureIgnoreCase))
				{
					var bc = Data.Barcodes.AddBarcodeToService(p.Id);
					if (bc is not null)
						Printer.PrintBarcode(bc);
				}
			}
		}

		if (!error)
		{
			Console.Clear();
			Console.WriteLine("======= NEW PRODUCT ADDED =======");
			Console.WriteLine($"{p.Name} ({p.ProductType})");
			Console.WriteLine($"Price: {p.Price} SEK");
			if (p.Stock is not null)
				Console.WriteLine($"Stock: {p.Stock}");
			if (!string.IsNullOrEmpty(barcode))
				Console.WriteLine($"Barcode: {barcode}");
			Console.WriteLine("=================================");
		}

		Console.WriteLine("Press any key to exit.");
		Console.ReadKey();

		return new SceneTransition.Pop();
	}
}