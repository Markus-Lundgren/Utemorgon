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
		public static int CreateOrder(int customerId, DateTime creationDate)
		{
			using var db = GetConnectionObject();
			string sql = """
				INSERT INTO COrder(CustomerId, CreationDate)
				VALUES(@CustomerId, @CreationDate)
				RETURNING Id;
				""";
			return db.QuerySingle<int>(
				sql, new { CustomerId = customerId, CreationDate = creationDate }
			);
		}
		public static Order[] GetAll()
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Order>("SELECT * FROM `COrder`;")];
		}
		public static Order[] GetFromCustomerId(int customerId)
		{
			using var db = GetConnectionObject();

			return [.. db.Query<Order>("SELECT * FROM `COrder` WHERE COrder.CustomerId = @customerId; ", new { customerId }).AsList()];
		}

		public static Order[] GetFromDate(DateOnly creationdate)
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Order>("Select * from COrder where CAST(CreationDate AS DATE) = @CreationDate ", new { CreationDate = creationdate })];
		}

		/// <summary>Fetches all orders between two dates<br/>Make sure start date is lower than end date</summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns>Order array</returns>
		public static Order[] GetOrdersBetweenDates(DateOnly startDate, DateOnly endDate)
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM COrder o WHERE o.CreationDate BETWEEN @StartDate AND @EndDate";
			return [.. db.Query<Order>(query, new { StartDate = startDate, EndDate = endDate })];
		}
		public static Order[] GetLate()
		{
			using var db = GetConnectionObject();
			string Query =
			"""
				SELECT CO.*,RR.*, O.*
				FROM COrder CO
				JOIN OrderRow O on CO.Id = O.COrderId
				JOIN RentalRow RR on O.Id = RR.OrderRowId
				where RR.ReturnedDate > RR.ExpectedReturnDate
				order by CO.Id;
			""";
			var lateReturns = db.Query<Order, OrderRow, RentalRow, Order>(Query, (COrder, OrderRow, RentalRow) =>
			{
				COrder.OrderRow = OrderRow;
				return COrder;
			}, splitOn: "Id").AsList().ToArray();
			return lateReturns;
		}

		public static int CreateOrderPriceModifier(int orderId, decimal priceDelta, string reason)
		{
			using var db = GetConnectionObject();
			string query =
			"""
				INSERT INTO OrderPriceModifier(OrderId, PriceDelta, Reason)
				VALUES(@orderId, @priceDelta, @reason)
				RETURNING Id;
			""";
			return db.QuerySingle<int>(query, new { orderId, priceDelta, reason });
		}

		public static int AddPaymentToOrder(int orderId, decimal amount, PaymentMethod paymentMethod, DateTime receivedDate)
		{
			using var db = GetConnectionObject();
			string query =
			"""
				INSERT INTO PaymentToOrder(OrderId, Amount, PaymentMethod, ReceivedDate)
				VALUES(@orderId, @amount, @PaymentMethod, @receivedDate)
				RETURNING
			""";

			return db.QuerySingle<int>(query, new
			{
				orderId,
				amount,
				PaymentMethod = paymentMethod.ToString(),
				receivedDate
			});
		}

	}
}
