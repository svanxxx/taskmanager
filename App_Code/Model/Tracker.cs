using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

public class Tracker : IdBasedObject
{
	private static string _pid = "idRecord";
	private static string _Nam = "Name";
	private static string _Own = "idOwner";
	private static string _Flt = "idFilter";
	private static string _Cli = "idClient";
	private static string[] _allCols = new string[] { _pid, _Nam, _Own, _Flt, _Cli };
	private static string _Tabl = "[TT_RES].[DBO].[DefectTracker]";

	public int ID
	{
		get { return GetAsInt(_pid); }
		set { this[_pid] = value; }
	}
	public string NAME
	{
		get { return GetAsString(_Nam); }
		set { this[_Nam] = value; }
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
		List<Tracker> res = new List<Tracker>();
		foreach (DataRow r in (new Tracker()).GetRecords($"WHERE {_Own} = {user} OR {_Cli} = {user} ORDER BY {_Nam} asc"))
		{
			Tracker d = new Tracker();
			d.Load(r);
			res.Add(d);
		}
		return res;
	}
}