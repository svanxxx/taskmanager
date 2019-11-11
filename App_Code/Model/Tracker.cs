using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

public class Complete
{
	public Complete() { }
	public string COLOR { get; set; }
	public double PERCENT { get; set; }
	public double Y { get; set; }
}
public class Tracker : IdBasedObject
{
	private static string _pid = "idRecord";
	private static string _Nam = "Name";
	private static string _Own = "idOwner";
	private static string _Flt = "idFilter";
	private static string _Cli = "idClient";
	private static string _Cre = "dateCreated";
	private static string _Col = "COLORDEF";
	private static string[] _allCols = new string[] { _pid, _Nam, _Own, _Flt, _Cli, _Cre, _Col };
	private static string _Tabl = "[TT_RES].[DBO].[DefectTracker]";

	public int ID
	{
		get { return GetAsInt(_pid); }
		set { this[_pid] = value; }
	}
	public List<Complete> Completes
	{
		get
		{
			double total = 0;
			List<Complete> res = new List<Complete>();
			foreach(var s in COLORDEFS.Split(';'))
			{
				if (string.IsNullOrEmpty(s))
				{
					continue;
				}
				string[] items = s.Split(':');
				if (items.Length > 1)
				{
					double val = double.Parse(items[0]);
					total += val;
					res.Add(new Complete() { PERCENT = val , COLOR = items[1] });
				}
			}
			double dy = 0;
			foreach(var p in res)
			{
				p.PERCENT = p.PERCENT * 100.0 / total;
				p.Y = dy;
				dy += p.PERCENT;
			}
			return res;
		}
	}
	public string NAME
	{
		get { return GetAsString(_Nam); }
		set { this[_Nam] = value; }
	}
	public string COLORDEFS
	{
		get { return GetAsString(_Col); }
		set { this[_Col] = value; }
	}
	public string CREATED
	{
		get { return GetAsDateTime(_Cre); }
		set { SetAsDate(_Cre, value); }
	}
	public int OWNER
	{
		get { return GetAsInt(_Own); }
		set { this[_Own] = value; }
	}
	public int IDFILTER
	{
		get { return GetAsInt(_Flt); }
		set { this[_Flt] = value; }
	}
	public int IDCLIENT
	{
		get { return GetAsInt(_Cli); }
		set { this[_Cli] = value; }
	}

	public Tracker()
	 : base(_Tabl, _allCols, 0.ToString(), _pid, false)
	{
	}
	public Tracker(int id)
	  : base(_Tabl, _allCols, id.ToString(), _pid, true)
	{
	}
	public string GetTag()
	{
		return "tag_" + NAME.Replace(" ", "_");
	}
	public DefectsFilter GetFilter()
	{
		if (IDFILTER < 0)
		{
			return new DefectsFilter() { text = GetTag() };
		}
		return new StoredDefectsFilter(IDFILTER).GetFilter();
	}
	static public Tracker New(string name, int user, int filter)
	{
		string g = Guid.NewGuid().ToString();
		SQLExecute($"INSERT INTO {_Tabl} ({_Nam}, {_Own}, {_Flt}) VALUES ('{g}', {user}, {filter})");
		int id = Convert.ToInt32(GetValue($"SELECT {_pid} FROM {_Tabl} WHERE {_Nam} = '{g}'"));
		Tracker tr = new Tracker(id)
		{
			NAME = name
		};
		tr.Store();
		return tr;
	}
	static public void Delete(int id)
	{
		SQLExecute(string.Format("DELETE FROM {0} WHERE {1} = {2}", _Tabl, _pid, id));
	}
	static public List<Tracker> Enum(int user)
	{
		if (user < 0)
		{
			return new List<Tracker>();
		}
		DefectUser usr = new DefectUser(user);
		MPSUser mpu = new MPSUser(usr.TRID);
		string filter = mpu.ISCLIENT ? $"WHERE {_Own} = {user} OR {_Cli} = {user} ORDER BY {_Nam} asc" : $"WHERE {_Own} = {user} OR {_Cli} is not null ORDER BY {_Nam} asc";
		List<Tracker> res = new List<Tracker>();
		foreach (DataRow r in (new Tracker()).GetRecords(filter))
		{
			Tracker d = new Tracker();
			d.Load(r);
			res.Add(d);
		}
		return res;
	}
}