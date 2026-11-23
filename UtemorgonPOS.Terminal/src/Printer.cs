using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using UtemorgonPOS.Core;

public static class Printer
{

	// TODO: Move into env vars
	const string PRINTER_ADDRESS = "192.168.10.119:9100";

	static ImmediateNetworkPrinter? GetPrinter()
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		return new ImmediateNetworkPrinter(settings: new() { ConnectionString = PRINTER_ADDRESS });
	}

	// Use correct byte translation for Swedish text
	static byte[] PrintSe(string text)
		=> Encoding.GetEncoding(1252).GetBytes(text + "\n");

	static byte[] AddItem(string label, double price)
		=> ItemRow(label, $"{price:F2} SEK");


	static byte[] ItemRow(string itemName, string price, int width = 42)
	{
		// Truncate item name if it's incredibly long to avoid breaking the row
		if (itemName.Length > width - price.Length - 1)
			itemName = itemName.Substring(0, width - price.Length - 1);

		string row = itemName.PadRight(width - price.Length) + price;
		return PrintSe(row);
	}

	static byte[] AggregateOrderRows(Order order)
	{
		List<byte[]> bytes = [];
		foreach (var row in order.OrderRows)
		{
			string rname = row.Quantity == 1 ? row.Name : $"{row.Name} x{row.Quantity}";
			bytes.Add(ItemRow(rname, row.GetTotalPrice().ToString()));
			if (row is RentalRow rr)
			{
				bytes.Add(PrintSe($"Returneras senast: {rr.ExpectedReturnDate:yyyy-MM-dd}"));
				if (rr.ReturnedDate is not null)
					bytes.Add(PrintSe($"Returnerad: {rr.ReturnedDate:yyyy-MM-dd}"));
			}

			foreach (var pm in row.PriceModifiers)
				bytes.Add(ItemRow($"{pm.Reason}", $"{pm.PriceDelta}  "));
		}

		return [.. bytes.SelectMany(b => b)];
	}

	static byte[] AggregateOrderModifiers(Order order)
	{
		List<byte[]> bytes = [];
		foreach (var pm in order.PriceModifiers)
			bytes.Add(ItemRow($"{pm.Reason}", $"{pm.PriceDelta}"));

		return [.. bytes.SelectMany(b => b)];
	}

	static byte[] AggregatePayments(Order order)
	{
		List<byte[]> bytes = [];
		foreach (var payment in order.Payments)
			bytes.Add(ItemRow(payment.Method.ToString(), payment.Amount.ToString()));

		return [.. bytes.SelectMany(b => b)];
	}

	public static void PrintOrder(Order order)
	{
		var printer = GetPrinter();
		var e = new EPSON();

		var receipt = ByteSplicer.Combine(
				e.Initialize(),
				[27, 116, 16], // Switch to WPC1252

				// HEADER
				e.CenterAlign(),
				e.SetStyles(PrintStyle.DoubleWidth | PrintStyle.DoubleHeight | PrintStyle.Bold),
				PrintSe("UTEMORGON"),
				e.SetStyles(PrintStyle.None),
				PrintSe("Bryggaregatan 12, 503 38 Borås"),
				PrintSe("Org.nr 202100-3138"),
				PrintSe(""),

				// METADATA
				e.LeftAlign(),
				PrintSe($"Datum: {order.CreationDate:yyyy-MM-dd HH:mm}    Kassa: 1"),
				PrintSe($"Kvittonr: #{order.Id}"),
				PrintSe($"Kund: {(order.CustomerId > 0 ? $"#{order.CustomerId}" : "Anonym")}"),
				PrintSe(new string('-', 42)), // Horizontal line

				// ITEMS
				AggregateOrderRows(order),
				PrintSe(""),

				// Modifiers
				PrintSe(new string('-', 42)), // Horizontal line
				AggregateOrderModifiers(order),
				PrintSe(""),
				PrintSe(""),

				// TOTALS
				e.RightAlign(),
				e.SetStyles(PrintStyle.DoubleWidth | PrintStyle.DoubleHeight),
				PrintSe($"TOTALT: {order.TotalCost:F2}"),
				e.SetStyles(PrintStyle.None),
				PrintSe($"Varav moms (25%): {order.TotalCost * 0.25m:F2}"),
				PrintSe(""),

				// PAYMENTS
				PrintSe(new string('-', 42)), // Horizontal line
				e.LeftAlign(),
				PrintSe("BETALNINGAR"),
				AggregatePayments(order),
				PrintSe(new string('-', 42)), // Horizontal line

				// FOOTER & BARCODE
				e.CenterAlign(),
				PrintSe("Tack för ditt besök!"),
				PrintSe("Spara kvittot vid eventuell retur."),
				e.PrintLine(""),
				e.SetBarcodeHeightInDots(100),
				e.SetBarWidth(BarWidth.Default),
				e.PrintBarcode(BarcodeType.CODE128, order.Id.ToString("D6"), BarcodeCode.CODE_C),

				e.FeedLines(5),
				e.FullCut()
			);

		printer?.WriteAsync(receipt).GetAwaiter().GetResult();
	}

	// TODO: Add real stuff in here...


	// Full on what the hell mode because the receiptprinter can't handle our barcodes.
	// Disclaimer: This is an AI generated fix.
	// It basically combines 2 barcode print-types for the special barcodes 
	// (barcodes starting with a letter followed by 12 digits)
	// This makes it more compact and allows the scanner to read it proper
	public static void PrintSpecialBarcode(string? barcode)
	{
		if (string.IsNullOrWhiteSpace(barcode))
			return;

		var printer = GetPrinter();
		var e = new EPSON();

		byte[] barcodeData = EncodeSpecialBarcode(barcode);
		byte[] gsK = [0x1D, 0x6B, 73, (byte)barcodeData.Length, .. barcodeData];

		var receipt = ByteSplicer.Combine(
			e.Initialize(),
			[27, 116, 16], // WPC1252
			e.CenterAlign(),
			e.SetBarcodeHeightInDots(100),
			e.SetBarWidth(BarWidth.Thin),
			gsK,
			PrintSe(barcode),
			e.FeedLines(5),
			e.FullCut()
		);

		printer?.WriteAsync(receipt).GetAwaiter().GetResult();
	}

	static byte[] EncodeSpecialBarcode(string barcode)
	{
		List<byte> data = [(byte)'{', (byte)'B', (byte)barcode[0], (byte)'{', (byte)'C'];

		for (int i = 1; i < barcode.Length; i += 2)
			data.Add(byte.Parse(barcode.AsSpan(i, 2)));

		return [.. data];
	}

}