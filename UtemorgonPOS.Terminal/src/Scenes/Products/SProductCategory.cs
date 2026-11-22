using System;
using RiverScenes;
using UtemorgonPOS.Core;

class SProductCategory : IScene
{
	SceneTransition? _deferredTransition = null;

	Menu _menu = new([]);

	SceneTransition.Pop pop = new SceneTransition.Pop();

	public void Enter()
	{
		_deferredTransition = null;

		_menu = new([
			new(ConsoleKey.P, "Set category price", ManagePrice),
			new(ConsoleKey.B, "Go back", () => _deferredTransition = pop),
		]);
	}

	public void Exit()
	{
	}

	// Edit prices etc for product categories
	public SceneTransition Run()
	{
		while (_deferredTransition is null)
		{

			Console.Clear();
			Console.WriteLine("=====~* UTEMORGON *~=====\nProducts > Product categories");

			int count = 0;
			foreach (ProductType e in Enum.GetValues<ProductType>())
			{
				Console.WriteLine($"{(int)e}. {e}");
			}

			Console.WriteLine("---------------");
			Console.WriteLine(_menu.MenuRows);


			var a = _menu.GetAction(Console.ReadKey(intercept: true).Key);

			if (a is null)
				Console.WriteLine("Not a valid choice!");
			else
				a?.Action();

		}
		return _deferredTransition;
	}

	void ManagePrice()
	{
		int id = GetId();
		decimal amount;
		do
		{
			Console.Write("Enter new category price: ");
		}
		while (!decimal.TryParse(Console.ReadLine(), out amount));

		var r = Data.Products.UpdateProductCategoryPrice(id, amount);
		if (r.Code == 1) Console.WriteLine(r.Message);
		Console.WriteLine("===Press any key to continue===");
		Console.ReadKey();
	}

	private int GetId()
	{
		int id;
		do
		{
			Console.Write("Enter product category id: ");
		}
		while (!int.TryParse(Console.ReadLine(), out id) || id <= 0 || id > Enum.GetValues<ProductType>().Length - 1);

		return id;
	}

}