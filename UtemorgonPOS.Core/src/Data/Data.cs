/*  
	This is the root data class. This handles things like the mariadb connection and lazy initialization.
	It is made partial so that nesting of Data.Customers, Data.Orders etc is easier and more intuitive. 
*/

namespace UtemorgonPOS.Core;

public static class Data
{
	// TODO: Add MariaDB connector with login inside a .env file.
	// TODO: Add GetConnection() which returns a new connection object (used by business classes)
	// Do not keep the connection object alive. We use the connection and kill it from the business logic caller.

	public static Customers Customers = new();
	public static Orders Orders = new();
	public static OrderItems OrderItems = new();
}