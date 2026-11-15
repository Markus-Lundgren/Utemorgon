using System;
using UtemorgonPOS.Core;

namespace UtemorgonPOS.Terminal;

class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("Hello, World!");

		// 1. Skapa en testkund
		Customer testCustomer = new Customer
		{
			SocialNumber = "199901019999",
			FName = "Test",
			LName = "Testsson",
			Address = "Testgatan 99, Göteborg",
			PhoneNumber = "0701234567"
		};

		// 2. Lägg till kunden i databasen
		Data.Customers.AddCustomer(testCustomer);

		Console.WriteLine("Test customer added!");

		// 3. Hämta kunden med personnumret för att få dess ID
		Customer? addedCustomer =
			Data.Customers.GetCustomerBySocialNumber("199901019999");

		if (addedCustomer == null)
		{
			Console.WriteLine("Could not find the test customer.");
			return;
		}

		Console.WriteLine($"Customer added with ID: {addedCustomer.Id}");

		// 4. Radera testkunden med det hämtade ID:t
		Data.Customers.DeleteCustomer(addedCustomer.Id);

		Console.WriteLine("Test customer deleted!");

		// 5. Kontrollera att kunden verkligen är borta
		Customer? deletedCustomer =
			Data.Customers.GetCustomer(addedCustomer.Id);

		Console.WriteLine(deletedCustomer == null
			? "Verification successful: customer no longer exists."
			: "Something went wrong: customer still exists.");

		Console.WriteLine($"Customer to delete: {addedCustomer.Id} - {addedCustomer.FullName}");
	}
}
