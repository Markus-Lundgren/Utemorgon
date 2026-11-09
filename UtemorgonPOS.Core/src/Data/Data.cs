/*  
	This is the root data class. This handles things like the mariadb connection and lazy initialization.
	It is made partial so that nesting of Data.Customers, Data.Orders etc is easier and more intuitive. 
*/

using System.Data;
using MySqlConnector;

namespace UtemorgonPOS.Core;

public static partial class Data
{

	static string? _connectionInfo = null;
	static string ConnectionInfo => _connectionInfo ??= GetLogin();

	static string GetLogin()
	=> $"""
		Server = {EasyEnv.Get<string>("Server")};
		Port = {EasyEnv.Get<string>("Port")};
		User = {EasyEnv.Get<string>("User")};
		Password = {EasyEnv.Get<string>("Password")};
		Database = {EasyEnv.Get<string>("Database")};
	""";

	public static IDbConnection GetConnectionObject()
		=> new MySqlConnection(ConnectionInfo);

	// TODO: Add MariaDB connector with login inside a .env file.
	// TODO: Add GetConnection() which returns a new connection object (used by business classes)
	// Do not keep the connection object alive. We use the connection and kill it from the business logic caller.
}