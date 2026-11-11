using System;
using System.Runtime.Intrinsics.Arm;
using UtemorgonPOS.Core;
namespace UtemorgonPOS.CustomerDisplay;

class Program
{
	static void Main(string[] args)
	{
		// var customers = Data.Customers.GetAll();

		// foreach (var c in customers)
		// 	Console.WriteLine($"{c.SSNFormated}: {c.FullName}");
		while (true)
		{
			Console.WriteLine($"{"Id",-5} | {"Name",-55} | {"Price",-10:C} | {"Lager",-10} | {"Borttagen",-10} | {"Type",-15}");
			Console.WriteLine(new string('-', 125));
			var products = Data.Products.GetProducts();
			foreach (var p in products)
			{
				Console.WriteLine($"{p.Id,-5} | {p.Name,-55} | {p.Price,-10:C} | {p.Stock,-10} | {p.IsRemoved,-10} | {p.TypeInfo,-15}");
			}

			Console.Write("Välj product du vill köpa/hyra: ");
			int id = int.Parse(Console.ReadLine());
			Console.Write("Välj mängd: ");
			int amount = int.Parse(Console.ReadLine());
			Data.Products.ProductTransaction(id, amount);
		}
	}
}