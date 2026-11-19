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

	// Edit prices, parents, etc for product categories
	public SceneTransition Run()
	{
		Console.Clear();
		Console.Write(_accumulativeMenu.ToString());

		int count = 0;
		foreach (ProductType e in Enum.GetValues<ProductType>())
		{
			Console.WriteLine($"{count++}. {e}");
		}

		Console.WriteLine("\nPress B to, or E to.");
		var pressedKey = Console.ReadKey(intercept: true).Key;

		if (pressedKey == ConsoleKey.B)
		{
			Console.WriteLine("\nPress A to , or R to.");
			pressedKey = Console.ReadKey(intercept: true).Key;
			if (pressedKey == ConsoleKey.A)
			{
				Console.WriteLine("\nPress R to , or W to.");
				if (Console.ReadKey(intercept: true).Key == ConsoleKey.R)
				{
					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
				else
				{
					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
			}
			else if (pressedKey == ConsoleKey.R)
			{
				Console.WriteLine("===Press any key to continue===");
				Console.ReadKey();
			}
		}
		else if (pressedKey == ConsoleKey.E)
		{
			Console.WriteLine("\nPress R to , or U to.");
			pressedKey = Console.ReadKey(intercept: true).Key;
			if (pressedKey == ConsoleKey.R)
			{
				Console.WriteLine("===Press any key to continue===");
				Console.ReadKey();
			}
			else if (pressedKey == ConsoleKey.U)
			{
				Console.WriteLine("\nPress S to , N to , T to , and C to");
				pressedKey = Console.ReadKey(intercept: true).Key;
				if (pressedKey == ConsoleKey.S)
				{

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
				else if (pressedKey == ConsoleKey.N)
				{

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
				else if (pressedKey == ConsoleKey.T)
				{

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
				else if (pressedKey == ConsoleKey.C)
				{

					Console.WriteLine("===Press any key to continue===");
					Console.ReadKey();
				}
			}
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
		while (!int.TryParse(Console.ReadLine(), out id));

		return id;
	}

}