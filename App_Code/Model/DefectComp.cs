using System;
using System.Collections.Generic;
using System.Linq;

public class DefectComp : Reference
{
	protected const string _Vac = "Vacation";
	public static string _Tabl = "[TT_RES].[DBO].[FLDCOMP]";
	static string[] _allCols = _allBaseCols.Concat(new string[] { _Vac }).ToArray();

	public bool VACATION
	{
		get
		{
			if (this[_Vac] == DBNull.Value)
			{
				return false;
			}
			return Convert.ToBoolean(this[_Vac]);
		}
		set { this[_Vac] = value; }
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

	static object _lock = new object();
	static List<int> _VacationRec = new List<int>();
	override public void Store()
	{
		ClearCache();
		base.Store();
	}
	public static List<int> GetVacationRec()
	{
		lock (_lock)
		{
			if (_VacationRec.Count < 1)
			{
				foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _Vac }, new object[] { 1 }))
				{
					_VacationRec.Add(i);
				}
			}
			return new List<int>(_VacationRec);
		}
	}
	static void ClearCache()
	{
		lock (_lock)
		{
			_VacationRec.Clear();
		}
	}
	public static int New(string desc)
	{
		int res = Reference.New(_Tabl, desc);
		ClearCache();
		return res;
	}
}