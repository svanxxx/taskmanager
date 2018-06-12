using System;
using System.Collections.Generic;

public class DefectDispo : IdBasedObject
{
	static string _ID = "idRecord";
	static string _Desc = "Descriptor";
	static string _idOrd = "FieldOrder";
	static string _Color = "Color";
	static string _ReqWork = "RequireWork";
	static string _Working = "BeingEorked";
	
	static string _Tabl = "[TT_RES].[DBO].[FLDDISPO]";
	static string[] _allCols = new string[] { _ID, _Desc, _idOrd, _Color, _ReqWork, _Working };

	public int ID
	{
		get { return Convert.ToInt32(this[_ID]); }
		set { this[_ID] = value; }
	}
	public int REQUIREWORK
	{
		get { return Convert.ToInt32(this[_ReqWork]); }
		set { this[_ReqWork] = value; }
	}
	public int WORKING
	{
		get { return Convert.ToInt32(this[_Working]); }
		set { this[_Working] = value; }
	}
	public string COLOR
	{
		get { return this[_Color].ToString(); }
		set { this[_Color] = value; }
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
	public static List<int> EnumWorkable()
	{
		List<int> res = new List<int>();
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _ReqWork }, new object[] { 1 }))
		{
			res.Add(i);
		}
		return res;
	}
	static int _WorkingRec = -1;
	override public void Store()
	{
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