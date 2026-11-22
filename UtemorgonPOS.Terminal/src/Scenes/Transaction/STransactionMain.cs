using System;
using System.Collections.Generic;
using System.Linq;
using RiverScenes;
using UtemorgonPOS.Core;

// TODO: ALL THESE
// Price modifier per product
// Price root per product
// Date for rental
// Amount for ware
// When order is finalized and we push to DB, reduce stock for wares.

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

			Console.WriteLine($"\n{"TOTAL:",-42} {_currentTransaction.CurrentTotal,10:N2} SEK");
			Console.WriteLine($"{"DISCOUNTS:",-42} {Math.Abs(_currentTransaction.CurrentDiscount),10:N2} SEK");


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
			Console.WriteLine($"\n----- ----- ORDER MODIFIERS ----- -----");
			var mods = _currentTransaction.AllModifiers;
			if (mods.Length < 1)
				Console.WriteLine("No order modifiers added.");
			else
				foreach (var om in mods)
					Console.WriteLine($"\"{om.Reason}\" {om.PriceDelta:N2}");

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
		new(ConsoleKey.C, "Add customer", AddCustomerToOrder),
		new(ConsoleKey.O, "Add order modifier", AddOrderModifier),
		new(ConsoleKey.F, "Finalize order", () => {}),
		new(ConsoleKey.S, "Save order and exit", () => {_savedTransactions.Add(_currentTransaction); _deferredTransition = new SceneTransition.Pop(); }),
		new(ConsoleKey.A, "Abort transaction", () => {_deferredTransition = new SceneTransition.Pop(); }),
		]);

		// If selecteditem != null && selecteditem is RentalRow
		_menuRental = new([
		new(ConsoleKey.B, "Enter barcode", AddProductThroughManualBarcode),
		new(ConsoleKey.R, "Change root price", () => {}),
		new(ConsoleKey.P, "Add price modifier",  AddProductModifier),
		new(ConsoleKey.D, "Change return date", () => {}),
		new(ConsoleKey.A, "Abort rental", RemoveSelected),
		new(ConsoleKey.Escape, "Deselect item", DeselectItem),
		]);

		// If selecteditem != null
		_menuWare = new([
		new(ConsoleKey.B, "Enter barcode", AddProductThroughManualBarcode),
		new(ConsoleKey.P, "Add price modifier",  AddProductModifier),
		new(ConsoleKey.A, "Change amount", ChangeAmount),
		new(ConsoleKey.Escape, "Deselect item", DeselectItem),
		]);

		_currentMenu = _menu;
	}

	void AddCustomerToOrder()
	{
		string? ssn = null;

		do
		{
			Console.Write("\nEnter SSN (12 digits): ");
			ssn = Console.ReadLine();
		} while (string.IsNullOrWhiteSpace(ssn) || ssn.Length != 12 || !ssn.All(char.IsDigit));
		// Note to self
		// ssn.All(char.IsDigit)
		// Is the same as
		// ssn.All(c => char.IsDigit(c))

		Customer? customer = Data.Customers.GetCustomerBySocialNumber(ssn);

		if (customer is null)
		{
			Console.WriteLine("Could not find a customer with that SSN.");
			Console.WriteLine("Press any key to continue.");
			Console.ReadKey(intercept: true);
		}
		else
		{
			_currentTransaction.Customer = customer;
			Console.WriteLine($"{customer.FullName} has been added to the order.");
			Console.WriteLine("Press any key to continue.");
			Console.ReadKey(intercept: true);
		}

		_forceReRender = true;
		return;
	}

	// Holy hack of hacks....
	void RemoveSelected()
	{
		var product = Data.Products.GetProductFromBarcode(_currentTransaction.SelectedItem?.Barcode ?? "");

		if (product is null)
			return;

		_currentTransaction.RemoveItem(product.Id, out var _);
		_currentTransaction.RemoveZeroQuantity();

		_forceReRender = true;
	}

	void ChangeAmount()
	{
		Console.WriteLine($"CHANGING QUANTITY OF {_currentTransaction.SelectedItem?.Name}");

		string? amountInput;
		int amount;
		do
		{
			Console.Write("Amount: ");
			amountInput = Console.ReadLine();

		} while (string.IsNullOrWhiteSpace(amountInput) || !int.TryParse(amountInput, out amount) || amount < 0);

		_currentTransaction.SelectedItem?.Quantity = amount;

		_currentTransaction.RemoveZeroQuantity();

		if (_currentTransaction.SelectedItem?.Quantity <= 0)
			_currentTransaction.DeselectItem();

		_forceReRender = true;
	}

	void AddOrderModifier()
	{
		string? reason = null;
		decimal delta;
		int mp = 1;

		Console.WriteLine("ADDING ORDER MODIFIER");

		ConsoleKey? k;
		do
		{
			Console.WriteLine("D: Add discount");
			Console.WriteLine("F: Add extra fee");
			k = Console.ReadKey(intercept: true).Key;
		}
		while (k != ConsoleKey.D && k != ConsoleKey.F);

		if (k == ConsoleKey.D)
			mp = -1;

		while (string.IsNullOrWhiteSpace(reason))
		{
			Console.Write("Reason: ");
			reason = Console.ReadLine();
		}
		string? inpt;
		do
		{
			Console.Write("Amount: ");
			inpt = Console.ReadLine();

		} while (string.IsNullOrWhiteSpace(inpt) || !decimal.TryParse(inpt, out delta));

		_currentTransaction.AddPriceModifier(reason, Math.Abs(delta) * mp);
		_forceReRender = true;
	}

	void AddProductModifier()
	{
		string? reason = null;
		decimal delta;
		int mp = 1;

		Console.WriteLine("ADDING PRODUCT MODIFIER");

		ConsoleKey? k;
		do
		{
			Console.WriteLine("D: Add discount");
			Console.WriteLine("F: Add extra fee");
			k = Console.ReadKey(intercept: true).Key;
		}
		while (k != ConsoleKey.D && k != ConsoleKey.F);

		if (k == ConsoleKey.D)
			mp = -1;

		while (string.IsNullOrWhiteSpace(reason))
		{
			Console.Write("Reason: ");
			reason = Console.ReadLine();
		}
		string? inpt;
		do
		{
			Console.Write("Amount: ");
			inpt = Console.ReadLine();

		} while (string.IsNullOrWhiteSpace(inpt) || !decimal.TryParse(inpt, out delta));

		_currentTransaction.SelectedItem?.AddPriceModifier(reason, Math.Abs(delta) * mp);
		_forceReRender = true;
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
		Console.WriteLine($"Name: {row.Name}");
		Console.WriteLine($"Quantity: {row.Quantity}");

		var pms = row.PriceModifiers;
		Console.Write($"Price modifiers: ");
		if (pms.Count < 1)
			Console.Write($"None\n");
		else
		{
			Console.WriteLine();
			foreach (var pm in row.PriceModifiers)
				Console.WriteLine($"\"{pm.Reason}\" {pm.PriceDelta:N2} SEK");
		}
	}

	void PrintRow(OrderRow row)
	{
		Console.WriteLine(row.ToString());
	}

}