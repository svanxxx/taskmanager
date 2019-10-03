using System;
using System.Collections.Generic;

public class DefectEvent : IdBasedObject
{
	private static string _ID = "idRecord";
	private static string _Dat = "dateEvent";
	private static string _idUser = "idUser";
	private static string _Notes = "Notes";
	private static string _ParentID = "ParentID";
	private static string _TimeSpent = "TimeSpent";
	private static string _Proj = "ProjectID";
	private static string _EvtDefID = "EvtDefID";
	private static string _Order = "OrderNum";
	private static string _EvtMUParnt = "EvtMUParnt";
	private static string _RsltState = "RsltState";
	private static string _RelVersion = "RelVersion";
	private static string _AsgndUsers = "AsgndUsers";
	private static string _GenByType = "GenByType";
	private static string _CreatorID = "CreatorID";
	private static string _DefAsgEff = "DefAsgEff";
	private static string _OvrWF = "OvrWF";
	private static string _OvrWFUsrID = "OvrWFUsrID";
	private static string[] _allCols = new string[] { _ID, _Dat, _idUser, _Notes, _ParentID, _TimeSpent, _EvtDefID, _AsgndUsers, _Order };
	private static string _Tabl = "[TT_RES].[DBO].[DEFECTEVTS]";

	public int ID
	{
		get
		{
			return GetAsInt(_ID);
		}
		set
		{
			this[_ID] = value;
		}
	}
	public int DEFECTID
	{
		get
		{
			return GetAsInt(_ParentID);
		}
		set
		{
			this[_ParentID] = value;
		}
	}
	public string NOTES
	{
		get { return this[_Notes].ToString(); }
		set { this[_Notes] = value; }
	}
	public string IDUSER
	{
		get { return this[_idUser].ToString(); }
		set { this[_idUser] = value; }
	}
	public string DATE
	{
		get { return Convert.ToDateTime(this[_Dat]).ToLocalTime().ToString(); }
		set { this[_Dat] = value; }
	}
	public string TIME
	{
		get
		{
			int time = Convert.ToInt32(this[_TimeSpent]);
			if (time < 0)
				return "";
			else
				return time.ToString();
		}
		set { this[_TimeSpent] = value; }
	}
	public int ORDER
	{
		get
		{
			if (this[_Order] == DBNull.Value)
			{
				return 0;
			}
			return Convert.ToInt32(this[_Order]);
		}
		set { this[_Order] = value; }
	}
	public enum Eventtype
	{
		assigned = 1,
		estimated = 2,
		worked = 11,
        versionIncluded = 12,
	}
	public int ASSIGNUSERID
	{
		get
		{
			int res = -1;
			Int32.TryParse(this[_AsgndUsers].ToString(), out res);
			return res;
		}
		set { this[_AsgndUsers] = value; }
	}
	public string EVENT
	{
		get
		{
			return ((Eventtype)Convert.ToInt32(this[_EvtDefID])).ToString();
		}
		set { this[_TimeSpent] = value; }
	}

	public DefectEvent()
		: base(_Tabl, _allCols, 0.ToString(), _ID, false)
	{
	}
	public DefectEvent(int id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public static List<DefectEvent> GetEventsByTask(int ttid)
	{
		List<DefectEvent> res = new List<DefectEvent>();
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _ParentID }, new object[] { Defect.GetIDbyTT(ttid) }))
		{
			res.Add(new DefectEvent(i));
		}
		return res;
	}
	public static List<int> GetEventsByDay(DateTime dt, int iduser)
	{
		List<int> res = new List<int>();
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _idUser, _EvtDefID, _Dat }, new object[] { iduser, (int)Eventtype.worked, dt }))
		{
			res.Add(i);
		}
		return res;
	}
	public static void AddEventByTask(int id, Eventtype type, decimal usr, string notes, int estimation = -1, int assign = -1, DateTime? dt = null)
	{
		string eventdate = "GETUTCDATE()";
		if (dt != null)
		{
			eventdate = $"'{dt.GetValueOrDefault().ToString(DBHelper.SQLDateFormat)}'";
		}
		notes = notes.Replace("'", "''");
		string sql = string.Format(@"
			INSERT INTO {0}
			({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18})
			values
			(
			   1
			 , (select max({2}) + 1 from {0} E1 where {2} < 3000000)
			 , {22}
			 , (select CASE WHEN max(E1.{4}) IS NULL THEN 1 ELSE max(E1.{4}) + 1 END from {0} E1 where E1.{5} = {19})
			 , {19}
			 , 4294967295
			 , {20}
			 , {24}
			 , '{25}'
			 , {21}
			 , 0
			 , ' '
			 , {23}
			 , 0
			 , 4294967295
			 , {22}
			 , 0
			 , 0
			)		
		", _Tabl,
			_Proj, _ID, _EvtDefID, _Order, _ParentID, _EvtMUParnt, _idUser, _Dat, _Notes, _TimeSpent, _RsltState, _RelVersion, _AsgndUsers, _GenByType, _CreatorID, _DefAsgEff, _OvrWF, _OvrWFUsrID,
			id, usr, estimation, (int)type, assign == -1 ? "' '" : assign.ToString(), eventdate, notes);

		SQLExecute(sql);
	}
	public static void DelHisotoryByTask(int parentid)
	{
		string sql = string.Format(@"DELETE FROM {0} WHERE {1} = {2}", _Tabl, _ParentID, parentid);
		SQLExecute(sql);
	}
	public static void Delete(int id)
	{
		SQLExecute($"DELETE FROM {_Tabl} WHERE {_ID} = {id}");
	}
}