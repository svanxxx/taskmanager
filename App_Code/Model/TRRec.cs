using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;

public class TRRec : IdBasedObject
{
	const string _TIMEFORMAT = "HH\\:mm\\:ss";
	const string _pid = "REPORT_ID";
	const string _perid = "PERSON_ID";
	const string _start = "TIME_START";
	const string _end = "TIME_END";
	const string _dat = "REPORT_DATE";
	const string _done = "REPORT_DONE";
	const string _break = "REPORT_BREAK";
	static string[] _allcols = new string[] { _pid, _perid, _start, _end, _dat, _done, _break };
	static string _Tabl = "[TASKS].[dbo].[REPORTS]";

	public int ID
	{
		get { return Convert.ToInt32(this[_pid]); }
		set { this[_pid] = value; }
	}
	public int USER
	{
		get { return Convert.ToInt32(this[_perid]); }
		set { this[_perid] = value; }
	}
	public string DATE
	{
		get { return (this[_dat] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(this[_dat])).ToString(defDateFormat, CultureInfo.InvariantCulture); }
		set { this[_dat] = Convert.ToDateTime(value, CultureInfo.InvariantCulture); }
	}
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
			DateTime d = Convert.ToDateTime(this[_dat]);
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
			DateTime d = Convert.ToDateTime(this[_dat]);
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
			DateTime d = Convert.ToDateTime(this[_dat]);
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
	public List<TRRec> Enum(DateTime[] d)
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

		foreach (DataRow r in GetRecords(where))
		{
			TRRec rec = new TRRec();
			rec.Load(r);
			ls.Add(rec);
		}
		return ls;
	}
}