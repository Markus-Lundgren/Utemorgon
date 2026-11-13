/*
	This is the orders part of the Data.
	This is accessed using Data.Orders
	EG: Orders[] _ = Data.Orders.GettAllOrdersForCustomer(12); // 12 = a customer id. 
*/
using System;
using Dapper;
using Microsoft.VisualBasic;
namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class COrders
	{
		// TODO: Add things like GetAllOrdersForCustomer(int customerId)
		public static void CreateOrder(int customerId, DateTime creationDate)
		{
			using var db = GetConnectionObject();
			string sql = """
				INSERT INTO COrder(CustomerId, CreationDate)
				VALUES(@CustomerId, @CreationDate)
				""";
			db.Execute(
				sql, new { CustomerId = customerId, CreationDate = creationDate }
			);
			Console.WriteLine($"Inserted {customerId} at {creationDate}");
		}
		public static COrder[] GetAll()
		{
			using var db = GetConnectionObject();
			return [.. db.Query<COrder>("SELECT * FROM `COrder`;")];
		}
		public static COrder GetFromId(int customerId)
		{

			using var db = GetConnectionObject();

			return db.QuerySingle<COrder>("""
			 SELECT * FROM `COrder`
			 WHERE COrder.Id = @customerId
			 LIMIT 1;
			 """, new { customerId });
		}

		public static COrder[] GetFromDate(DateOnly creationdate)
		{
			using var db = GetConnectionObject();
			return [.. db.Query<COrder>("Select * from COrder where CAST(CreationDate AS DATE) = @CreationDate ", new { CreationDate = creationdate })];
		}
		public static COrder[] GetLate()
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
			var lateReturns = db.Query<COrder, OrderRow, RentalRow, COrder>(Query, (COrder, OrderRow, RentalRow) =>
			{
				COrder.OrderRow = OrderRow;
				return COrder;
			}, splitOn: "Id").AsList().ToArray();
			return lateReturns;
		}

	}
}
