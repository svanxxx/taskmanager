using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

public class TRRecSignal : IdBasedObject
{
	protected static string _pid = "REPORT_ID";
	protected static string _dat = "REPORT_DATE";
	protected static string _perid = "PERSON_ID";
	protected static string _Tabl = "[TASKS].[dbo].[REPORTS]";
	protected static string[] _allBasecols = new string[] { _pid, _perid, _dat };

	public int ID
	{
		get { return Convert.ToInt32(this[_pid]); }
		set { this[_pid] = value; }
	}
	public string DATE
	{
		get { return GetAsDate(_dat); }
		set { SetAsDate(_dat, value); }
	}
	public int USER
	{
		get { return Convert.ToInt32(this[_perid]); }
		set { this[_perid] = value; }
	}
	public TRRecSignal()
	  : base(_Tabl, _allBasecols, 0.ToString(), _pid, false)
	{
	}
	public TRRecSignal(int id)
	  : base(_Tabl, _allBasecols, id.ToString(), _pid)
	{
	}
	public TRRecSignal(string table, string[] columns, string id, string pcname = "ID", bool doload = true)
		: base(table, columns, id, pcname, doload)
	{
	}
	public static List<TRRecSignal> Enum(DateTime from, DateTime to)
	{
		List<TRRecSignal> ls = new List<TRRecSignal>();
		string where = string.Format(" WHERE ( [{0}] >= '{1}' AND [{0}] <= '{2}')", _dat, from.ToString(DBHelper.SQLDateFormat), to.ToString(DBHelper.SQLDateFormat));
		TRRecSignal loader = new TRRecSignal();
		foreach (DataRow r in loader.GetRecords(where))
		{
			TRRecSignal rec = new TRRecSignal();
			rec.Load(r);
			ls.Add(rec);
		}
		return ls;
	}
}
public partial class TRRec : TRRecSignal
{
	static string _TIMEFORMAT = "HH\\:mm\\:ss";
	static string _start = "TIME_START";
	static string _end = "TIME_END";
	static string _done = "REPORT_DONE";
	static string _break = "REPORT_BREAK";
	static string[] _allcols = _allBasecols.Concat(new string[] { _start, _end, _done, _break }).ToArray();

	public string IN
	{
		get
		{
			DateTime d = (this[_start] == DBNull.Value) ? d = DateTime.Now : Convert.ToDateTime(this[_start]);
			d = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
			return d.ToString(_TIMEFORMAT, CultureInfo.InvariantCulture);
		}
		set
		{
			DateTime d = DateTime.Today;
			if (this[_dat] != DBNull.Value)
			{
				d = Convert.ToDateTime(this[_dat]);
			}
			string[] nums = value.Split(':');
			TimeSpan time = new TimeSpan(Convert.ToInt32(nums[0]), Convert.ToInt32(nums[1]), Convert.ToInt32(nums[2]));
			this[_start] = d.Add(time);
		}
	}
	public string OUT
	{
		get
		{
			DateTime d = (this[_end] == DBNull.Value) ? d = DateTime.Now : Convert.ToDateTime(this[_end]);
			d = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
			return d.ToString(_TIMEFORMAT, CultureInfo.InvariantCulture);
		}
		set
		{
			DateTime d = DateTime.Today;
			if (this[_dat] != DBNull.Value)
			{
				d = Convert.ToDateTime(this[_dat]);
			}
			string[] nums = value.Split(':');
			TimeSpan time = new TimeSpan(Convert.ToInt32(nums[0]), Convert.ToInt32(nums[1]), Convert.ToInt32(nums[2]));
			this[_end] = d.Add(time);
		}
	}
	public string BREAK
	{
		get
		{
			DateTime d = (this[_break] == DBNull.Value) ? d = DateTime.Now : Convert.ToDateTime(this[_break]);
			d = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
			return d.ToString(_TIMEFORMAT, CultureInfo.InvariantCulture);
		}
		set
		{
			DateTime d = DateTime.Today;
			if (this[_dat] != DBNull.Value)
			{
				d = Convert.ToDateTime(this[_dat]);
			}
			string[] nums = value.Split(':');
			TimeSpan time = new TimeSpan(Convert.ToInt32(nums[0]), Convert.ToInt32(nums[1]), Convert.ToInt32(nums[2]));
			this[_break] = d.Add(time);
		}
	}
	public string DONE
	{
		get { return this[_done].ToString(); }
		set { this[_done] = value; }
	}
	public List<int> SCHEDULEDTASKS
	{
		get
		{
			List<int> res = new List<int>();
			if (!IsLoaded())
				return res;
			MPSUser u = new MPSUser(USER);
			foreach (var d in DefectBase.EnumScheduled(DATE, u.EMAIL))
			{
				res.Add(d.ID);
			}
			return res;
		}
	}
	public List<int> CREATEDTASKS
	{
		get
		{
			List<int> res = new List<int>();
			if (!IsLoaded())
				return res;
			MPSUser u = new MPSUser(USER);
			foreach (var d in DefectBase.EnumCreated(DATE, u.EMAIL))
			{
				res.Add(d.ID);
			}
			return res;
		}
	}
	public List<int> MODIFIEDTASKS
	{
		get
		{
			List<int> res = new List<int>();
			if (!IsLoaded())
				return res;
			MPSUser u = new MPSUser(USER);
			foreach (var d in DefectBase.EnumModified(DATE, u.EMAIL))
			{
				res.Add(d.ID);
			}
			return res;
		}
	}
	public TRRec()
	  : base(_Tabl, _allcols, 0.ToString(), _pid, false)
	{
	}
	public TRRec(int id)
	  : base(_Tabl, _allcols, id.ToString(), _pid)
	{
	}
	public static TRRec GetRec(DateTime d, int personid)
	{
		foreach (int id in EnumRecords(_Tabl, _pid, new string[] { _perid, _dat }, new object[] { personid, d.Date }))
		{
			return new TRRec(id);
		}
		return null;
	}
	public static void DelRec(int id)
	{
		DeleteObject(_Tabl, id.ToString(), _pid);
	}
	public static void NewRec(DateTime d, int personid, bool lastday)
	{
		AddObject(_Tabl, new string[] { _perid, _dat }, new object[] { personid, d }, "");
		if (lastday)
		{
			string update = string.Format(@"
			UPDATE {0} SET {1} =
			(SELECT TOP 1 T2.{1} FROM {0} T2 WHERE T2.{2} < ? AND T2.{3} = ? ORDER BY T2.{2} DESC)
			WHERE {2} = ? AND {3} = ?
			", _Tabl, _done, _dat, _perid);
			SQLExecute(update, new object[] { d, personid, d, personid });
		}
	}
	public static List<TRRec> Enum(DateTime[] d)
	{
		List<TRRec> ls = new List<TRRec>();
		if (d.Length < 1)
		{
			return ls;
		}

		string where = "";
		foreach(var date in d)
		{
			if (!string.IsNullOrEmpty(where))
			{
				where += " OR ";
			}
			else
			{
				where = " WHERE ";
			}
			where += string.Format(" [{0}] = '{1}' ", _dat, date.ToString(DBHelper.SQLDateFormat));
		}

		foreach (DataRow r in (new TRRec()).GetRecords(where))
		{
			TRRec rec = new TRRec();
			rec.Load(r);
			ls.Add(rec);
		}
		return ls;
	}
	public static List<TRRec> EnumPersonal(int id, DateTime start, int days)
	{
		List<TRRec> ls = new List<TRRec>();
		DateTime end = (new DateTime(start.Year, start.Month, start.Day)).AddDays(days);
		string where = string.Format(" WHERE [{0}] = '{1}' AND [{2}] >= '{3}' AND [{2}] <= '{4}' ORDER BY {2} DESC", _perid, id, _dat, start.ToString(DBHelper.SQLDateFormat), end.ToString(DBHelper.SQLDateFormat));
		foreach (DataRow r in (new TRRec()).GetRecords(where))
		{
			TRRec rec = new TRRec();
			rec.Load(r);
			ls.Add(rec);
		}
		return ls;
	}
}