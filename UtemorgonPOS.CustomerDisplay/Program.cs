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
		while (true)
		{
			Console.WriteLine("1. Visa produkter");
			Console.WriteLine("2. Lägg till produkt");
			Console.WriteLine("3. Uppdatera pris för kategori");

			int choice = int.Parse(Console.ReadLine());

			switch (choice)
			{
				case 1:
					Console.WriteLine($"{"Id",-5} | {"Name",-55} | {"Price",-10:C} | {"Lager",-10} | {"Borttagen",-10} | {"Type",-15}");
					Console.WriteLine(new string('-', 125));
					var products = Data.Products.GetProducts();
					foreach (var p in products)
					{
						Console.WriteLine($"{p.Id,-5} | {p.Name,-55} | {p.Price,-10:C} | {p.Stock,-10} | {p.IsRemoved,-10} | {p.TypeInfo,-15}");
					}
					break;
				case 2:
					Product product = new();
					Console.Write("Namn: ");
					product.Name = Console.ReadLine();
					int count = 1;
					foreach (ProductType e in Enum.GetValues<ProductType>())
					{
						Console.WriteLine($"{count++}. {e.ToString()}");
					}
					Console.Write("Välj kategori från listan: ");
					product.TypeInfo = (ProductType)int.Parse(Console.ReadLine());
					Console.Write("Välj pris: ");
					product.Price = int.Parse(Console.ReadLine());
					Console.Write("Skriv in lagersaldo: ");
					product.Stock = int.Parse(Console.ReadLine());

					Data.Products.AddProduct(product);
					break;
				case 3:
					int c = 1;
					foreach (ProductType e in Enum.GetValues<ProductType>())
					{
						Console.WriteLine($"{c++}. {e.ToString()}");
					}
					Console.Write("Välj kategori(id): ");
					int id = int.Parse(Console.ReadLine());
					Console.Write("Välj nytt pris: ");
					int amount = int.Parse(Console.ReadLine());
					Data.Products.UpdateProductCategoryPrice(id, amount);
					break;

			}

		}

	}
}