using System;
using System.Collections.Generic;

public class DefectProduct : Reference
{
	private static string[] _allCols = new string[] { _ID, _Desc, _idOrd };
	private const string _Tabl = "[TT_RES].[DBO].[FLDPROD]";

	public DefectProduct()
		: base(_Tabl, _allCols, 1.ToString(), _ID, false)
	{
	}
	public DefectProduct(int id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
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