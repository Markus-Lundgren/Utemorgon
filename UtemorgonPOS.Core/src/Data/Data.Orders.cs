/*
	This is the orders part of the Data.
	This is accessed using Data.Orders
	EG: Orders[] _ = Data.Orders.GettAllOrdersForCustomer(12); // 12 = a customer id. 
*/
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Orders
	{

		// Thanks to: https://www.learndapper.com/misc/transaction
		// Big up! 
		public static Response FinalizeOrder(ITransaction transaction)
		{
			using var db = GetConnectionObject();
			db.Open();

			using var sqlTrans = db.BeginTransaction();
			try
			{
				// If customer id is -1, CreteOrder will automatically set it to null
				int orderId = CreateOrder(transaction.Customer?.Id ?? -1, DateTime.Today);
				foreach (var row in transaction.AllItems)
				{
					row.OrderId = orderId;
					if (row is RentalRow rr)
						row.Id = OrderRows.PushRentalRow(rr);
					else
						row.Id = OrderRows.PushOrderRow(row);
					OrderRows.AddOrderRowPriceModifier(row);
				}

				AddOrderPriceModifier(transaction.AllModifiers, orderId);
				sqlTrans.Commit();

				return new(0, null);
			}
			catch (Exception e)
			{
				sqlTrans.Rollback();
				return new(1, e.Message);
			}
		}

		// TODO: Add things like GetAllOrdersForCustomer(int customerId)
		public static int CreateOrder(int? customerId, DateTime creationDate)
		{
			using var db = GetConnectionObject();
			string sql = """
				INSERT INTO `Order`(CustomerId, CreationDate)
				VALUES(@CustomerId, @CreationDate)
				RETURNING Id;
				""";
			return db.QuerySingle<int>(
				sql, new { CustomerId = customerId == -1 ? null : customerId, CreationDate = creationDate }
			);
		}
		public static Order[] GetAll()
		{
			using var db = GetConnectionObject();

			string query = """
				SELECT o.*, opm.*, odrr.*, odrrpm.* 
				FROM `Order` o 
				LEFT JOIN OrderPriceModifier opm ON opm.OrderId = o.Id 
				LEFT JOIN OrderRow odrr ON odrr.OrderId = o.Id 
				LEFT JOIN OrderRowPriceModifier odrrpm ON odrrpm.OrderRowId = odrr.Id;
			""";

			Dictionary<int, Order> ordersDict = [];
			Dictionary<int, OrderRow> rowsDict = [];

			var orders = db.Query<Order, OrderPriceModifier, RowPriceModifier, OrderRow, Order>(query,
			(order, orderPriceModifier, rowPriceModifier, row) =>
			{
				if (!ordersDict.TryGetValue(order.Id, out var currentOrder))
				{
					currentOrder = order;
					ordersDict.Add(order.Id, order);
				}

				if (!rowsDict.TryGetValue(row.Id, out var currentRow))
				{
					currentRow = row;
					rowsDict.Add(row.Id, row);
				}

				if (orderPriceModifier is not null && !currentOrder.PriceModifiers.Any(x => x.Id == orderPriceModifier.Id))
					currentOrder.PriceModifiers.Add(orderPriceModifier);

				if (rowPriceModifier is not null && !currentRow.PriceModifiers.Any(x => x.Id == rowPriceModifier.Id))
					currentRow.PriceModifiers.Add(rowPriceModifier);

				return order;
			},
			splitOn: "Id").AsList();

			return [.. ordersDict.Values];
		}
		public static Order[] GetFromCustomerId(int? customerId)
		{
			using var db = GetConnectionObject();

			string query = """
				SELECT o.*, opm.*, odrr.*, odrrpm.* 
				FROM `Order` o 
				LEFT JOIN OrderPriceModifier opm ON opm.OrderId = o.Id 
				LEFT JOIN OrderRow odrr ON odrr.OrderId = o.Id 
				LEFT JOIN OrderRowPriceModifier odrrpm ON odrrpm.OrderRowId = odrr.Id
				WHERE o.CustomerId = @customerId;
			""";

			Dictionary<int, Order> ordersDict = [];
			Dictionary<int, OrderRow> rowsDict = [];

			var orders = db.Query<Order, OrderPriceModifier, RowPriceModifier, OrderRow, Order>(query,
			(order, orderPriceModifier, rowPriceModifier, row) =>
			{
				if (!ordersDict.TryGetValue(order.Id, out var currentOrder))
				{
					currentOrder = order;
					ordersDict.Add(order.Id, order);
				}

				if (!rowsDict.TryGetValue(row.Id, out var currentRow))
				{
					currentRow = row;
					rowsDict.Add(row.Id, row);
				}

				if (orderPriceModifier is not null && !currentOrder.PriceModifiers.Any(x => x.Id == orderPriceModifier.Id))
					currentOrder.PriceModifiers.Add(orderPriceModifier);

				if (rowPriceModifier is not null && !currentRow.PriceModifiers.Any(x => x.Id == rowPriceModifier.Id))
					currentRow.PriceModifiers.Add(rowPriceModifier);

				return order;
			},
			new { customerId },
			splitOn: "Id").AsList();

			return [.. ordersDict.Values];
		}

		public static Order[] GetFromDate(DateOnly creationdate)
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Order>("Select * from `Order` where CAST(CreationDate AS DATE) = @CreationDate ", new { CreationDate = creationdate })];
		}

		/// <summary>Fetches all orders between two dates<br/>Make sure start date is lower than end date</summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns>Order array</returns>
		public static Order[] GetOrdersBetweenDates(DateOnly startDate, DateOnly endDate)
		{
			using var db = GetConnectionObject();

			string query = "SELECT * FROM `Order` o WHERE o.CreationDate BETWEEN @StartDate AND @EndDate";
			return [.. db.Query<Order>(query, new { StartDate = startDate, EndDate = endDate })];
		}
		public static Order[] GetLate()
		{
			using var db = GetConnectionObject();
			string Query =
			"""
				SELECT CO.*,RR.*, O.*
				FROM `Order` CO
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

		public static void AddOrderPriceModifier(ImmutableArray<OrderPriceModifier> orderPriceModifiers, int orderId)
		{
			using var db = GetConnectionObject();

			string query = "INSERT INTO OrderPriceModifier(OrderId, PriceDelta, Reason) VALUES(@OrderId, @PriceDelta, @Reason)";

			foreach (var modifier in orderPriceModifiers)
			{
				db.Execute(query, new
				{
					OrderId = orderId,
					modifier.PriceDelta,
					modifier.Reason
				});
			}
		}
	}
}
