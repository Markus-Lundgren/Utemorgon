using System;
using RiverScenes;
using UtemorgonPOS.Core;

class SProductReturnRentals : IScene
{
	public void Enter()
	{
	}

	public void Exit()
	{
	}

	public SceneTransition Run()
	{
		Console.WriteLine("=====~* UTEMORGON *~=====");
		Console.WriteLine("Return rental");

		string barcode = ScannerTUI.GetBarcode();

		var rental = Data.OrderRows.GetLastRentalRowFromBarcode(barcode);
		bool returned = rental is not null && Data.OrderRows.ReturnRental(barcode) > 0;

		if (returned)
		{
			Console.WriteLine("Rental product returned!");
			var order = Data.Orders.GetFromId(rental!.OrderId);
			if (order is not null)
				Printer.PrintOrder(order);
			else
				Console.WriteLine($"Could not load order #{rental.OrderId} for printing.");
		}
		else
			Console.WriteLine("No rental unit is out with this barcode.");

		Console.WriteLine("Press any key to continue...");
		Console.ReadKey(intercept: true);

		return new SceneTransition.Pop();
	}
}