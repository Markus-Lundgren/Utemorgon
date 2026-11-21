using System;
using RiverScenes;

class STransactionMain : IScene
{
	SceneTransition? _deferredTransition;

	Menu _menu = new([]);
	Menu _menuRental = new([]);
	Menu _menuWare = new([]);

	//TODO: Add state cache: 
	// CurrentTransaction
	// Selected product

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
			Console.WriteLine("Transaction");

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
		// TODO: If transaction list is not empty, choose transaction
		// Else: Create one automatically using current datetime.
	}

	void SetupMenus()
	{
		_menu = new([
		new(ConsoleKey.B, "Enter barcode", () => {}),
		new(ConsoleKey.C, "Add customer", () => {}),
		new(ConsoleKey.O, "Add order modifier", () => {}),
		new(ConsoleKey.F, "Finalize order", () => {}),
		new(ConsoleKey.S, "Save order and exit", () => {Console.WriteLine("Order saved (not rly)"); Console.ReadKey(); _deferredTransition = new SceneTransition.Pop(); }),
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