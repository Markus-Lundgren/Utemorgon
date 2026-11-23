using System;
using System.Data.Common;
using RiverScenes;
using UtemorgonPOS.Core;

class SAllOrders : IScene
{
	public void Enter()
	{ }

	public void Exit()
	{ }

	public SceneTransition Run()
	{
		Console.Clear();
		Console.WriteLine("=====~* UTEMORGON *~=====");
		Console.WriteLine("Customers > Customers info > All orders for customer");
		Console.WriteLine();

		string socialNumber = "";
		bool isValid = false;

		while (!isValid)
		{
			Console.Write("Enter personal number: ");
			socialNumber = Console.ReadLine() ?? "";

			bool onlyDigits = true;

			for (int i = 0; i < socialNumber.Length; i++)
			{
				if (socialNumber[i] < '0' || socialNumber[i] > '9')
				{
					onlyDigits = false;
					break;
				}
			}

			isValid = (socialNumber.Length == 12) && onlyDigits;

			if (!isValid)
			{
				Console.WriteLine("Invalid personal number. Try again: ");
			}
		}

		try
		{
			var customer = Data.Customers.GetCustomerBySocialNumber(socialNumber);
			Console.WriteLine();

			if (customer == null)
			{
				Console.WriteLine("No customer found.");
			}
			else
			{
				var orders = Data.Orders.GetFromCustomerId(customer.Id);
				Console.WriteLine("===== CUSTOMER BY PERSONAL NUMBER =====");
				Console.WriteLine(customer.AsCard());

				Console.WriteLine($"\nORDER HISTORY");

				foreach (var order in orders)
				{
					Console.WriteLine($"Order ID: {order.Id}");
					Console.WriteLine($"Total: {order.TotalCost}");
					Console.WriteLine($"MODIFIERS");
					foreach (var mod in order.PriceModifiers)
						Console.WriteLine($"\"{mod.Reason}\" {mod.PriceDelta}");

					Console.WriteLine($"ITEMS");
					foreach (var row in order.OrderRows)
					{
						Console.WriteLine($"{row}");
					}

					Console.WriteLine("-------------------------");
				}
				Console.WriteLine("============================");
			}
		}
		catch (DbException)
		{
			Console.WriteLine();
			Console.WriteLine("A database error occurred.");
			Console.WriteLine("The customer could not be retrieved.");
		}

		Console.WriteLine();
		Console.WriteLine("Press any key to return.");
		Console.ReadKey();

		return new SceneTransition.Pop();
	}
}