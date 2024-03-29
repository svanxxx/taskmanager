﻿using System;
using System.Collections.Generic;
using System.Linq;

public class DefectSeverity : Reference
{
	private const string _plan = "Planable";

	private const string _Tabl = "[TT_RES].[DBO].[FLDSEVER]";
	static string[] _allCols = _allBaseCols.Concat(new string[] { _plan }).ToArray();

	public bool PLAN
	{
		get
		{
			return GetAsBool(_plan);
		}
		set
		{
			if (value)
			{
				this[_plan] = value;
			}
			else
			{
				this[_plan] = DBNull.Value;
			}
		}
	}
	public DefectSeverity()
		 : base(_Tabl, _allCols, 0.ToString(), _ID, false)
	{
	}
	public DefectSeverity(object id)
		 : base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public static List<DefectSeverity> Enum()
	{
		List<DefectSeverity> res = new List<DefectSeverity>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectSeverity(i));
		}
		return res;
	}
	static List<int> _Planable = new List<int>();
	static string _planwhere = null;
	static object _lock = new object();
	public static List<int> EnumPlanable()
	{
		lock (_lock)
		{
			if (_Planable.Count < 1)
			{
				foreach (int i in EnumRecords(_Tabl, _ID, new string[] { string.Format(" UPPER(LEFT({0}, 1)) ", _Desc) }, new object[] { "A" }))
				{
					_Planable.Add(i);
				}
			}
			return new List<int>(_Planable);
		}
	}
	public static string PlanableDefectFilter()
	{
		lock (_lock)
		{
			if (_planwhere != null)
			{
				return _planwhere;
			}
			List<int> pl = DefectSeverity.EnumPlanable();
			if (pl.Count > 0)
			{
				_planwhere = string.Format(" AND  ({0} in ({1}))", Defect._Seve, string.Join(",", pl));
			}
			return _planwhere;
		}
	}
	static void ClearCache()
	{
		lock (_lock)
		{
			_Planable.Clear();
			_planwhere = null;
		}
	}
	public static int New(string desc)
	{
		int res = Reference.New(_Tabl, desc);
		ClearCache();
		return res;
	}
	protected override void PostStore()
	{
		base.PostStore();
		ClearCache();
	}
}