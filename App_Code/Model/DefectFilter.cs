﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

public class DefectsFilter
{
	public DefectsFilter() { }
	public List<int> dispositions;
	public List<int> users;
	public List<int> createdUsers;
	public List<int> modifiedUsers;
	public List<int> components;
	public List<int> severities;

	public string ID; //ids in form xxx,yyy-zzz,aaa
	public string text;
	public string orderer;

	public string startDateEnter;
	public string endDateEnter;

	public string startDateCreated;
	public string endDateCreated;

	public string startDateScheduled;
	public string endDateScheduled;

	public string startDateModified;
	public string endDateModified;

	public string startEstim;
	public string endEstim;
}

public partial class DefectBase : IdBasedObject
{
	string PrepareQueryForEnum(DefectsFilter f, bool order)
	{
		List<string> lswhere = new List<string>();
		if (!string.IsNullOrEmpty(f.ID))
		{
			string swhere = "";
			string[] ranges = f.ID.Trim().Split(',');
			foreach (string range in ranges)
			{
				string[] numbers = range.Trim().Split('-');
				int val1, val2;
				if (numbers.Length == 1 && int.TryParse(numbers[0], out val1))
				{
					if (!string.IsNullOrEmpty(swhere))
					{
						swhere += " OR ";
					}
					swhere += string.Format("({0} = {1})", _ID, val1);
				}
				else if (numbers.Length == 2 && int.TryParse(numbers[0], out val1) && int.TryParse(numbers[1], out val2))
				{
					if (!string.IsNullOrEmpty(swhere))
					{
						swhere += " OR ";
					}
					swhere += string.Format("(({0} >= {1}) AND ({0} <= {2}))", _ID, Math.Min(val1, val2), Math.Max(val1, val2));
				}
			}
			if (!string.IsNullOrEmpty(swhere))
			{
				lswhere.Add(string.Format(" AND ({0})", swhere));
			}
		}
		if (f.dispositions != null && f.dispositions.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _Disp, string.Join(",", f.dispositions)));
		}
		if (f.severities != null && f.severities.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _Seve, string.Join(",", f.severities)));
		}
		if (f.users != null && f.users.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _AsUser, string.Join(",", f.users)));
		}
		if (f.createdUsers != null && f.createdUsers.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _CreaBy, string.Join(",", f.createdUsers)));
		}
		if (f.modifiedUsers != null && f.modifiedUsers.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _ModBy, string.Join(",", f.modifiedUsers)));
		}
		if (f.components != null && f.components.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _Comp, string.Join(",", f.components)));
		}
		if (!string.IsNullOrEmpty(f.startDateEnter))
		{
			lswhere.Add(string.Format(" AND  ({0} >= '{1}')", _Date, DateTime.ParseExact(f.startDateEnter, defDateFormat, CultureInfo.InvariantCulture).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.endDateEnter))
		{
			lswhere.Add(string.Format(" AND  ({0} < '{1}')", _Date, DateTime.ParseExact(f.endDateEnter, defDateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.startDateCreated))
		{
			lswhere.Add(string.Format(" AND  ({0} >= '{1}')", _Created, DateTime.ParseExact(f.startDateCreated, defDateFormat, CultureInfo.InvariantCulture).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.endDateCreated))
		{
			lswhere.Add(string.Format(" AND  ({0} < '{1}')", _Created, DateTime.ParseExact(f.endDateCreated, defDateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.startDateScheduled))
		{
			lswhere.Add(string.Format(" AND  ({0} >= '{1}')", _OrderDate, DateTime.ParseExact(f.startDateScheduled, defDateFormat, CultureInfo.InvariantCulture).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.endDateScheduled))
		{
			lswhere.Add(string.Format(" AND  ({0} < '{1}')", _OrderDate, DateTime.ParseExact(f.endDateScheduled, defDateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.startDateModified))
		{
			lswhere.Add(string.Format(" AND  ({0} >= '{1}')", _ModDate, DateTime.ParseExact(f.startDateModified, defDateFormat, CultureInfo.InvariantCulture).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.endDateModified))
		{
			lswhere.Add(string.Format(" AND  ({0} < '{1}')", _ModDate, DateTime.ParseExact(f.endDateModified, defDateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(DBHelper.SQLDateFormat)));
		}
		if (!string.IsNullOrEmpty(f.orderer))
		{
			lswhere.Add(string.Format(" AND  ({0} = '{1}')", _sMod, f.orderer));
		}
		if (!string.IsNullOrEmpty(f.startEstim))
		{
			lswhere.Add(string.Format(" AND  ({0} >= '{1}')", _Est, int.Parse(f.startEstim)));
		}
		if (!string.IsNullOrEmpty(f.endEstim))
		{
			lswhere.Add(string.Format(" AND  ({0} <= '{1}')", _Est, int.Parse(f.endEstim)));
		}
		if (!string.IsNullOrEmpty(f.text))
		{
			string[] words = null;
			if (f.text.StartsWith("\"") && f.text.EndsWith("\""))
			{
				words = new string[] { f.text.Trim('"') };
			}
			else
			{
				words = f.text.Split(null);
			}
			string s1 = "";
			string s2 = "";
			foreach (var w in words)
			{
				s1 += (s1 == "") ? "(" : " AND ";
				s1 += string.Format(" CONTAINS({0}, '\"{1}\"')", _Summ, w);

				s2 += (s2 == "") ? string.Format("{0} IN (SELECT idDefRec FROM {1} WHERE CONTAINS ({2}, '", _idRec, Defect._RepTable, Defect._DescInt) : " AND ";
				s2 += string.Format("\"{0}\"", w);
			}
			s1 += " )";
			s2 += "'))";
			lswhere.Add(string.Format(" AND  ({0} OR {1})", s1, s2));
		}
		return string.Format(" WHERE (1=1 {0}) {1}", string.Join(string.Empty, lswhere), order ? string.Format("ORDER BY {0} DESC", _ID) : "");
	}
	public int EnumCount(DefectsFilter f)
	{
		string where = PrepareQueryForEnum(f, false);
		string sql = string.Format("select count({0}) from {1} {2}", _ID, _Tabl, where);
		return Convert.ToInt32(DBHelper.GetValue(sql));
	}
	public List<DefectBase> Enum(DefectsFilter f, int maxrecs = 200)
	{
		string where = PrepareQueryForEnum(f, true);
		List<DefectBase> ls = new List<DefectBase>();
		foreach (DataRow r in GetRecords(where, maxrecs))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			ls.Add(d);
		}
		return ls;
	}
}
