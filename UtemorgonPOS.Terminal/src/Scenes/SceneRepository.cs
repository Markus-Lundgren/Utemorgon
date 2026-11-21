static class SceneRepository
{
	public static SMain Main = new();
	public static class Products
	{
		public static SProductsMain Main = new();
		public static SNewProduct New = new();
		public static SProductInfo Info = new();
		public static SProductCategory Category = new();
	}

	public static class Transaction
	{
		public static STransactionMain Main = new();
	}

	public static class Customers
	{
		public static SCustomersMain Main = new();
		public static SCustomerInfo Info = new();
		public static SAllCustomers All = new();
		public static SCustomersById ById = new();
		public static SCustomerBySocialNumber BySocialNumber = new();
	}
}