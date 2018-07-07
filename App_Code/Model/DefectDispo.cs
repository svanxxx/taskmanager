using System;
using System.Collections.Generic;
using System.Linq;

public class DefectDispo : Reference
{
	static string _Color = "Color";
	static string _ReqWork = "RequireWork";
	static string _Working = "BeingEorked";

	static string _Tabl = "[TT_RES].[DBO].[FLDDISPO]";
	static string[] _allCols = _allBaseCols.Concat(new string[] { _Color, _ReqWork, _Working }).ToArray();

	public bool REQUIREWORK
	{
		get { return Convert.ToBoolean(this[_ReqWork]); }
		set { this[_ReqWork] = value ? 1 : 0; }
	}
	public bool WORKING
	{
		get { return Convert.ToBoolean(this[_Working]); }
		set { this[_Working] = value ? 1 : 0; }
	}
	public string COLOR
	{
		get { return this[_Color].ToString(); }
		set { this[_Color] = value; }
	}

	public DefectDispo()
		: base(_Tabl, _allCols, 1.ToString(), _ID, false)
	{
	}
	public DefectDispo(int id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public static List<DefectDispo> Enum()
	{
		List<DefectDispo> res = new List<DefectDispo>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectDispo(i));
		}
		return res;
	}
	static List<int> _gWorkable;
	static Object thisLock = new Object();
	public static List<int> EnumWorkable()
	{
		lock (thisLock)
		{
			if (_gWorkable == null)
			{
				_gWorkable = new List<int>();
				foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _ReqWork }, new object[] { 1 }))
				{
					_gWorkable.Add(i);
				}
			}
			return new List<int>(_gWorkable);
		}
	}
	static int _WorkingRec = -1;
	override public void Store()
	{
		lock (thisLock)
		{
			_gWorkable = null;
		}
		_WorkingRec = -1;
		base.Store();
	}
	public static int GetWorkingRec()
	{
		if (_WorkingRec != -1)
		{
			return _WorkingRec;
		}

		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _Working }, new object[] { 1 }))
		{
			_WorkingRec = i;
			return i;
		}
		return 1;
	}
}