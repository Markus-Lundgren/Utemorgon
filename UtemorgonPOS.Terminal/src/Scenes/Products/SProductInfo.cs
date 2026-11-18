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

		Console.WriteLine($"{"Id",-3} | {"Name",-55} | {"Price",-15} | {"Lager",-10} | {"Borttagen",-10} | {"Type",-15}");
		Console.WriteLine(new string('-', 125));
		var products = Data.Products.GetProducts();
		foreach (var pr in products)
		{
			Console.WriteLine($"{pr.Id,-3} | {pr.Name,-55} | {pr.Price + " SEK",-15} | {pr.Stock,-10} | {pr.IsArchived,-10} | {pr.ProductType,-15}");
		}

		Product? p = null;
		Barcode? b = null;

		Console.WriteLine("\nPress B handle barcodes, or E to edit product.");

		if (Console.ReadKey(intercept: true).Key == ConsoleKey.B)
		{
			Console.WriteLine("\nPress A to attach a barcode to a product, or R to remove a barcode.");
			if (Console.ReadKey(intercept: true).Key == ConsoleKey.A)
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
					if (r.Code == 1)
					{
						Console.WriteLine(r.Message);
					}
				}
			}
			else if (Console.ReadKey(intercept: true).Key == ConsoleKey.R)
			{
				Response r = Data.Barcodes.ArchiveBarcode(GetBarcode());
				if (r.Code == 1)
				{
					Console.WriteLine(r.Message);
				}
			}
		}

		if (Console.ReadKey(intercept: true).Key == ConsoleKey.E)
		{
			Console.WriteLine("\nPress R to remove a product, or U to update a product.");

			if (Console.ReadKey(intercept: true).Key == ConsoleKey.R)
			{
				Response r = Data.Products.DeleteProduct(GetId());

				if (r.Code == 1) Console.WriteLine(r.Message);
			}
			else if (Console.ReadKey(intercept: true).Key == ConsoleKey.U)
			{
				Console.WriteLine("\nPress S to update stock, or N to rename a product.");
				if (Console.ReadKey(intercept: true).Key == ConsoleKey.S)
				{
					int id = GetId();
					int amount;
					do
					{
						Console.Write("Enter new amount: ");
					}
					while (!int.TryParse(Console.ReadLine(), out amount));

					Data.Products.UpdateStock(id, amount);
				}
				else if (Console.ReadKey(intercept: true).Key == ConsoleKey.N)
				{
					int id = GetId();

					string name = null;

					while (string.IsNullOrEmpty(name))
					{

						Console.Write("Enter new product name: ");
						name = Console.ReadLine() ?? "";
					}

					Data.Products.ChangeName(id, name);
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