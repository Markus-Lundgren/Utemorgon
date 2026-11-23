using System;
using System.Data.Common;
using System.Linq;
using RiverScenes;
using UtemorgonPOS.Core;

class SCustomerById : IScene
{
	public void Enter()
	{ }

	public void Exit()
	{ }

	public SceneTransition Run()
	{
		Console.Clear();
		Console.WriteLine("=====~* UTEMORGON *~=====");
		Console.WriteLine("Customers > Customers info > Search by ID");
		Console.WriteLine();

		Console.Write("Enter customer ID: ");

		int id;
		while (!int.TryParse(Console.ReadLine(), out id))
		{
			Console.Write("Invalid ID. Enter a number: ");
		}

		try
		{
			var customer = Data.Customers.GetCustomer(id);
			Console.WriteLine();

			if (customer == null)
			{
				Console.WriteLine("No customer found.");
			}
			else
			{
				Console.WriteLine("===== CUSTOMER BY ID  =====");
				Console.WriteLine(customer.AsCard());
				Order[] orders = Data.Customers.GetOutstandingOrders(customer.Id);
				decimal debt = orders.Sum(o => o.TotalCost - o.TotalPaid);
				Console.WriteLine($"Outstanding amount: {debt} SEK");
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