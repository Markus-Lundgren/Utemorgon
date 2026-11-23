static class SceneRepository
{
	public static SMain Main = new();
	public static class Products
	{
		public static SProductsMain Main = new();
		public static SNewProduct New = new();
		public static SProductInfo Info = new();
		public static SProductCategory Category = new();
		public static SProductReturnRentals ReturnRentals = new();
	}

	public static class Transaction
	{
		public static STransactionMain Main = new();
	}

	public static class Customers
	{
		public static SCustomersMain Main = new();
		public static SCustomersInfo Info = new();
		public static SAllCustomers All = new();
		public static SCustomerById ById = new();
		public static SCustomerBySocialNumber BySocialNumber = new();
		public static SCustomersByName ByName = new();
		public static SCustomersWithLateRentals LateRentals = new();
		public static SNewCustomer NewCustomer = new();
		public static SDeleteCustomer DeleteCustomer = new();
		public static SUpdateCustomer UpdateCustomer = new();
		public static SAllOrders AllOrders = new();
	}
}