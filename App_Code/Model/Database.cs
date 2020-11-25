using Microsoft.Win32;
using System.Data.SqlClient;

public class Database : tt_resEntities
{
	public Database()
	{
		SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(this.Database.Connection.ConnectionString);
		sb.IntegratedSecurity = false;
		using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\TaskManager"))
		{
			sb.Password = key.GetValue("p").ToString();
			sb.UserID = key.GetValue("u").ToString();
		}
		this.Database.Connection.ConnectionString = sb.ToString();
	}
}