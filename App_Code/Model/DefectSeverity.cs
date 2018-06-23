using System;
using System.Collections.Generic;

public class DefectSeverity : Reference
{
	private const string _ID = "idRecord";
	private const string _Desc = "Descriptor";
	private const string _idOrd = "FieldOrder";
	private const string _plan = "Planable";

	private const string _Tabl = "[TT_RES].[DBO].[FLDSEVER]";

	private static string[] _allCols = new string[] { _ID, _Desc, _idOrd, _plan };
	public string ID
	{
		get { return this[_ID].ToString(); }
		set { this[_ID] = value; }
	}
	public bool PLAN
	{
		get
		{
			if (this[_plan] == DBNull.Value)
			{
				return false;
			}
			return Convert.ToBoolean(this[_plan]);
		}
		set { this[_plan] = value; }
	}
	public string DESCR
	{
		get { return this[_Desc].ToString(); }
		set { this[_Desc] = value; }
	}
	public string FORDER
	{
		get { return this[_idOrd].ToString(); }
		set { this[_idOrd] = value; }
	}
	public DefectSeverity()
		: base(_Tabl, _allCols, 0.ToString(), _ID, false)
	{
	}
	public DefectSeverity(object id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public static List<DefectSeverity> Enum()
	{
		List<DefectSeverity> res = new List<DefectSeverity>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectSeverity(i));
		}
		return res;
	}
	public static List<int> EnumPlanable()
	{
		List<int> res = new List<int>();
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { string.Format(" UPPER(LEFT({0}, 1)) ", _Desc) }, new object[] { "A" }))
		{
			res.Add(i);
		}
		return res;
	}
}