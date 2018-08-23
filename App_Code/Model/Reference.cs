using System;

public class ReferenceVersion
{
	private static string _refsid = "";
	private static Object _lock = new Object();

	public static string REFSVERSION()
	{
		lock (_lock)
		{
			if (string.IsNullOrEmpty(_refsid))
			{
				_refsid = DBHelper.GetValue("SELECT [VALUE] FROM [TT_RES].[DBO].[SETTINGS] WHERE [NAME] = 'REFID'").ToString();
			}
			return _refsid;
		}
	}
	public static void Updatekey()
	{
		lock (_lock)
		{
			string val = System.Guid.NewGuid().ToString();
			DBHelper.SQLExecute(string.Format("UPDATE [TT_RES].[DBO].[SETTINGS] SET [VALUE] = '{0}' WHERE [NAME] = 'REFID'", val));
			_refsid = val;
		}
	}
}

public class Reference : IdBasedObject
{
	protected const string _ID = "idRecord";
	protected const string _Desc = "Descriptor";
	protected const string _idOrd = "FieldOrder";
	protected static string[] _allBaseCols = new string[] { _ID, _Desc, _idOrd };

	public int ID
	{
		get { return Convert.ToInt32(this[_ID]); }
		set { this[_ID] = value; }
	}
	public string DESCR
	{
		get { return this[_Desc].ToString(); }
		set { this[_Desc] = value; }
	}
	public int FORDER
	{
		get { return Convert.ToInt32(this[_idOrd]); }
		set { this[_idOrd] = value; }
	}

	protected Reference(string table, string[] columns, string id, string pcname = "ID", bool doload = true)
		: base(table, columns, id, pcname, doload)
	{ }
	protected override void PostStore()
	{
		ReferenceVersion.Updatekey();
	}
}