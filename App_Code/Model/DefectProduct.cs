using System;
using System.Collections.Generic;

public class DefectProduct : IdBasedObject
{
	private const string _ID = "idRecord";
	private const string _Desc = "Descriptor";
	private const string _idOrd = "FieldOrder";
	private const string _Tabl = "[TT_RES].[DBO].[FLDPROD]";
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
	public DefectProduct()
		: base(_Tabl, new string[] { _ID }, 1.ToString(), _ID, false)
	{
	}
	public DefectProduct(int id)
		: base(	_Tabl, new string[] {_ID, _Desc, _idOrd }, id.ToString(), _ID)
	{
	}
	public static List<DefectProduct> Enum()
	{
		List<DefectProduct> res = new List<DefectProduct>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectProduct(i));
		}
		return res;
	}
}