using System;
using RiverScenes;

class SMain : IScene
{
	static SceneTransition? _deferredTransition;

	Menu _menu = new([
		new(ConsoleKey.P, "Product menu", () => { _deferredTransition = new SceneTransition.Push(SceneRepository.ProductMain); }),
		new(ConsoleKey.C, "Customer menu", () => {}),
		new(ConsoleKey.T, "Enter transaction", () => {}),
		new(ConsoleKey.R, "Rental menu", () => {}),
		new(ConsoleKey.B, "Manage barcodes", () => {}),
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