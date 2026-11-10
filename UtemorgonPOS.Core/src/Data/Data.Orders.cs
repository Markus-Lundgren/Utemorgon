/*
	This is the orders part of the Data.
	This is accessed using Data.Orders
	EG: Orders[] _ = Data.Orders.GettAllOrdersForCustomer(12); // 12 = a customer id. 
*/
using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Orders
	{
		// TODO: Add things like GetAllOrdersForCustomer(int customerId)
		public static Order[] GetAll()
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Order>("SELECT * FROM `Order`;")];
		}
		public static Order GetFromId(int customerId)
		{

			using var db = GetConnectionObject();

			return db.QuerySingle<Order>("""
			 SELECT * FROM `Order`
			 WHERE Order.Id = @customerId
			 LIMIT 1;
			 """, new { customerId });
		}

		// public OrderPriceModifier[] CreateOrderObject()
		// {
		// 	using var db = GetConnectionObject();

		// 	return [.. db.Query<OrderPriceModifier>("SELECT")];
		// }

	}
}
