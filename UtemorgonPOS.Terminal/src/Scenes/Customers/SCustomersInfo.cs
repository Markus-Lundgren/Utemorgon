using System;
using RiverScenes;

class SCustomersInfo : IScene
{
	static SceneTransition? _deferredTransition;

	Menu _menu = new([
	  new(ConsoleKey.A, "Show all customers", () => {_deferredTransition = new SceneTransition.Push(SceneRepository.Customers.All); }),
	new(ConsoleKey.I, "Search customer by ID", () => {_deferredTransition = new SceneTransition.Push(SceneRepository.Customers.ById); }),
	new(ConsoleKey.P, "Search customer by personal number", () => {_deferredTransition = new SceneTransition.Push(SceneRepository.Customers.BySocialNumber); }),
	new(ConsoleKey.N, "Search customer by name", () => {_deferredTransition = new SceneTransition.Push(SceneRepository.Customers.ByName); }),
	new(ConsoleKey.S, "Customers with late rentals", () => {_deferredTransition = new SceneTransition.Push(SceneRepository.Customers.LateRentals); }),
	new(ConsoleKey.O, "All orders for a customer", () => {_deferredTransition = new SceneTransition.Push(SceneRepository.Customers.AllOrders); }),
	new(ConsoleKey.B, "Back", () => {_deferredTransition = new SceneTransition.Pop(); }),
  ]);

	public void Enter()
	{
		_deferredTransition = null;
	}

	public void Exit()
	{ }

	public SceneTransition Run()
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine("=====~* UTEMORGON *~=====");
			Console.WriteLine("Customers > Customer info");
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