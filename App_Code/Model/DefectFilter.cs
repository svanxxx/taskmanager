using System;
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
			if (f.text.StartsWith("\"") && f.text.EndsWith("\""))
			{
				f.text = f.text.Trim('"');
				lswhere.Add(string.Format(" AND  ({0} like '%{1}%')", _Summ, f.text));
			}
			else
			{
				string[] words = f.text.Split(null);
				foreach (var w in words)
				{
					lswhere.Add(string.Format(" AND  ({0} like '%{1}%')", _Summ, w));
				}
			}
		}
		return string.Format(" WHERE ({0} > 0 {1}) {2}", _ID, string.Join(string.Empty, lswhere), order ? string.Format("ORDER BY {0} DESC", _ID) : "");
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
