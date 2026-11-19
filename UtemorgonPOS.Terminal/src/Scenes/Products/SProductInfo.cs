using System;
using System.Text;
using RiverScenes;
using UtemorgonPOS.Core;

class SProductInfo : IScene
{
	StringBuilder _accumulativeMenu = new();

	public void Enter()
	{
		_accumulativeMenu = new("=====~* UTEMORGON *~=====\nProducts > Product information\n");
	}

	public void Exit()
	{
	}

	// Find, Check stock, Attach / remove barcode, remove, update etc
	public SceneTransition Run()
	{
		Console.Clear();
		Console.Write(_accumulativeMenu.ToString());

		Console.WriteLine($"{"Id",-3} | {"Name",-55} | {"Price",-15} | {"Lager",-10} | {"Borttagen",-10} | {"Type",-15} | {"Arkiverad",-10}");
		Console.WriteLine(new string('-', 125));
		var products = Data.Products.GetProducts();
		foreach (var pr in products)
		{
			// This line can be removed. Its only purpose is to filter out the removed products. 
			// However if one wants to see all products, including the archived ones, simply remove this
			if (pr.IsArchived) continue;

			Console.WriteLine($"{pr.Id,-3} | {pr.Name,-55} | {pr.Price + " SEK",-15} | {pr.Stock,-10} | {pr.IsArchived,-10} | {pr.ProductType,-15} | {pr.IsArchived,-10}");
		}

		Product? p = null;
		Barcode? b = null;

		Console.WriteLine("\nPress B handle barcodes, or E to edit product.");
		var pressedKey = Console.ReadKey(intercept: true).Key;

		if (pressedKey == ConsoleKey.B)
		{
			Console.WriteLine("\nPress A to attach a barcode to a product, or R to remove a barcode.");
			pressedKey = Console.ReadKey(intercept: true).Key;
			if (pressedKey == ConsoleKey.A)
			{
				int id = GetId();

				Console.WriteLine("\nPress R to attach a rental barcode, or W to attach a normal barcode.");
				if (Console.ReadKey(intercept: true).Key == ConsoleKey.R)
				{
					Data.Barcodes.AddBarcodeToRental(id);
				}
				else
				{
					Response r = Data.Barcodes.AddBarcodeToProduct(id, GetBarcode());
					if (r.Code == 1) Console.WriteLine(r.Message);
					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
			}
			else if (pressedKey == ConsoleKey.R)
			{
				Response r = Data.Barcodes.ArchiveBarcode(GetBarcode());
				if (r.Code == 1) Console.WriteLine(r.Message);
				Console.WriteLine("===Press any key to continue===");
				Console.ReadKey();
			}
		}
		else if (pressedKey == ConsoleKey.E)
		{
			Console.WriteLine("\nPress R to remove a product, or U to update a product.");
			pressedKey = Console.ReadKey(intercept: true).Key;
			if (pressedKey == ConsoleKey.R)
			{
				int id = GetId();
				Response r = Data.Products.DeleteProduct(id);

				if (r.Code == 1) Console.WriteLine(r.Message);
				Console.WriteLine("===Press any key to continue===");
				Console.ReadKey();
			}
			else if (pressedKey == ConsoleKey.U)
			{
				Console.WriteLine("\nPress S to update stock, N to rename a product, T to change product category, and C to change price");
				pressedKey = Console.ReadKey(intercept: true).Key;
				if (pressedKey == ConsoleKey.S)
				{
					int id = GetId();
					int amount;
					do
					{
						Console.Write("Enter new amount: ");
					}
					while (!int.TryParse(Console.ReadLine(), out amount));

					Response r = Data.Products.UpdateStock(id, amount);
					if (r.Code == 1) Console.WriteLine(r.Message);

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
				else if (pressedKey == ConsoleKey.N)
				{
					int id = GetId();

					string name = null;

					while (string.IsNullOrEmpty(name))
					{

						Console.Write("Enter new product name: ");
						name = Console.ReadLine() ?? "";
					}

					Response r = Data.Products.ChangeName(id, name);
					if (r.Code == 1) Console.WriteLine(r.Message);

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
				else if (pressedKey == ConsoleKey.T)
				{
					int count = 0;
					foreach (ProductType e in Enum.GetValues<ProductType>())
					{
						Console.WriteLine($"{count++}. {e}");
					}

					int id = GetId();

					int productId;
					do
					{
						Console.Write("Enter product category id: ");
					}
					while (!int.TryParse(Console.ReadLine(), out productId));

					Response r = Data.Products.ChangeProductType(id, (ProductType)productId);
					if (r.Code == 1) Console.WriteLine(r.Message);

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
				else if (pressedKey == ConsoleKey.C)
				{
					int id = GetId();
					decimal price;
					do
					{
						Console.Write("Enter new price: ");
					}
					while (!decimal.TryParse(Console.ReadLine(), out price));

					Response r = Data.Products.ChangePrice(id, price);
					if (r.Code == 1) Console.WriteLine(r.Message);

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
			}
		}

		return new SceneTransition.Pop();
	}

	private int GetId()
	{
		int id;
		do
		{
			Console.Write("Enter product id: ");
		}
		while (!int.TryParse(Console.ReadLine(), out id));

		return id;
	}
	private string GetBarcode()
	{
		string barcode = null;
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
		return barcode;
	}
	// Console.WriteLine("\nPress B to search by barcode, or I to search by id.");

	// if (Console.ReadKey(intercept: true).Key == ConsoleKey.B)
	// {
	// 	string? barcode = null;
	// 	while (string.IsNullOrEmpty(barcode))
	// 	{
	// 		Console.Write("Scan or enter barcode: ");
	// 		barcode = Console.ReadLine() ?? "";

	// 		if (barcode.Length != 13)
	// 		{
	// 			barcode = null;
	// 			continue;
	// 		}
	// 	}
	// 	b = Data.Barcodes.GetBarcodeFromString(barcode);
	// p = b.Product;
	// }
	// else if (Console.ReadKey(intercept: true).Key == ConsoleKey.I)
	// {
	// int id = 0;

	// while (id == 0)
	// {
	// 	Console.Write("Enter id: ");
	// 	if (!int.TryParse(Console.ReadLine(), out id))
	// 	{
	// 		continue;
	// 	}
	// }

	// p = Data.Products.GetProductFromId(id);
	// }

	// if (p == null)
	// {
	// Console.WriteLine("====== PRODUCT NOT FOUND ======");
	// }
	// else
	// {
	// Console.Clear();
	// Console.WriteLine("====== PRODUCT INFORMATION ======");
	// Console.WriteLine($"Id: {p.Id}");
	// Console.WriteLine($"{p.Name} ({p.ProductType})");
	// Console.WriteLine($"Price: {p.Price} SEK");
	// if (!p.ProductType.ToString().StartsWith("Rental_", StringComparison.InvariantCultureIgnoreCase) &&
	// !p.ProductType.ToString().StartsWith("Service_", StringComparison.InvariantCultureIgnoreCase))
	// 	Console.WriteLine($"Stock: {p.Stock}");
	// if (b != null)
	// 	Console.WriteLine($"Barcode: {b.BarCode}");
	// Console.WriteLine("=================================");

	// Console.WriteLine("\nPress any key to exit.");
	// Console.ReadKey();

	// }
}