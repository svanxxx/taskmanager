using System;
using System.Collections.Generic;
using System.Linq;

public class DefectPriority : Reference
{
	private const string _Tabl = "[TT_RES].[DBO].[FLDPRIOR]";
	public DefectPriority()
		: base(_Tabl, _allBaseCols, 1.ToString(), _ID, false)
	{
	}
	public DefectPriority(int id)
		: base(	_Tabl, _allBaseCols, id.ToString(), _ID)
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