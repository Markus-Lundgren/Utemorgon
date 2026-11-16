using System;
using System.Text;

namespace UtemorgonPOS.Core;

public class RentalRow : OrderRow
{
	public int RentalRowId { get; set; }
	public DateTime ExpectedReturnDate { get; set; }
	public DateTime? ReturnedDate { get; set; }

	/// <summary>The amount of products bought at point of sale.</summary>
	/// <remarks>On a rental product, this is always exactly 1.</remarks>
	public override int Quantity
	{
		get => 1;
		set { /* Leaving accessible because Dapper needs it, but do nothing for Rentals! */ }
	}

	public bool IsReturned => ReturnedDate is not null;

	public int LateDays
	{
		get
		{
			DateTime actualReturn = ReturnedDate ?? DateTime.Today;

			// If we are not at the expected return date, return 0 instead of like -7.
			return Math.Max(0, (actualReturn.Date - ExpectedReturnDate.Date).Days);
		}
	}

	public override string ToString()
	{
		const int LEFT = -42;
		const int RIGHT = 10;
		StringBuilder sb = new($"{Name,LEFT} {Price,RIGHT:N2} SEK\n  Returneras senast: {ExpectedReturnDate:yyyy-MM-dd}\n");
		string rdate = !IsReturned ? "Ej returnerad." : $"Returnerad: {ReturnedDate:yyyy-MM-dd}";
		sb.Append($"  {rdate} ");
		if (LateDays > 0)
			sb.Append($"({LateDays} dagar sen.)\n");

		foreach (var pm in PriceModifiers)
			sb.AppendLine($"  {pm.Reason,LEFT + 2} {pm.PriceDelta,RIGHT - 2} SEK");
		if (PriceModifiers.Count > 0)
			sb.AppendLine($"{"Summa",LEFT} {GetTotalPrice(),RIGHT:N2} SEK");

		return sb.ToString().TrimEnd();
	}
}