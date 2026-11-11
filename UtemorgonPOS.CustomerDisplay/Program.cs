using System;
using UtemorgonPOS.Core;
namespace UtemorgonPOS.CustomerDisplay;

class Program
{
	static void Main(string[] args)
	{
		var customers = Data.Customers.GetAll();

		var ord = Data.COrders.GetFromDate(DateOnly.FromDateTime(DateTime.Now));

		foreach (var o in ord)
		{
			Console.WriteLine(o.CreationDate);
		}
	}
}