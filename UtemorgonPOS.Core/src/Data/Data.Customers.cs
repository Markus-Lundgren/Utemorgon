/*
	This is the customers part of the Data.
	This is accessed using Data.Customers
	EG: Customer _ = Data.Customers.GetCustomer(12); // 12 = a customer id. 
*/

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
	}
}