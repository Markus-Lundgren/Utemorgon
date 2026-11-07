/*
	This is the order item part of the Data.
	This is accessed using Data.OrderItems
	EG: 
		OrderItem[] items = Data.Orders.GetAllOrderItems(322); // 322 = an order id.
		foreach(var oi in items)
			Console.WriteLine($"{oi.Name} : {oi.Price}");

	Note that an order items total price also depends on price modifiers.
*/

public static class OrderItems
{
	// TODO: Add business logic mumbo jumbo...
}