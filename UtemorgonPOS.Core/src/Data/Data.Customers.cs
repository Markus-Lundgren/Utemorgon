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
	}
}