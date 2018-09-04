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
	protected static string _Tabl = "[TT_RES].[dbo].[DefectBuild]";
	protected static string[] _allBasecols = new string[] { _pid, _par, _date, _stat, _dateUp, _mach, _not };

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
		get { return this[_pid].ToString(); }
		set { this[_pid] = value; }
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
				switch (Convert.ToInt32(o))
				{
					case 1:
						{
							return "Building...";
						}
					case 2:
						{
							return "Finished. Status: OK!";
						}
					case 3:
						{
							return "Cancelled";
						}
					default:
						{
							return "Finished. Status: FAILED!";
						}
				}
			}
		}
		set { this[_dateUp] = int.Parse(value); }
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
		string sql = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", _Tabl, _stat, 3, _par, Defect.GetIDbyTT(ttid));
		SQLExecute(sql);
	}
}
