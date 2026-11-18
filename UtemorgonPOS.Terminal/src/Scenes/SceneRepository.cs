static class SceneRepository
{
	public static SMain Main = new();
	public static class Products
	{
		public static SProductsMain Main = new();
		public static SNewProduct New = new();
		public static SProductInfo Info = new();
	}

	public static class Transaction
	{
		public static STransactionMain Main = new();
	}
}