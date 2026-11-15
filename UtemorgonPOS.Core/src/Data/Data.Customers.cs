/*
	This is the customers part of the Data.
	This is accessed using Data.Customers
	EG: Customer _ = Data.Customers.GetCustomer(12); // 12 = a customer id. 
*/

using System.Collections.Generic;
using Dapper;

namespace UtemorgonPOS.Core;

public static partial class Data
{
	public static class Customers
	{
		public static Customer[] GetAll()
		{
			using var db = GetConnectionObject();
			return [.. db.Query<Customer>("SELECT * FROM Customer;")];
		}

		// fetch customer by id
		public static Customer? GetCustomer(int id)
		{
			using var db = GetConnectionObject();
			return db.QuerySingleOrDefault<Customer>(
				"SELECT * FROM Customer WHERE Id = @Id;",
				new { Id = id }
			);
		}

		// fetch customer with ssn
		public static Customer? GetCustomerBySocialNumber(string ssn)
		{
			using var db = GetConnectionObject();
			return db.QuerySingleOrDefault<Customer>(
				"SELECT * FROM Customer WHERE SocialNumber = @SocialNumber",
				new { SocialNumber = ssn }
			);
		}

		// fetch customer by multiple ids
		public static Customer[] GetCustomersByMultipleIds(IEnumerable<int> ids)
		{
			using var db = GetConnectionObject();

			var conditions = new List<string>();
			var parameters = new DynamicParameters();

			int index = 0;
			foreach (int id in ids)
			{
				string parameterName = $"Id{index}";

				conditions.Add($"Id = @{parameterName}");
				parameters.Add(parameterName, id);

				index++;
			}
			;

			if (index == 0) return [];

			var sql = $"SELECT * FROM Customer WHERE {string.Join(" OR ", conditions)}";

			return [.. db.Query<Customer>(sql, parameters)];
		}

		// add customer
		public static void AddCustomer(Customer addCustomer)
		{
			using var db = GetConnectionObject();

			string sql =
				"""
					INSERT INTO Customer (SocialNumber, FName, LName, Address, PhoneNumber)
					VALUES (@SocialNumber, @FName, @LName, @Address, @PhoneNumber)
				""";

			db.Execute(sql, addCustomer);
		}

		// update customer
		public static void UpdateCustomer(Customer updateCustomer)
		{
			using var db = GetConnectionObject();

			db.Execute(
				"""
					UPDATE Customer 
					SET 
					SocialNumber = @SocialNumber,
					FName = @FName,
					LName = @LName,
					Address = @Address,
					PhoneNumber = @PhoneNumber
					WHERE Id = @Id;
				""",
				updateCustomer
			);
		}

		// fetch customers with late rentals
		public static LateRental[] GetCustomersWithLateRentals()
		{
			using var db = GetConnectionObject();

			var sql =
			"""
				SELECT 
					CONCAT(Customer.FName, ' ', Customer.LName) AS FullName,
					Customer.PhoneNumber,
					OrderRow.Name AS MovieName,
					RentalRow.ExpectedReturnDate
				FROM Customer
				JOIN COrder ON Customer.Id = COrder.CustomerId
				JOIN OrderRow ON COrder.Id = OrderRow.COrderId
				JOIN RentalRow ON OrderRow.Id = RentalRow.OrderRowId
				WHERE RentalRow.ExpectedReturnDate < NOW()
					AND RentalRow.ReturnedDate IS NULL
			""";

			return [.. db.Query<LateRental>(sql)];
		}
	}
}