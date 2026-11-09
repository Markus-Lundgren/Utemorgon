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
			return [.. db.Query<Order>("SELECT * FROM Customer;")];
		}
		public static Order GetFromId(int id)
		{

			using var db = GetConnectionObject();

			return db.QuerySingle<Order>("SELECT Customer.Id, Customer.Fname, Customer.Lname, Order.Id FROM `Order` JOIN Customer On CustomerId = Customer.Id where Customer.Id = @id;", new { Id = id });
		}
	}
}
