using System;
using System.Collections.Generic;
using System.Linq;
using RiverScenes;

class STransactionMain : IScene
{
	SceneTransition? _deferredTransition;

	Menu _menu = new([]);
	Menu _menuRental = new([]);
	Menu _menuWare = new([]);

	List<Transaction> _savedTransactions = [];

	// Yeah, this is a code-smell, but this will not be null. I know that for a fact.
	// We are using a state machine pattern and explicitly setting it through all paths in Enter.
#pragma warning disable CS8618
	Transaction _currentTransaction;
#pragma warning restore CS8618

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
		while (true)
		{
			Console.Clear();
			Console.WriteLine("=====~* UTEMORGON *~=====");

			// TODO: Get the current transaction from static manager and use the date to set header:
			// EG: "Transaction 2026-11-10 00:10"
			Console.WriteLine($"{_currentTransaction.AsRow()}");

			// TODO: Use current order state to render Order and Insert.

			// TODO: Check order state and render / key-query the appropriate menu.
			Console.WriteLine(_menu.MenuRows);

			var a = _menu.GetAction(Console.ReadKey(intercept: true).Key);
			if (a is null)
				Console.WriteLine("Not a valid choice!");
			else
				a?.Action();

			if (_deferredTransition != null)
				return _deferredTransition;

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
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
		new(ConsoleKey.B, "Enter barcode", () => {}),
		new(ConsoleKey.C, "Add customer", () => {}),
		new(ConsoleKey.O, "Add order modifier", () => {}),
		new(ConsoleKey.F, "Finalize order", () => {}),
		new(ConsoleKey.S, "Save order and exit", () => {_savedTransactions.Add(_currentTransaction); _deferredTransition = new SceneTransition.Pop(); }),
		new(ConsoleKey.A, "Abort transaction", () => {_deferredTransition = new SceneTransition.Pop(); }),
		]);

		_menuRental = new([
		new(ConsoleKey.B, "Enter barcode", () => {}),
		new(ConsoleKey.P, "Change root price", () => {}),
		new(ConsoleKey.M, "Add price modifier", () => {}),
		new(ConsoleKey.D, "Change return date", () => {}),
		new(ConsoleKey.E, "End scan", () => {_deferredTransition = new SceneTransition.Pop(); }),
		]);

		_menuWare = new([
		new(ConsoleKey.B, "Enter barcode", () => {}),
		new(ConsoleKey.N, "Add price modifier", () => {}),
		new(ConsoleKey.A, "Change amount", () => {}),
		new(ConsoleKey.E, "End scan", () => {_deferredTransition = new SceneTransition.Pop(); }),
		]);
	}
}