/*
	This is the orders part of the Data.
	This is accessed using Data.Orders
	EG: Orders[] _ = Data.Orders.GettAllOrdersForCustomer(12); // 12 = a customer id. 
*/
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Dapper;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Orders
	{

		// Thanks to: https://www.learndapper.com/misc/transaction
		// Big up! 
		public static Order? FinalizeOrder(ITransaction transaction)
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
				AddOrderPayments(transaction.AllPayments, orderId);
				sqlTrans.Commit();

				return new Order
				{
					Id = orderId,
					CustomerId = transaction.Customer?.Id ?? 0,
					CreationDate = transaction.CreationDate,
					OrderRows = [.. transaction.AllItems],
					PriceModifiers = [.. transaction.AllModifiers],
					Payments = [.. transaction.AllPayments]
				};
			}
			catch (Exception e)
			{
				sqlTrans.Rollback();
				return null;
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
				SELECT o.*, opm.*, odrr.*, odrrpm.*, rr.*, pto.*
				FROM `Order` o 
				LEFT JOIN OrderPriceModifier opm ON opm.OrderId = o.Id 
				LEFT JOIN OrderRow odrr ON odrr.OrderId = o.Id 
				LEFT JOIN OrderRowPriceModifier odrrpm ON odrrpm.OrderRowId = odrr.Id
				LEFT JOIN RentalRow rr ON rr.OrderRowId = odrr.Id
				LEFT JOIN PaymentToOrder pto ON pto.OrderId = o.Id;
			""";

			Dictionary<int, Order> ordersDict = [];
			Dictionary<int, OrderRow> rowsDict = [];

			var orders = db.Query<Order, OrderPriceModifier, OrderRow, RowPriceModifier, RentalRow, Payment, Order>(query,
			(order, orderPriceModifier, row, rowPriceModifier, rentalRow, payment) =>
			{
				if (!ordersDict.TryGetValue(order.Id, out var currentOrder))
				{
					currentOrder = order;
					ordersDict.Add(order.Id, order);
				}

				OrderRow? currentRow = null;
				if (row is not null && !rowsDict.TryGetValue(row.Id, out currentRow))
				{
					if (rentalRow is not null)
					{
						rentalRow.RentalRowId = rentalRow.Id;
						rentalRow.Id = row.Id;
						rentalRow.OrderId = row.OrderId;
						rentalRow.Barcode = row.Barcode;
						rentalRow.Name = row.Name;
						rentalRow.Price = row.Price;
						currentRow = rentalRow;
					}
					else
						currentRow = row;

					rowsDict.Add(row.Id, currentRow);
					currentOrder.OrderRows.Add(currentRow);
				}

				if (payment is not null && !currentOrder.Payments.Any(p => p.Id == payment.Id))
					currentOrder.Payments.Add(payment);

				if (orderPriceModifier is not null && !currentOrder.PriceModifiers.Any(pm => pm.Id == orderPriceModifier.Id))
					currentOrder.PriceModifiers.Add(orderPriceModifier);

				if (rowPriceModifier is not null && currentRow is not null && !currentRow.PriceModifiers.Any(x => x.Id == rowPriceModifier.Id))
					currentRow.PriceModifiers.Add(rowPriceModifier);

				return order;
			},
			splitOn: "Id").AsList();

			return [.. ordersDict.Values];
		}

		public static Order? GetFromId(int orderId)
		{
			using var db = GetConnectionObject();

			string query = """
				SELECT o.*, opm.*, odrr.*, odrrpm.*, rr.*, pto.*
				FROM `Order` o 
				LEFT JOIN OrderPriceModifier opm ON opm.OrderId = o.Id 
				LEFT JOIN OrderRow odrr ON odrr.OrderId = o.Id 
				LEFT JOIN OrderRowPriceModifier odrrpm ON odrrpm.OrderRowId = odrr.Id
				LEFT JOIN RentalRow rr ON rr.OrderRowId = odrr.Id
				LEFT JOIN PaymentToOrder pto ON pto.OrderId = o.Id
				WHERE o.Id = @orderId;
			""";

			Order? currentOrder = null;
			Dictionary<int, OrderRow> rowsDict = [];

			db.Query<Order, OrderPriceModifier, OrderRow, RowPriceModifier, RentalRow, Payment, Order>(query,
			(order, orderPriceModifier, row, rowPriceModifier, rentalRow, payment) =>
			{
				currentOrder ??= order;

				OrderRow? currentRow = null;
				if (row is not null && !rowsDict.TryGetValue(row.Id, out currentRow))
				{
					if (rentalRow is not null)
					{
						rentalRow.RentalRowId = rentalRow.Id;
						rentalRow.Id = row.Id;
						rentalRow.OrderId = row.OrderId;
						rentalRow.Barcode = row.Barcode;
						rentalRow.Name = row.Name;
						rentalRow.Price = row.Price;
						currentRow = rentalRow;
					}
					else
						currentRow = row;

					rowsDict.Add(row.Id, currentRow);
					currentOrder.OrderRows.Add(currentRow);
				}

				if (payment is not null && !currentOrder.Payments.Any(p => p.Id == payment.Id))
					currentOrder.Payments.Add(payment);

				if (orderPriceModifier is not null && !currentOrder.PriceModifiers.Any(pm => pm.Id == orderPriceModifier.Id))
					currentOrder.PriceModifiers.Add(orderPriceModifier);

				if (rowPriceModifier is not null && currentRow is not null && !currentRow.PriceModifiers.Any(x => x.Id == rowPriceModifier.Id))
					currentRow.PriceModifiers.Add(rowPriceModifier);

				return currentOrder;
			},
			new { orderId },
			splitOn: "Id");

			return currentOrder;
		}

		// This sucks. We should do this in the query, not in RAM. Crunchtime...
		public static Order[] GetAllOutstanding()
			=> GetAll().Where(o => o.IsOutstanding).ToArray() ?? [];

		public static Order[] GetFromCustomerId(int? customerId)
		{
			using var db = GetConnectionObject();

			string query = """
				SELECT o.*, opm.*, odrr.*, odrrpm.*, rr.*, pto.*
				FROM `Order` o 
				LEFT JOIN OrderPriceModifier opm ON opm.OrderId = o.Id 
				LEFT JOIN OrderRow odrr ON odrr.OrderId = o.Id 
				LEFT JOIN OrderRowPriceModifier odrrpm ON odrrpm.OrderRowId = odrr.Id
				LEFT JOIN RentalRow rr ON rr.OrderRowId = odrr.Id
				LEFT JOIN PaymentToOrder pto ON pto.OrderId = o.Id
				WHERE o.CustomerId = @customerId;
			""";

			Dictionary<int, Order> ordersDict = [];
			Dictionary<int, OrderRow> rowsDict = [];

			var orders = db.Query<Order, OrderPriceModifier, OrderRow, RowPriceModifier, RentalRow, Payment, Order>(query,
			(order, orderPriceModifier, row, rowPriceModifier, rentalRow, payment) =>
			{
				if (!ordersDict.TryGetValue(order.Id, out var currentOrder))
				{
					currentOrder = order;
					ordersDict.Add(order.Id, order);
				}

				OrderRow? currentRow = null;
				if (row is not null && !rowsDict.TryGetValue(row.Id, out currentRow))
				{
					if (rentalRow is not null)
					{
						rentalRow.RentalRowId = rentalRow.Id;
						rentalRow.Id = row.Id;
						rentalRow.OrderId = row.OrderId;
						rentalRow.Barcode = row.Barcode;
						rentalRow.Name = row.Name;
						rentalRow.Price = row.Price;
						currentRow = rentalRow;
					}
					else
						currentRow = row;

					rowsDict.Add(row.Id, currentRow);
					currentOrder.OrderRows.Add(currentRow);
				}

				if (payment is not null && !currentOrder.Payments.Any(p => p.Id == payment.Id))
					currentOrder.Payments.Add(payment);

				if (orderPriceModifier is not null && !currentOrder.PriceModifiers.Any(x => x.Id == orderPriceModifier.Id))
					currentOrder.PriceModifiers.Add(orderPriceModifier);

				if (rowPriceModifier is not null && currentRow is not null && !currentRow.PriceModifiers.Any(x => x.Id == rowPriceModifier.Id))
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

		// TODO: Fix query to function with multiple rows
		// public static Order[] GetLate()
		// {
		// 	using var db = GetConnectionObject();
		// 	string Query =
		// 	"""
		// 		SELECT CO.*,RR.*, O.*
		// 		FROM `Order` CO
		// 		JOIN OrderRow O on CO.Id = O.OrderId
		// 		JOIN RentalRow RR on O.Id = RR.OrderRowId
		// 		where RR.ReturnedDate > RR.ExpectedReturnDate
		// 		order by CO.Id;
		// 	""";
		// 	var lateReturns = db.Query<Order, OrderRow, RentalRow, Order>(Query, (Order, OrderRow, RentalRow) =>
		// 	{
		// 		Order.OrderRows = OrderRow;
		// 		return Order;
		// 	}, splitOn: "Id").AsList().ToArray();
		// 	return lateReturns;
		// }

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
			if (orderPriceModifiers.IsDefaultOrEmpty)
				return;

			using var db = GetConnectionObject();

			var query = new StringBuilder(
				"INSERT INTO OrderPriceModifier (OrderId, PriceDelta, Reason) VALUES ");

			var parameters = new DynamicParameters();

			for (int i = 0; i < orderPriceModifiers.Length; i++)
			{
				if (i > 0)
					query.Append(", ");

				query.Append($"(@OrderId{i}, @PriceDelta{i}, @Reason{i})");

				parameters.Add($"OrderId{i}", orderId);
				parameters.Add($"PriceDelta{i}", orderPriceModifiers[i].PriceDelta);
				parameters.Add($"Reason{i}", orderPriceModifiers[i].Reason);
			}

			db.Execute(query.ToString(), parameters);
		}

		public static void AddOrderPayments(ImmutableArray<Payment> orderPayments, int orderId)
		{
			if (orderPayments.IsDefaultOrEmpty)
				return;

			using var db = GetConnectionObject();

			var query = new StringBuilder(
				"INSERT INTO PaymentToOrder (OrderId, PaymentMethodId, Amount, ReceivedDate) VALUES ");

			var parameters = new DynamicParameters();

			for (int i = 0; i < orderPayments.Length; i++)
			{
				if (i > 0)
					query.Append(", ");

				query.Append($"(@OrderId{i}, @PaymentMethodId{i}, @Amount{i}, @ReceivedDate{i})");

				parameters.Add($"OrderId{i}", orderId);
				parameters.Add($"PaymentMethodId{i}", orderPayments[i].PaymentMethodId);
				parameters.Add($"Amount{i}", orderPayments[i].Amount);
				parameters.Add($"ReceivedDate{i}", orderPayments[i].ReceivedDate);
			}

			db.Execute(query.ToString(), parameters);
		}
	}
}
