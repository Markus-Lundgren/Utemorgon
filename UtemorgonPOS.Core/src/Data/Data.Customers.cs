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

		public static Customer? GetCustomer(int id)
		{
			using var db = GetConnectionObject();
			return db.QuerySingleOrDefault<Customer>(
				"SELECT * FROM Customer WHERE Id = @Id;",
				new { Id = id }
			);
		}

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

		public static Customer? GetCustomerBySocialNumber(string ssn)
		{
			using var db = GetConnectionObject();
			return db.QuerySingleOrDefault<Customer>(
				"SELECT * FROM Customer WHERE SocialNumber = @SocialNumber",
				new { SocialNumber = ssn }
			);
		}

		public static Customer[] GetCustomersByName(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return [];

			using var db = GetConnectionObject();

			var searchPattern = $"%{name.Trim()}%";

			return [.. db.Query<Customer>(
				"""
					SELECT *
					FROM Customer
					WHERE FName LIKE @SearchTerm
					OR LName LIKE @SearchTerm
					OR CONCAT(FName, ' ', LName) LIKE @SearchTerm;
				""",
				new { SearchTerm = searchPattern }
			)];
		}

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

		public static void UpdateCustomer(Customer originalCustomer, Customer updatedCustomer)
		{
			using var db = GetConnectionObject();

			var updates = new List<string>();
			var parameters = new DynamicParameters();

			parameters.Add("Id", originalCustomer.Id);

			if (originalCustomer.SocialNumber != updatedCustomer.SocialNumber)
			{
				updates.Add("SocialNumber = @SocialNumber");
				parameters.Add("SocialNumber", updatedCustomer.SocialNumber);
			}

			if (originalCustomer.FName != updatedCustomer.FName)
			{
				updates.Add("FName = @FName");
				parameters.Add("FName", updatedCustomer.FName);
			}

			if (originalCustomer.LName != updatedCustomer.LName)
			{
				updates.Add("LName = @LName");
				parameters.Add("LName", updatedCustomer.LName);
			}

			if (originalCustomer.Address != updatedCustomer.Address)
			{
				updates.Add("Address = @Address");
				parameters.Add("Address", updatedCustomer.Address);
			}

			if (originalCustomer.PhoneNumber != updatedCustomer.PhoneNumber)
			{
				updates.Add("PhoneNumber = @PhoneNumber");
				parameters.Add("PhoneNumber", updatedCustomer.PhoneNumber);
			}

			if (updates.Count == 0) return;

			string sql = $"UPDATE Customer SET {string.Join(", ", updates)} WHERE Id = @Id;";

			db.Execute(sql, parameters);
		}

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
				JOIN OrderRow ON COrder.Id = OrderRow.OrderId
				JOIN RentalRow ON OrderRow.Id = RentalRow.OrderRowId
				WHERE RentalRow.ExpectedReturnDate < NOW()
					AND RentalRow.ReturnedDate IS NULL
			""";

			return [.. db.Query<LateRental>(sql)];
		}

		///<remarks>This scrambles the columns, it does not delete a row in the database.</remarks>
		public static void DeleteCustomer(int id)
		{
			using var db = GetConnectionObject();

			db.Execute(
				"""
					UPDATE Customer
					SET SocialNumber = NULL,
						FName = 'DELETED',
						LName = 'CUSTOMER',
						Address = '',
						PhoneNumber = ''
					WHERE Id = @Id;
				""",
				new { Id = id }
			);
		}
	}
}