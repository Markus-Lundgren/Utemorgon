using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UtemorgonPOS.Core;

public class Transaction : ITransaction
{
	const int DEFAULT_RENTAL_DAYS = 3;
	/// Used for identification in frontend flow
	public DateTime Timestamp { get; private set; }
	public decimal CurrentTotal => GetTotal();
	public decimal TotalPaid => _payments.Sum(p => p.Amount);
	public decimal CurrentDiscount => GetTotalDiscount();
	public int ItemCount => GetItemCount();

	public int? OrderId { get; set; }
	public Customer? Customer { get; set; }
	public OrderRow? SelectedItem { get; private set; }
	public ImmutableArray<OrderRow> AllItems => _items.Values.ToImmutableArray();
	public ImmutableArray<OrderPriceModifier> AllModifiers => _priceModifiers.ToImmutableArray();
	public ImmutableArray<Payment> AllPayments => _payments.ToImmutableArray();

	Dictionary<int, OrderRow> _items = [];
	List<OrderPriceModifier> _priceModifiers = [];
	List<Payment> _payments = [];

	public Transaction()
	{
		Timestamp = DateTime.Now;
	}

	public void AddPayment(Payment payment) => _payments.Add(payment);

	public void DeselectItem() => SelectedItem = null;

	public void SelectItem(Barcode bc)
	{
		if (bc.Product is null)
			return;

		if (_items.TryGetValue(bc.Product.Id, out var row))
			SelectedItem = row;
	}

	public void AddItem(Barcode barcode)
	{
		if (barcode.Product is null || barcode.Product.IsArchived)
			return;

		bool isRental = barcode.Product.ProductType.ToString().Contains("rental", StringComparison.InvariantCultureIgnoreCase);

		if (isRental && Customer is null)
		{
			Console.WriteLine("Cannot add a rental product when the order has no customer!");
			Console.WriteLine("Press any key to continue.");
			Console.ReadKey(intercept: true);
			return;
		}


		if (!_items.TryAdd(barcode.Product.Id, isRental ?
		new RentalRow()
		{
			Barcode = barcode.BarCode,
			Name = barcode.Product.Name,
			DailyFee = barcode.Product.Price,
			Price = barcode.Product.Price * DEFAULT_RENTAL_DAYS,
			ExpectedReturnDate = DateTime.Today.AddDays(DEFAULT_RENTAL_DAYS),
			ReturnedDate = null,
			Quantity = 1
		} :
		new OrderRow()
		{
			Barcode = barcode.BarCode,
			Name = barcode.Product.Name,
			Price = barcode.Product.Price,
			Quantity = 1
		}))
		{
			_items[barcode.Product.Id].Quantity += isRental ? 0 : 1;
		}

		SelectedItem = _items[barcode.Product.Id];
	}


	public void RecalculateTotalRentalPrice(RentalRow row)
		=> row.Price = row.DailyFee * row.GetReturnDaysDelta(DateTime.Today);

	public bool RemoveItem(Barcode barcode, out OrderRow? row)
	{
		if (barcode.Product is not null)
			return _items.Remove(barcode.Product.Id, out row);
		else
		{
			row = null;
			return false;
		}
	}

	public bool RemoveItem(int productId, out OrderRow? row)
		=> _items.Remove(productId, out row);

	public void RemoveZeroQuantity()
		=> _items = _items.Where(kvp => kvp.Value.Quantity > 0).ToDictionary();

	public void AddPriceModifier(string reason, decimal priceDelta)
		=> _priceModifiers.Add(new(reason, priceDelta));

	public bool RemovePriceModifier(int index)
	{
		if (index > 0 && index < _priceModifiers.Count)
		{
			_priceModifiers.RemoveAt(index);
			return true;
		}
		return false;
	}

	public string AsRow()
	{
		string? cname = Customer is not null ? $"{Customer.FullName}" : "Anonymous";
		return $"({cname}) {Timestamp} ({ItemCount}) {TotalPaid} / {CurrentTotal} SEK (Diff: {CurrentTotal - TotalPaid})";
	}

	// DISCLAIMER: We know this code is dodo.
	// This was done as crunch-time finalization

	decimal GetTotal()
	{
		decimal agg = 0;

		foreach (var pm in _priceModifiers)
			agg += pm.PriceDelta;

		foreach (var itm in _items.Values)
			agg += itm.GetTotalPrice();

		return agg;
	}

	decimal GetTotalDiscount()
	{
		decimal agg = 0;

		foreach (var pm in _priceModifiers)
			if (pm.PriceDelta < 0)
				agg += pm.PriceDelta;

		foreach (var itm in _items.Values)
			foreach (var ipm in itm.PriceModifiers)
				if (ipm.PriceDelta < 0)
					agg += ipm.PriceDelta;

		return agg;
	}

	int GetItemCount()
	{
		int agg = 0;

		foreach (var itm in _items.Values)
			agg += itm.Quantity;

		return agg;
	}

}