using System;
using RiverScenes;

class SProductsMain : IScene
{
	static SceneTransition? _deferredTransition;

	Menu _menu = new([
	new(ConsoleKey.N, "New product", () => {}),
	new(ConsoleKey.I, "Products info", () => {}), // Find, Check stock, Attach / remove barcode, remove, update etc
	new(ConsoleKey.C, "Product category info", () => {}), // Edit prices, parents, etc for product categories
	new(ConsoleKey.B, "Back", () => {_deferredTransition = new SceneTransition.Pop(); }),
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
			Console.WriteLine("Products");
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