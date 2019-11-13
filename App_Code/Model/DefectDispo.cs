using System;
using System.Collections.Generic;
using System.Linq;

public class DefectDispo : Reference
{
	static string _Color = "Color";
	static string _ReqWork = "RequireWork";
	static string _Working = "BeingWorked";
	static string _CannotStart = "CannotStart";
	static string _TestsPas = "TestsPassed";
	static string _TestsRej = "TestsRejected";
	static string _TestsSta = "TestsStarted";

	public static string _Tabl = "[TT_RES].[DBO].[FLDDISPO]";
	static string[] _allCols = _allBaseCols.Concat(new string[] { _Color, _ReqWork, _Working, _CannotStart, _TestsPas, _TestsRej, _TestsSta }).ToArray();

	public bool REQUIREWORK
	{
		get { return Convert.ToBoolean(this[_ReqWork]); }
		set { this[_ReqWork] = value ? 1 : 0; }
	}
	public bool TESTSPASS
	{
		get { return Convert.ToBoolean(this[_TestsPas]); }
		set { this[_TestsPas] = value ? 1 : 0; }
	}
	public bool TESTSFAIL
	{
		get { return Convert.ToBoolean(this[_TestsRej]); }
		set { this[_TestsRej] = value ? 1 : 0; }
	}
	public bool TESTSSTART
	{
		get { return Convert.ToBoolean(this[_TestsSta]); }
		set { this[_TestsSta] = value ? 1 : 0; }
	}
	public bool WORKING
	{
		get { return Convert.ToBoolean(this[_Working]); }
		set { this[_Working] = value ? 1 : 0; }
	}
	public bool CANNOTSTART
	{
		get { return Convert.ToBoolean(this[_CannotStart]); }
		set { this[_CannotStart] = value ? 1 : 0; }
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

	static Object _lock = new Object();
	static List<DefectDispo> _gDispos;
	static string _planwhere = null;
	public static string PlanableDefectFilter()
	{
		lock (_lock)
		{
			if (_planwhere != null)
			{
				return _planwhere;
			}
			List<int> pl = EnumWorkableIDs();
			if (pl.Count > 0)
			{
				_planwhere = string.Format(" AND  ({0} in ({1}))", Defect._Disp, string.Join(",", pl));
			}
			return _planwhere;
		}
	}
	public static List<DefectDispo> Enum()
	{
		lock (_lock)
		{
			if (_gDispos == null)
			{
				_gDispos = new List<DefectDispo>();
				foreach (int i in EnumRecords(_Tabl, _ID))
				{
					_gDispos.Add(new DefectDispo(i));
				}
			}
			return new List<DefectDispo>(_gDispos);
		}
	}
	public static void ClearCache()
	{
		lock (_lock)
		{
			_gDispos = null;
			_planwhere = null;
		}
	}
	public static int New(string desc)
	{
		int res = Reference.New(_Tabl, desc);
		ClearCache();
		return res;
	}
	public static List<DefectDispo> EnumWorkable()
	{
		return new List<DefectDispo>(Enum().Where(item => item.REQUIREWORK == true));
	}
	public static List<int> EnumWorkableIDs()
	{
		return EnumWorkable().Select(x => x.ID).ToList();
	}
	public static List<DefectDispo> EnumCannotStart()
	{
		return new List<DefectDispo>(Enum().Where(item => item.CANNOTSTART == true));
	}
	public static List<int> EnumCannotStartIDs()
	{
		return EnumCannotStart().Select(x => x.ID).ToList();
	}
	public static List<DefectDispo> EnumTestsPassed()
	{
		return new List<DefectDispo>(Enum().Where(item => item.TESTSPASS == true));
	}
	public static List<DefectDispo> EnumTestsFailed()
	{
		return new List<DefectDispo>(Enum().Where(item => item.TESTSFAIL == true));
	}
	public static List<DefectDispo> EnumTestsStarted()
	{
		return new List<DefectDispo>(Enum().Where(item => item.TESTSSTART == true));
	}
	public static List<DefectDispo> EnumCanStart()
	{
		return new List<DefectDispo>(Enum().Where(item => item.CANNOTSTART == false));
	}
	public static List<int> EnumCanStartIDs()
	{
		return EnumCanStart().Select(x => x.ID).ToList();
	}
	public static List<DefectDispo> EnumWorkNow()
	{
		return new List<DefectDispo>(Enum().Where(item => item.WORKING == true));
	}
	public static string GetDispColor(int id)
	{
		DefectDispo d = Enum().Find(x => x.ID == id);
		if (d == null)
		{
			return "#FFFFFF";
		}
		return d.COLOR;
	}

	static int _WorkingRec = -1;
	override public void Store()
	{
		ClearCache();
		ClearCache();
		_WorkingRec = -1;
		base.Store();
	}
	public static int GetWorkingRec()
	{
		if (_WorkingRec != -1)
		{
			return _WorkingRec;
		}

		foreach (DefectDispo d in EnumWorkNow())
		{
			_WorkingRec = d.ID;
			return _WorkingRec;
		}
		return 1;
	}
}