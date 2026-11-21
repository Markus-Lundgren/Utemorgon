using System;
using System.Collections.Generic;
using UtemorgonPOS.Core;

public class Transaction
{
	/// Used for identification in frontend flow
	public DateTime Timestamp { get; private set; }
	public decimal CurrentTotal => GetTotal();
	public decimal ItemCount => _items.Count;

	public int? OrderId { get; set; }
	public Customer? Customer { get; set; }
	public OrderRow? SelectedItem { get; private set; }


	Dictionary<string, OrderRow> _items = [];
	List<OrderPriceModifier> _priceModifiers = [];

	public Transaction()
	{
		Timestamp = DateTime.Now;
	}

	public void DeselectItem() => SelectedItem = null;

	public void SelectItem(string barcode)
	{
		if (_items.TryGetValue(barcode, out var row))
			SelectedItem = row;
	}

	public void AddItem(Barcode barcode)
	{
		if (barcode.Product is null)
			return;

		if (!_items.TryAdd(barcode.BarCode, new()
		{
			Barcode = barcode.BarCode,
			Name = barcode.Product.Name,
			Price = barcode.Product.Price,
			Quantity = 1
		}))
		{
			_items[barcode.BarCode].Quantity += barcode.Product.ProductType.ToString().Contains("rental", StringComparison.InvariantCultureIgnoreCase) ?
			0 : 1;
		}

		SelectedItem = _items[barcode.BarCode];
	}

	public bool RemoveItem(Barcode barcode, out OrderRow? row)
		=> _items.Remove(barcode.BarCode, out row);

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

	public string AsRow() => $"{Timestamp} ({ItemCount}) {CurrentTotal} SEK";

	decimal GetTotal()
	{
		decimal agg = 0;

		foreach (var pm in _priceModifiers)
			agg += pm.PriceDelta;

		foreach (var itm in _items.Values)
			agg += itm.GetTotalPrice();

		return agg;
	}

}