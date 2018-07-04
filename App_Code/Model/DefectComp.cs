using System;
using System.Collections.Generic;
using System.Linq;

public class DefectComp : Reference
{
	protected const string _Vac = "Vacation";
	private const string _Tabl = "[TT_RES].[DBO].[FLDCOMP]";
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
	static int _VacationRec = -1;
	override public void Store()
	{
		_VacationRec = -1;
		base.Store();
	}
	public static int GetVacationRec()
	{
		if (_VacationRec != -1)
		{
			return _VacationRec;
		}

		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _Vac }, new object[] { 1 }))
		{
			_VacationRec = i;
			return i;
		}
		return 1;
	}
}