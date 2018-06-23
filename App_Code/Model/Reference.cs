using System;

public class ReferenceVersion
{
	private static string _refsid = System.Guid.NewGuid().ToString();
	private static Object _lock = new Object();

	public static string REFSVERSION()
	{
		lock (_lock)
		{
			return _refsid;
		}
	}
	public static void Updatekey()
	{
		lock (_lock)
		{
			_refsid = System.Guid.NewGuid().ToString();
		}
	}
}

public class Reference : IdBasedObject
{
	protected Reference(string table, string[] columns, string id, string pcname = "ID", bool doload = true)
		: base(table, columns, id, pcname, doload)
	{ }
	protected override void PostStore()
	{
		ReferenceVersion.Updatekey();
	}
}