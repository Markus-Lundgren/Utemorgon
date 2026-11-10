using System;
using UtemorgonPOS.Core;
namespace UtemorgonPOS.CustomerDisplay;

class Program
{
	static void Main(string[] args)
	{
		// var customers = Data.Customers.GetAll();

		// foreach (var c in customers)
		// 	Console.WriteLine($"{c.SSNFormated}: {c.FullName}");

		Product p = Data.Products.CreateProductFromId(1);
		Console.WriteLine($"{p.Id}, {p.Name}, {p.GetPrice}, {p.IsRemoved}, Category: {p.Category.Name}");
	}
}