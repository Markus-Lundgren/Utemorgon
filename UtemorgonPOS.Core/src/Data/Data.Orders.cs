/*
	This is the orders part of the Data.
	This is accessed using Data.Orders
	EG: Orders[] _ = Data.Orders.GettAllOrdersForCustomer(12); // 12 = a customer id. 
*/
using System;
using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Orders
	{
		// TODO: Add things like GetAllOrdersForCustomer(int customerId)
		public static void CreateOrder(int customerId, DateTime creationDate)
		{
			using var db = GetConnectionObject();
			string sql = """
				INSERT INTO Order(CustomerId, CreationDate)
				VALUES(@CustomerId, @CreationDate)
				""";
			db.Execute(
				sql, new { CustomerId = customerId, CreationDate = creationDate }
			);
			Console.WriteLine($"Inserted {customerId} at {creationDate}");
		}
		public static Order[] GetAll()
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Order>("SELECT * FROM `Order`;")];
		}
		public static Order[] GetFromCustomerId(int customerId)
		{
			using var db = GetConnectionObject();

			return [.. db.Query<Order>("SELECT * FROM `Order` WHERE Order.CustomerId = @customerId; ", new { customerId }).AsList()];
		}

		public static Order[] GetFromDate(DateOnly creationdate)
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Order>("Select * from Order where CAST(CreationDate AS DATE) = @CreationDate ", new { CreationDate = creationdate })];
		}

		/// <summary>Fetches all orders between two dates<br/>Make sure start date is lower than end date</summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns>Order array</returns>
		public static Order[] GetOrdersBetweenDates(DateOnly startDate, DateOnly endDate)
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM Order o WHERE o.CreationDate BETWEEN @StartDate AND @EndDate";
			return [.. db.Query<Order>(query, new { StartDate = startDate, EndDate = endDate })];
		}
		public static Order[] GetLate()
		{
			using var db = GetConnectionObject();
			string Query =
			"""
				SELECT CO.*,RR.*, O.*
				FROM Order CO
				JOIN OrderRow O on CO.Id = O.OrderId
				JOIN RentalRow RR on O.Id = RR.OrderRowId
				where RR.ReturnedDate > RR.ExpectedReturnDate
				order by CO.Id;
			""";
			var lateReturns = db.Query<Order, OrderRow, RentalRow, Order>(Query, (Order, OrderRow, RentalRow) =>
			{
				Order.OrderRow = OrderRow;
				return Order;
			}, splitOn: "Id").AsList().ToArray();
			return lateReturns;
		}

	}
}
