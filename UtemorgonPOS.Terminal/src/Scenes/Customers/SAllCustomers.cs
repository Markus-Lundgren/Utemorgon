using System;
using System.Data.Common;
using System.Linq;
using RiverScenes;
using UtemorgonPOS.Core;

class SAllCustomers : IScene
{
	public void Enter()
	{ }

	public void Exit()
	{ }

	public SceneTransition Run()
	{
		Console.Clear();
		Console.WriteLine("=====~* UTEMORGON *~=====");
		Console.WriteLine("Customers > Customers info > All customers");
		Console.WriteLine();

		bool hasCustomers = false;

		try
		{
			foreach (var customer in Data.Customers.GetAll())
			{
				hasCustomers = true;

				Console.WriteLine("===== CUSTOMER DETAILS =====");
				Console.WriteLine(customer.AsCard());
				Order[] orders = Data.Customers.GetOutstandingOrders(customer.Id);
				decimal debt = orders.Sum(o => o.TotalCost - o.TotalPaid);
				Console.WriteLine($"Outstanding amount: {debt} SEK");
				Console.WriteLine("============================");
				Console.WriteLine();
			}

			if (!hasCustomers) Console.WriteLine("No customers found.");
		}
		catch (DbException)
		{
			Console.WriteLine("A database error occurred.");
			Console.WriteLine("Customers could not be retrieved.");
		}

		Console.WriteLine();
		Console.WriteLine("Press any key to return.");
		Console.ReadKey();

		return new SceneTransition.Pop();
	}
}