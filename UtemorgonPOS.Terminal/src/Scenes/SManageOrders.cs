using System;
using RiverScenes;
using UtemorgonPOS.Core;

class SManageOrders : IScene
{
	SceneTransition? _deferredTransition;

	Menu _menuNoOrder = new([]);
	Menu _menuOrder = new([]);
	Menu _currentMenu = null!;

	Order? _currentOrder;
	bool _forceReRender = false;

	public void Enter()
	{
		SetupMenus();
		_currentOrder = null;
		_deferredTransition = null;
	}

	public void Exit()
	{
	}

	public SceneTransition Run()
	{
		while (true)
		{
			_forceReRender = false;
			Console.Clear();
			Console.WriteLine("=====~* UTEMORGON *~=====");
			Console.WriteLine("Manage Orders (Scan receipt barcode or enter Order ID)");
			Console.WriteLine();

			if (_currentOrder is null)
			{
				Console.WriteLine("No order loaded.");
				_currentMenu = _menuNoOrder;
			}
			else
			{
				Console.WriteLine(_currentOrder.AsCard());
				Console.WriteLine();
				Console.WriteLine("- PAYMENTS");
				if (_currentOrder.Payments.Count < 1)
					Console.WriteLine("No payments registered.");
				else
					foreach (var p in _currentOrder.Payments)
						Console.WriteLine($"  {p.ReceivedDate:yyyy-MM-dd HH:mm}  {p.Method,-12} {p.Amount,10:N2} SEK");

				_currentMenu = _menuOrder;
			}

			Console.WriteLine("\n----- ----- ------ ----- -----");
			Console.WriteLine(_currentMenu.MenuRows);

			// Scanner + keyboard loop
			while (true)
			{
				Scanner.ClearBuffer();
				string? scanned = Scanner.ReadForMs(50);

				if (!string.IsNullOrWhiteSpace(scanned))
				{
					LoadOrderFromString(scanned.Trim());
				}
				else if (Console.KeyAvailable)
				{
					MenuAction? a = _currentMenu.GetAction(Console.ReadKey(intercept: true).Key);
					if (a is MenuAction ma)
						ma.Action();
				}

				if (_deferredTransition is not null)
					return _deferredTransition;

				if (_forceReRender)
					break;
			}
		}
	}

	void SetupMenus()
	{
		_menuNoOrder = new([
			new(ConsoleKey.O, "Enter Order ID", EnterOrderIdManually),
			new(ConsoleKey.Escape, "Back", () => _deferredTransition = new SceneTransition.Pop()),
		]);

		_menuOrder = new([
			new(ConsoleKey.P, "Add payment", AddPayment),
			new(ConsoleKey.R, "Print receipt", PrintReceipt),
			new(ConsoleKey.O, "Load another order", EnterOrderIdManually),
			new(ConsoleKey.Escape, "Back", () => _deferredTransition = new SceneTransition.Pop()),
		]);

		_currentMenu = _menuNoOrder;
	}

	void EnterOrderIdManually()
	{
		Console.Write("\nType /a to abort.\nEnter Order ID: ");
		string? input = Console.ReadLine()?.Trim();

		if (string.IsNullOrWhiteSpace(input) || input == "/a")
		{
			_forceReRender = true;
			return;
		}

		LoadOrderFromString(input);
	}

	void LoadOrderFromString(string input)
	{
		if (!int.TryParse(input, out int orderId))
		{
			Console.WriteLine($"\nInvalid Order ID: \"{input}\"");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(intercept: true);
			_forceReRender = true;
			return;
		}

		Order? order = Data.Orders.GetFromId(orderId);
		if (order is null)
		{
			Console.WriteLine($"\nNo order found with ID #{orderId}.");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(intercept: true);
		}
		else
		{
			_currentOrder = order;
		}

		_forceReRender = true;
	}

	void AddPayment()
	{
		if (_currentOrder is null)
			return;

		decimal paidAmount;
		string? price;
		string methodChoice;
		int methodId;

		string[] methods = Enum.GetNames(typeof(PaymentMethod));

		Console.WriteLine();
		for (int i = 1; i < methods.Length; i++)
			Console.WriteLine($"{i}: {methods[i]}");

		do
		{
			Console.Write("\nChoose a payment method: ");
			methodChoice = Console.ReadLine() ?? "";
		} while (!int.TryParse(methodChoice, out methodId) || methodId <= 0 || methodId > methods.Length - 1);

		do
		{
			Console.Write("\nEnter amount (SEK): ");
			price = Console.ReadLine();
		} while (!decimal.TryParse(price, out paidAmount));

		Data.Orders.AddPaymentToOrder(_currentOrder.Id, paidAmount, (PaymentMethod)methodId, DateTime.Now);

		// Reload from DB so payments and totals stay in sync
		_currentOrder = Data.Orders.GetFromId(_currentOrder.Id);
		_forceReRender = true;
	}

	void PrintReceipt()
	{
		if (_currentOrder is null)
			return;

		Printer.PrintOrder(_currentOrder);
		Console.WriteLine("\nReceipt sent to printer. Press any key to continue...");
		Console.ReadKey(intercept: true);
		_forceReRender = true;
	}
}