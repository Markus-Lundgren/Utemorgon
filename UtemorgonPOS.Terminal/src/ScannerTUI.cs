using System;

public static class ScannerTUI
{
	/// <summary>
	/// Forces the user into a loop that requires them to provide a barcode, either through scanner or manually.
	/// </summary>
	/// <returns>A barcode of type string and length 13.</returns>
	public static string GetBarcode()
	{
		Scanner.ClearBuffer();
		string? barcode = null;

		Console.WriteLine("Scan barcode or press B to enter manually.");

		while (string.IsNullOrEmpty(barcode) || barcode.Length != 13)
		{
			barcode = Scanner.ReadForMs(50);

			if (!string.IsNullOrEmpty(barcode))
				break;
			else if (Console.KeyAvailable && Console.ReadKey(intercept: true).Key == ConsoleKey.B)
			{
				Console.Write("Enter barcode: ");
				barcode = Console.ReadLine();
			}
		}

		return barcode;
	}
}