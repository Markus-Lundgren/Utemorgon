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

		bool returned = Data.OrderRows.ReturnRental(barcode) > 0;
		if (returned)
			Console.WriteLine("Rental product returned!");
		else
			Console.WriteLine("No rental unit is out with this barcode.");

		Console.WriteLine("Press any key to continue...");
		Console.ReadKey(intercept: true);

		return new SceneTransition.Pop();
	}
}