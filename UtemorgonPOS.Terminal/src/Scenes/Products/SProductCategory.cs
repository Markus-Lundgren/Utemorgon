using System;
using System.Text;
using RiverScenes;
using UtemorgonPOS.Core;

class SProductCategory : IScene
{
	StringBuilder _accumulativeMenu = new();

	public void Enter()
	{
		_accumulativeMenu = new("=====~* UTEMORGON *~=====\nProducts > Product categories\n");
	}

	public void Exit()
	{
	}

	// Edit prices etc for product categories
	public SceneTransition Run()
	{
		Console.Clear();
		Console.Write(_accumulativeMenu.ToString());

		int count = 0;
		foreach (ProductType e in Enum.GetValues<ProductType>())
		{
			Console.WriteLine($"{(int)e}. {e}");
		}

		Console.WriteLine("\nPress E to edit category price or B to return");
		var pressedKey = Console.ReadKey(intercept: true).Key;

		if (pressedKey == ConsoleKey.E)
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

		return new SceneTransition.Pop();
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