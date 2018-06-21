using System;
using System.Collections.Generic;
using System.Data;

public class Machine : IdBasedObject
{
	static string _ID = "ID";
	static string _Name = "PCNAME";
	static string _Mac = "MAC";
	
	static string _Tabl = "[TASKS].[dbo].[PCS]";
	static string[] _allCols = new string[] { _ID, _Name, _Mac };

	public int ID
	{
		get { return Convert.ToInt32(this[_ID]); }
		set { this[_ID] = value; }
	}
	public string NAME
	{
		get { return this[_Name].ToString(); }
		set { this[_Name] = value; }
	}
	public string MAC
	{
		get { return this[_Mac].ToString(); }
		set { this[_Mac] = value; }
	}
	public Machine()
		: base(_Tabl, _allCols, 1.ToString(), _ID, false)
	{
	}
	public Machine(int id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public List<Machine> Enum()
	{
		List<Machine> ls = new List<Machine>();
		foreach(DataRow r in GetRecords(""))
		{
			Machine m = new Machine();
			m.Load(r);
			ls.Add(m);
		}
		return ls;
	}
}