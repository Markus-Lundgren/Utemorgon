using System;
using RiverScenes;
using UtemorgonPOS.Core;

namespace UtemorgonPOS.Terminal;

class Program
{
	static void Main(string[] _)
	{
		Console.Clear();
		SceneManager sm = new();

		Console.WriteLine($"SENT TO LABEL PRINTER: {Data.Barcodes.AddBarcodeToRental(2)}");

		Console.WriteLine($"Press any key to continue.");
		Console.ReadKey();



		Console.WriteLine($"------------------ All rows (grouped by order) ------------------");
		int i = 1;
		while (i++ > 0)
		{
			var rows = Data.OrderRows.GetAllRowsForOrder(i);

			if (rows.Length < 1)
				break;

			foreach (var row in rows)
				Console.WriteLine($"{row}");

			Console.WriteLine($"-------------------------------------------------------");
		}

		Console.WriteLine($"Press any key to continue.");
		Console.ReadKey();

		Console.WriteLine($"----------------------- Late movies --------------------------");
		foreach (var rr in Data.OrderRows.GetLateRentalRows())
			Console.WriteLine($"{rr}");
		Console.WriteLine($"------------------------------------------------------------");

		Console.WriteLine($"Press any key to continue.");
		Console.ReadKey();

		sm.Initialize(SceneRepository.Main);

		Console.WriteLine("End of main execution (success).");
	}
}