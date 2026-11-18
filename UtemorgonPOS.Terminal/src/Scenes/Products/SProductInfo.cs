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

	public SceneTransition Run()
	{
		Console.Clear();
		Console.Write(_accumulativeMenu.ToString());

		Product? p = null;
		Barcode? b = null;

		Console.WriteLine("\nPress B to search by barcode, or I to search by id.");

		if (Console.ReadKey(intercept: true).Key == ConsoleKey.B)
		{
			string? barcode = null;
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
			b = Data.Barcodes.GetBarcodeFromString(barcode);
			p = b.Product;
		}
		else if (Console.ReadKey(intercept: true).Key == ConsoleKey.I)
		{
			int id = 0;

			while (id == 0)
			{
				Console.Write("Enter id: ");
				if (!int.TryParse(Console.ReadLine(), out id))
				{
					continue;
				}
			}

			p = Data.Products.GetProductFromId(id);
		}

		if (p == null)
		{
			Console.WriteLine("====== PRODUCT NOT FOUND ======");
		}
		else
		{
			Console.Clear();
			Console.WriteLine("====== PRODUCT INFORMATION ======");
			Console.WriteLine($"Id: {p.Id}");
			Console.WriteLine($"{p.Name} ({p.ProductType})");
			Console.WriteLine($"Price: {p.Price} SEK");
			if (!p.ProductType.ToString().StartsWith("Rental_", StringComparison.InvariantCultureIgnoreCase) &&
			!p.ProductType.ToString().StartsWith("Service_", StringComparison.InvariantCultureIgnoreCase))
				Console.WriteLine($"Stock: {p.Stock}");
			if (b != null)
				Console.WriteLine($"Barcode: {b.BarCode}");
			Console.WriteLine("=================================");

			Console.WriteLine("\nPress any key to exit.");
			Console.ReadKey();

		}
		return new SceneTransition.Pop();
	}
}