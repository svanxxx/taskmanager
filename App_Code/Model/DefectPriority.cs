using System;
using System.Collections.Generic;

public class DefectPriority : IdBasedObject
{
	private const string _ID = "idRecord";
	private const string _Desc = "Descriptor";
	private const string _idOrd = "FieldOrder";
	private const string _Tabl = "[TT_RES].[DBO].[FLDPRIOR]";
	public string ID
	{
		get { return this[_ID].ToString(); }
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
	public DefectPriority()
		: base(_Tabl, new string[] { _ID }, 1.ToString(), _ID, false)
	{
	}
	public DefectPriority(int id)
		: base(	_Tabl, new string[] {_ID, _Desc, _idOrd }, id.ToString(), _ID)
	{
	}
	public static List<DefectPriority> Enum()
	{
		List<DefectPriority> res = new List<DefectPriority>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectPriority(i));
		}
		return res;
	}
}