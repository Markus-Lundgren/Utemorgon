using System;
using System.Collections.Generic;
using System.Linq;
using RiverScenes;
using UtemorgonPOS.Core;

class STransactionMain : IScene
{
	SceneTransition? _deferredTransition;

	Menu _currentMenu = null!;
	Menu _menu = new([]);
	Menu _menuRental = new([]);
	Menu _menuWare = new([]);

	List<Transaction> _savedTransactions = [];

	// This will not be null. It is initialized int Enter().
	Transaction _currentTransaction = null!;

	bool _forceReRender = false;

	public void Enter()
	{
		SetupMenus();
		SelectOrCreateTransaction();
		_deferredTransition = null;
	}

	public void Exit()
	{

	}

	public SceneTransition Run()
	{
		// Outer loop - this is the menu
		while (true)
		{
			_forceReRender = false;
			Console.Clear();
			Console.WriteLine("=====~* UTEMORGON *~=====");
			Console.WriteLine($"Transaction: {_currentTransaction.AsRow()}");

			Console.WriteLine($"----- ----- ORDER ----- -----");
			if (_currentTransaction.ItemCount < 1)
				Console.WriteLine("No items added to order.");
			else
				foreach (var row in _currentTransaction.AllItems)
					PrintRow(row);

			Console.WriteLine($"----- ----- INSERT ----- -----");
			if (_currentTransaction.SelectedItem is not null)
			{
				if (_currentTransaction.SelectedItem is RentalRow)
					_currentMenu = _menuRental;
				else
					_currentMenu = _menuWare;

				PrintItemCard(_currentTransaction.SelectedItem);
			}
			else
			{
				Console.WriteLine($"No item selected.");
				_currentMenu = _menu;
			}

			Console.WriteLine($"----- ----- ------ ----- -----");
			Console.WriteLine(_currentMenu.MenuRows);

			// SCANNER LOOP HACK
			// 
			// This hack allows us to query the scanner on the same thread.
			// We simply scan for 50ms, and then check for keys in an infinite loop.
			// Once we need to update the UI, we break the inner loop so that the outer loop can re-draw.
			// This is probably super fragile and not very good. But we need to get this ready in time for the demo.

			// Inner loop - this is the scanner
			while (true)
			{
				int currentItemCount = _currentTransaction.ItemCount;
				OrderRow? selectedItem = _currentTransaction.SelectedItem;

				Scanner.ClearBuffer();
				string? barcode = Scanner.ReadForMs(50);

				if (!string.IsNullOrEmpty(barcode))
					AddAndReRender(Data.Barcodes.GetBarcodeFromString(barcode));
				else if (Console.KeyAvailable)
				{
					MenuAction? a = _currentMenu.GetAction(Console.ReadKey(intercept: true).Key);
					if (a is MenuAction ma)
						ma.Action();
				}

				// We are dirty, re-render!
				if (_forceReRender || currentItemCount != _currentTransaction.ItemCount ||
					selectedItem != _currentTransaction.SelectedItem)
					break;

				if (_deferredTransition != null)
					return _deferredTransition;
			}
		}
	}

	void SelectOrCreateTransaction()
	{
		var transactionsbydate = _savedTransactions.OrderBy(t => t.Timestamp).ToArray() ?? [];

		if (transactionsbydate.Length <= 0)
		{
			var t = new Transaction();
			_currentTransaction = t;
			return;
		}
		while (true)
		{
			Console.Clear();
			for (int i = 0; i < transactionsbydate.Length; i++)
			{
				var t = transactionsbydate[i];
				Console.WriteLine($"{i + 1}: {t.AsRow()}");
			}

			Console.WriteLine($"N: New Transaction");
			Console.Write("Select transaction: ");

			string? inp = Console.ReadLine();

			if (inp is "N" or "n")
			{
				_currentTransaction = new();
				return;
			}

			if (int.TryParse(inp, out int tn) && tn - 1 >= 0 && tn - 1 < transactionsbydate.Length)
			{
				_currentTransaction = transactionsbydate[tn - 1];
				_savedTransactions.Remove(_currentTransaction); // O(N) ):
				return;
			}
		}
	}

	void SetupMenus()
	{
		_menu = new([
		new(ConsoleKey.B, "Enter barcode", AddProductThroughManualBarcode),
		new(ConsoleKey.C, "Add customer", () => {}),
		new(ConsoleKey.O, "Add order modifier", () => {}),
		new(ConsoleKey.F, "Finalize order", () => {}),
		new(ConsoleKey.S, "Save order and exit", () => {_savedTransactions.Add(_currentTransaction); _deferredTransition = new SceneTransition.Pop(); }),
		new(ConsoleKey.A, "Abort transaction", () => {_deferredTransition = new SceneTransition.Pop(); }),
		]);

		// If selecteditem != null && selecteditem is RentalRow
		_menuRental = new([
		new(ConsoleKey.B, "Enter barcode", AddProductThroughManualBarcode),
		new(ConsoleKey.P, "Change root price", () => {}),
		new(ConsoleKey.M, "Add price modifier", () => {}),
		new(ConsoleKey.D, "Change return date", () => {}),
		new(ConsoleKey.Escape, "Deselect item", DeselectItem),
		]);

		// If selecteditem != null
		_menuWare = new([
		new(ConsoleKey.B, "Enter barcode", () => {}),
		new(ConsoleKey.N, "Add price modifier", () => {}),
		new(ConsoleKey.A, "Change amount", () => {}),
		new(ConsoleKey.Escape, "Deselect item", DeselectItem),
		]);

		_currentMenu = _menu;
	}

	void DeselectItem() => _currentTransaction.DeselectItem();

	void AddAndReRender(Barcode? barcode)
	{
		if (barcode is not null)
			_currentTransaction.AddItem(barcode);
		_forceReRender = true;
	}

	void AddProductThroughManualBarcode()
	{
		string? barcode = null;
		while (string.IsNullOrEmpty(barcode) || barcode.Length != 13 && barcode != "/a")
		{
			Console.Write("\nType /a to abort.\nEnter barcode: ");
			barcode = Console.ReadLine();
		}

		if (barcode == "/a")
			return;

		AddAndReRender(Data.Barcodes.GetBarcodeFromString(barcode));

		_forceReRender = true;

	}

	void PrintItemCard(OrderRow row)
	{
		Console.WriteLine(row.ToString());
	}

	void PrintRow(OrderRow row)
	{
		Console.WriteLine(row.ToString());
	}

}