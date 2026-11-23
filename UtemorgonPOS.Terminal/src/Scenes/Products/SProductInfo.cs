using System;
using System.Linq;
using RiverScenes;
using UtemorgonPOS.Core;

class SProductInfo : IScene
{
	SceneTransition? _deferredTransition;
	Product? _selectedProduct;

	Menu _listMenu = new([]);
	Menu _productMenu = new([]);
	Menu _barcodeMenu = new([]);
	Menu _currentMenu = new([]);

	public void Enter()
	{
		_deferredTransition = null;
		_selectedProduct = null;

		_listMenu = new([
			new(ConsoleKey.I, "Select product by ID", SelectById),
			new(ConsoleKey.S, "Select product by barcode", SelectByBarcode),
			new(ConsoleKey.Escape, "Back", () => _deferredTransition = new SceneTransition.Pop()),
		]);

		_productMenu = new([
			new(ConsoleKey.N, "Change name", RenameProduct),
			new(ConsoleKey.C, "Change category", ChangeCategory),
			new(ConsoleKey.P, "Change price", ChangePrice),
			new(ConsoleKey.S, "Update stock", UpdateStock),
			new(ConsoleKey.B, "Manage barcodes", () => _currentMenu = _barcodeMenu),
			new(ConsoleKey.D, "Toggle archive", ToggleArchive),
			new(ConsoleKey.Escape, "Back", BackToList),
		]);

		_barcodeMenu = new([
			new(ConsoleKey.A, "Attach normal barcode", AttachNormalBarcode),
			new(ConsoleKey.R, "Attach rental barcode", AttachRentalBarcode),
			new(ConsoleKey.D, "Remove barcode", RemoveBarcode),
			new(ConsoleKey.Escape, "Back", () => _currentMenu = _productMenu),
		]);

		_currentMenu = _listMenu;
	}

	public void Exit()
	{
	}

	public SceneTransition Run()
	{
		while (_deferredTransition is null)
		{
			Console.Clear();
			Console.WriteLine("=====~* UTEMORGON *~=====\nProducts > Product information\n");

			if (_selectedProduct is null)
				PrintProductList();
			else
				PrintProductCard(_selectedProduct);

			Console.WriteLine("--------------------------");
			Console.WriteLine(_currentMenu.MenuRows);

			var a = _currentMenu.GetAction(Console.ReadKey(intercept: true).Key);

			a?.Action();
		}

		return _deferredTransition;
	}

	void PrintProductList()
	{
		Console.WriteLine($"{"Id",-3} | {"Name",-45} | {"Price",-12} | {"Stock",-8} | {"Type",-15}");
		Console.WriteLine(new string('-', 95));

		foreach (var pr in Data.Products.GetProducts())
			Console.WriteLine($"{pr.Id,-3} | {$"{pr.Name} {(pr.IsArchived ? $"(A) " : null)}",-45} | {pr.Price + " SEK",-12} | {pr.Stock,-8} | {pr.ProductType,-15}");
	}

	void PrintProductCard(Product p)
	{
		string stockText = p.Stock is null ? "N/A" : $"{p.Stock} units";

		Console.WriteLine($"{"Name:",-12}{p.Name} {(p.IsArchived ? $"(Archived) " : null)}");
		Console.WriteLine($"{"Category:",-12}{p.ProductType} ({(int)p.ProductType})");
		Console.WriteLine($"{"Price:",-12}{p.Price} SEK");
		Console.WriteLine($"{"Stock:",-12}{stockText}");
		Console.WriteLine($"----- Barcodes -----");
		// Forgive me.
		var codes = Data.Barcodes.GetAllProductsWithBarcode().Where(b => b.ProductId == p.Id);
		foreach (var c in codes)
			Console.WriteLine($"{c.BarCode}{(c.IsArchived ? " (Archived)" : null)}");
	}

	void SelectById()
	{
		int id;
		do
		{
			Console.Write("Enter product id: ");
		}
		while (!int.TryParse(Console.ReadLine(), out id));

		var product = Data.Products.GetProductFromId(id);
		if (product is null)
		{
			Console.WriteLine("Product not found.");
			Pause();
			return;
		}

		_selectedProduct = product;
		_currentMenu = _productMenu;
	}

	void SelectByBarcode()
	{
		string barcode = ScannerTUI.GetBarcode();
		var result = Data.Barcodes.GetBarcodeFromString(barcode);

		if (result?.Product is null)
		{
			Console.WriteLine("No active product found for that barcode.");
			Pause();
			return;
		}

		_selectedProduct = result.Product;
		_currentMenu = _productMenu;
	}

	void BackToList()
	{
		_selectedProduct = null;
		_currentMenu = _listMenu;
	}

	void RefreshSelectedProduct()
	{
		if (_selectedProduct is not null)
			_selectedProduct = Data.Products.GetProductFromId(_selectedProduct.Id);
	}

	void RenameProduct()
	{
		string name = "";
		while (string.IsNullOrEmpty(name))
		{
			Console.Write("Enter new product name: ");
			name = Console.ReadLine() ?? "";
		}

		Response r = Data.Products.ChangeName(_selectedProduct!.Id, name);
		if (r.Code != 0)
		{
			Console.WriteLine(r.Message);
			Pause();
		}

		RefreshSelectedProduct();
	}

	void ChangeCategory()
	{
		foreach (ProductType e in Enum.GetValues<ProductType>())
		{
			if (e == ProductType.None) continue;
			Console.WriteLine($"{(int)e}. {e}");
		}

		int typeId;
		do
		{
			Console.Write("Enter product category id: ");
		}
		while (!int.TryParse(Console.ReadLine(), out typeId) || typeId <= 0 || typeId >= Enum.GetValues<ProductType>().Length);

		Response r = Data.Products.ChangeProductType(_selectedProduct!.Id, (ProductType)typeId);
		if (r.Code != 0)
		{
			Console.WriteLine(r.Message);
			Pause();
		}

		RefreshSelectedProduct();
	}

	void ChangePrice()
	{
		decimal price;
		do
		{
			Console.Write("Enter new price: ");
		}
		while (!decimal.TryParse(Console.ReadLine(), out price));

		Response r = Data.Products.ChangePrice(_selectedProduct!.Id, price);
		if (r.Code != 0)
		{
			Console.WriteLine(r.Message);
			Pause();
		}

		RefreshSelectedProduct();
	}

	void UpdateStock()
	{
		int amount;
		do
		{
			Console.Write("Enter new stock amount: ");
		}
		while (!int.TryParse(Console.ReadLine(), out amount));

		Response r = Data.Products.UpdateStock(_selectedProduct!.Id, amount);
		if (r.Code != 0)
		{
			Console.WriteLine(r.Message);
			Pause();
		}

		RefreshSelectedProduct();
	}

	void ToggleArchive()
	{
		Response r;
		if (!_selectedProduct!.IsArchived)
			r = Data.Products.ArchiveProduct(_selectedProduct!.Id);
		else
			r = Data.Products.UnarchiveProduct(_selectedProduct!.Id);

		if (r.Code != 0)
		{
			Console.WriteLine(r.Message);
			Pause();
			return;
		}

		BackToList();
	}

	void AttachNormalBarcode()
	{
		string barcode = ScannerTUI.GetBarcode();
		Response r = Data.Barcodes.AddBarcodeToProduct(_selectedProduct!.Id, barcode);

		Console.WriteLine(r.Code == 0
			? $"Attached barcode {barcode} to {_selectedProduct.Name}."
			: r.Message);
		Pause();
	}

	void AttachRentalBarcode()
	{
		string? barcode = Data.Barcodes.AddBarcodeToRental(_selectedProduct!.Id);
		Printer.PrintSpecialBarcode(barcode);
		Console.WriteLine($"Attached rental barcode {barcode} to {_selectedProduct.Name}.");
		Pause();
	}

	void RemoveBarcode()
	{
		string barcode = ScannerTUI.GetBarcode();
		Response r = Data.Barcodes.ArchiveBarcode(barcode);

		Console.WriteLine(r.Code == 0
			? $"Removed barcode {barcode}."
			: r.Message);
		Pause();
	}

	static void Pause()
	{
		Console.WriteLine("=== Press any key to continue ===");
		Console.ReadKey(intercept: true);
	}
}