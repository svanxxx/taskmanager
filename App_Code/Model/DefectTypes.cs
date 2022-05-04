using System;
using System.Collections.Generic;
using System.Linq;

public class DefectType : Reference
{
	private const string _Tabl = "[TT_RES].[DBO].[FLDTYPE]";

	public DefectType()
		: base(_Tabl, _allBaseCols, 1.ToString(), _ID, false)
	{
	}
	public DefectType(int id)
		: base(_Tabl, _allBaseCols, id.ToString(), _ID)
	{
	}
	public static List<DefectType> Enum()
	{
		List<DefectType> res = new List<DefectType>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectType(i));
		}
		return res;
	}
	static object _lock = new object();
	static int? _dbStatus = null;
	public static int DbType()
	{
		lock (_lock)
		{
			if (!_dbStatus.HasValue)
			{
				var rec = Enum().OrderBy(x => x.FORDER).FirstOrDefault();
				_dbStatus = rec == null ? -1 : rec.ID;
			}
			return _dbStatus.Value;
		}
	}
}