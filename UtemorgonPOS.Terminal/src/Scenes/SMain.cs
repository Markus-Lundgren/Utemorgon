using System;
using RiverScenes;

class SMain : IScene
{
	static SceneTransition? _deferredTransition;

	Menu _menu = new([
		new(ConsoleKey.T, "Enter transaction", () => { _deferredTransition = new SceneTransition.Push(SceneRepository.Transaction.Main); }),
		new(ConsoleKey.C, "Customers", () => {}),
		new(ConsoleKey.P, "Products", () => { _deferredTransition = new SceneTransition.Push(SceneRepository.Products.Main); }),
		new(ConsoleKey.R, "Rentals", () => {}),
		new(ConsoleKey.B, "Barcodes", () => {}),
	]);

	public void Enter()
	{
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
}