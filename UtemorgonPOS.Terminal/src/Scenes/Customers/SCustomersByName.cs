using System;
using System.Data.Common;
using System.Linq;
using RiverScenes;
using UtemorgonPOS.Core;

class SCustomersByName : IScene
{
	public void Enter()
	{ }

	public void Exit()
	{ }

	public SceneTransition Run()
	{
		Console.Clear();
		Console.WriteLine("=====~* UTEMORGON *~=====");
		Console.WriteLine("Customers > Customers info > Search by name");
		Console.WriteLine();

		Console.Write("Enter customer name: ");
		string name = Console.ReadLine() ?? "";

		try
		{
			var customers = Data.Customers.GetCustomersByName(name);
			Console.WriteLine();

			bool hasCustomers = false;

			foreach (var customer in customers)
			{
				hasCustomers = true;
				Console.WriteLine("===== CUSTOMER BY NAME =====");
				Console.WriteLine(customer.AsCard());
				Order[] orders = Data.Customers.GetOutstandingOrders(customer.Id);
				decimal debt = orders.Sum(o => o.TotalCost - o.TotalPaid);
				Console.WriteLine($"Outstanding amount: {debt} SEK");
				Console.WriteLine("============================");
			}

			if (!hasCustomers) Console.WriteLine("No customers found.");
		}
		catch (DbException)
		{
			Console.WriteLine();
			Console.WriteLine("A database error occurred.");
			Console.WriteLine("Customers could not be retrieved.");
		}

		Console.WriteLine();
		Console.WriteLine("Press any key to return.");
		Console.ReadKey();

		return new SceneTransition.Pop();
	}
}