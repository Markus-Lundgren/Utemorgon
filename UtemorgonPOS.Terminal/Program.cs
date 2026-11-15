using System;
using UtemorgonPOS.Core;

namespace UtemorgonPOS.Terminal;

class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("Hello, World!");

		LateRental[] customers = Data.Customers.GetCustomersWithLateRentals();

		foreach (LateRental customer in customers)
		{
			Console.WriteLine($"Product: {customer.MovieName} | ReturnDate: {customer.ExpectedReturnDate} \nCustomer: {customer.FullName}, {customer.PhoneNumber}");
		}
	}
}
