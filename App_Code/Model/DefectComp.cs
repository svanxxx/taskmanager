using System;
using System.Collections.Generic;

public class DefectComp : Reference
{
	private const string _ID = "idRecord";
	private const string _Desc = "Descriptor";
	private const string _idOrd = "FieldOrder";

	private const string _Tabl = "[TT_RES].[DBO].[FLDCOMP]";
	static string[] _allCols = new string[] { _ID, _Desc, _idOrd };

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
	public string FORDER
	{
		get { return this[_idOrd].ToString(); }
		set { this[_idOrd] = value; }
	}
	public DefectComp()
		: base(_Tabl, _allCols, 1.ToString(), _ID, false)
	{
	}
	public DefectComp(int id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public static List<DefectComp> Enum()
	{
		List<DefectComp> res = new List<DefectComp>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectComp(i));
		}
		return res;
	}
}