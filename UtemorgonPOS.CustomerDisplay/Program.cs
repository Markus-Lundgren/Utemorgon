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
			Console.WriteLine("4. Visa produkter med barcodes");
			Console.WriteLine("5. Hämnta produkt från barcode");
			Console.WriteLine("6. Lägg till en barcode till en produkt");
			Console.WriteLine("7. Return rented product");

			int choice = int.Parse(Console.ReadLine());

			switch (choice)
			{
				case 1:
					Console.WriteLine($"{"Id",-5} | {"Name",-55} | {"Price",-10:C} | {"Lager",-10} | {"Borttagen",-10} | {"Type",-15}");
					Console.WriteLine(new string('-', 125));
					var products = Data.Products.GetProducts();
					foreach (var p in products)
					{
						Console.WriteLine($"{p.Id,-5} | {p.Name,-55} | {p.Price,-10:C} | {p.Stock,-10} | {p.IsArchived,-10} | {p.TypeInfo,-15}");
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
				case 4:
					Console.WriteLine($"{"Id",-5} | {"Name",-55} | {"Barcode",-25}");
					Console.WriteLine(new string('-', 90));
					var barcodes = Data.Barcodes.GetAllProductsWithBarcode();

					foreach (var b in barcodes)
					{
						Console.WriteLine($"{b.Product.Id,-5} | {b.Product.Name,-55} | {b.BarCode,-25}");
						if (Data.Barcodes.GetRentedStatus(b))
						{
							Console.WriteLine("Rented ^^^^^");
							//7312345000051
						}
					}
					break;

				case 5:
					Console.Write("Välj barcode att söka produkt efter: ");
					string code = Console.ReadLine();

					Product pr = Data.Barcodes.GetBarcodeFromString(code).Product;

					Console.WriteLine($"{"Id",-5} | {"Name",-55} | {"Price",-10:C} | {"Lager",-10} | {"Borttagen",-10} | {"Type",-15}");
					Console.WriteLine(new string('-', 125));
					Console.WriteLine($"{pr.Id,-5} | {pr.Name,-55} | {pr.Price,-10:C} | {pr.Stock,-10} | {pr.IsArchived,-10} | {pr.TypeInfo,-15}");
					Console.WriteLine(new string('-', 125));
					break;
				case 6:
					Console.Write("Välj id: ");
					//Timmy får fixa input validation...
					Product pro = Data.Products.GetProductFromId(int.Parse(Console.ReadLine()));

					if (pro == null)
					{
						Console.WriteLine("Hittade ingen produkt med detta Id");
						continue;
					}

					Console.Write("Skriv in barcode(13 karaktärer): ");
					string barCode = Console.ReadLine()!;
					if (string.IsNullOrWhiteSpace(barCode))
					{
						Console.WriteLine("Du måste skriva något!");
						continue;
					}
					if (barCode.Length != 13)
					{
						Console.WriteLine("Barcode måste innehålle exakt 13 karaktärer!");
						continue;
					}

					Data.Barcodes.AddBarcodeToProduct(pro, barCode);
					break;
				case 7:
					Console.Write("Välj barcode att returnera: ");
					string bcode = Console.ReadLine();
					Barcode ba = Data.Barcodes.GetBarcodeFromString(bcode);

					if (Data.Barcodes.TryReturnSingleProduct(ba))
					{
						Console.WriteLine("Success");
					}
					else
					{
						Console.WriteLine("Fail");
					}
					break;
			}

		}

	}
}