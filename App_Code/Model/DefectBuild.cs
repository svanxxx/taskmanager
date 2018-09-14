using System;
using System.Collections.Generic;

public class DefectBuild : IdBasedObject
{
	protected static string _pid = "idRecord";
	protected static string _par = "ParentID";
	protected static string _date = "DateTimeCreate";
	protected static string _stat = "Status";
	protected static string _dateUp = "DateTimeUpdate";
	protected static string _mach = "Machine";
	protected static string _not = "Notes";
	protected static string _gui = "UGuid";
	protected static string _stText = "StatusText";
	protected static string _Tabl = "[TT_RES].[dbo].[DefectBuild]";
	protected static string[] _allBasecols = new string[] { _pid, _par, _date, _stat, _dateUp, _mach, _not, _stText };

	public int ID
	{
		get { return Convert.ToInt32(this[_pid]); }
		set { this[_pid] = value; }
	}
	public string DATE
	{
		get { return Convert.ToDateTime(this[_date]).ToLocalTime().ToString(); }
		set { this[_date] = value; }
	}
	public string NOTES
	{
		get { return this[_not].ToString(); }
		set { this[_not] = value; }
	}
	public string MACHINE
	{
		get { return this[_mach].ToString(); }
		set { this[_mach] = value; }
	}
	public string DATEUP
	{
		get
		{
			if (this[_dateUp] == DBNull.Value)
			{
				return "";
			}
			return Convert.ToDateTime(this[_dateUp]).ToLocalTime().ToString();
		}
		set { this[_dateUp] = value; }
	}
	public int DEFID
	{
		get { return Convert.ToInt32(this[_par]); }
		set { this[_par] = value; }
	}
	public string STATUSTXT
	{
		get { return this[_stText].ToString(); }
		set { this[_stText] = value; }
	}
	public enum BuildStatus
	{
		progress = 1,
		finishedok = 2,
		cancelled = 3,
		failed = 4
	}
	public string STATUS
	{
		get
		{
			var o = this[_stat];
			if (o == DBNull.Value)
			{
				return "Awaiting build machine...";
			}
			else
			{
				switch ((BuildStatus)Convert.ToInt32(o))
				{
					case BuildStatus.progress:
						{
							return "Building in progress...";
						}
					case BuildStatus.finishedok:
						{
							return "Finished. Status: OK!";
						}
					case BuildStatus.cancelled:
						{
							return "Cancelled";
						}
					case BuildStatus.failed:
						{
							return "Finished. Status: FAILED!";
						}
					default:
						{
							return "Unknown";
						}
				}
			}
		}
		set
		{
			int res;//for set from web interface and/or normal c# code
			if (int.TryParse(value, out res))
			{
				this[_stat] = res;
				return;
			}
			BuildStatus st = (BuildStatus)Enum.Parse(typeof(BuildStatus), value);
			this[_stat] = (int)st;
		}
	}
	public DefectBuild()
	  : base(_Tabl, _allBasecols, 0.ToString(), _pid, false)
	{
	}
	public DefectBuild(int id)
	  : base(_Tabl, _allBasecols, id.ToString(), _pid)
	{
	}
	public static List<DefectBuild> GetEventsByTask(int ttid)
	{
		List<DefectBuild> res = new List<DefectBuild>();
		foreach (int i in EnumRecords(_Tabl, _pid, new string[] { _par }, new object[] { Defect.GetIDbyTT(ttid) }))
		{
			res.Add(new DefectBuild(i));
		}
		return res;
	}
	public static void AddRequestByTask(int ttid, string notes)
	{
		AddObject(_Tabl, new string[] { _par, _not }, new object[] { Defect.GetIDbyTT(ttid), notes }, _pid);
	}
	public static void CancelRequestByTask(int ttid)
	{
		string sql = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", _Tabl, _stat, (int)BuildStatus.cancelled, _par, Defect.GetIDbyTT(ttid));
		SQLExecute(sql);
	}
	public static DefectBuild GetTask2Build(string machine)
	{
		string g = Guid.NewGuid().ToString();

		SQLExecute(string.Format("UPDATE {0} SET [{1}] = {2}, [{3}] = '{4}', [{5}] = '{6}' WHERE [{1}] is NULL", _Tabl, _stat, (int)BuildStatus.progress, _mach, machine, _gui, g));

		var o = DBHelper.GetValue(string.Format("SELECT [{0}] FROM {1} WHERE [{2}] = '{3}'", _pid, _Tabl, _gui, g));
		if (o == DBNull.Value || o == null)
		{
			return null;
		}
		return new DefectBuild(Convert.ToInt32(o));
	}
	public static bool hasBuildRequest()
	{
		return Convert.ToInt32(GetValue(string.Format("SELECT COUNT(*) FROM {0} WHERE {1} IS NULL OR {1} = {2}", _Tabl, _stat, (int)BuildStatus.progress))) > 0;
	}
}