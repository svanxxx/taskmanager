using System;
using System.Globalization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

public class LockInfo
{
	public LockInfo(string lockedby, string globallock)
	{
		this.lockedby = lockedby;
		this.globallock = globallock;
	}
	public string lockedby { get; set; }
	public string globallock { get; set; }
}
public class LockEvent
{
	public LockEvent(string id)
	{
		Prolongate();
		lockid = id;
		usr = CurrentContext.User.EMAIL;
	}
	public void Prolongate()
	{
		locktime = DateTime.Now;
	}
	public bool Obsolete
	{
		get
		{
			return DateTime.Now.Subtract(locktime).TotalSeconds > 30;
		}
	}
	DateTime locktime { get; set; }
	public string lockid { get; set; }
	public string usr { get; set; }
}
public class DefectBase : IdBasedObject
{
	protected static string _idRec = "idRecord";
	protected static string _ID = "DefectNum";
	protected static string _Summ = "Summary";
	protected static string _Disp = "idDisposit";
	protected static string _Est = "Estim";
	protected static string _Order = "iOrder";
	protected static string _BackOrder = "BackOrder";
	protected static string _AsUser = "idUsr";
	protected static string _Seve = "idSeverity";
	protected static string _sMod = "sModifier";
	protected static string _Comp = "idCompon";
	protected static string _Date = "dateEnter";
	protected static string _BackOr = "_BackOr";

	protected static string _Tabl = "[TT_RES].[DBO].[DEFECTS]";

	static string[] _allcols = new string[] { _ID, _Summ, _idRec, _Disp, _Est, _Order, _AsUser, _Seve, _sMod, _BackOrder, _Comp, _Date };
	static string[] _allcolsNames = new string[] { _ID, "Summary", _idRec, "Disposition", "Estimation", "Schedule Order", "Assigned User", "Severity", "", "Schedule Order", "Component", "Date Entered" };

	public string SEVE
	{
		get { return this[_Seve].ToString(); }
		set { this[_Seve] = Convert.ToInt32(value); }
	}
	public int ID
	{
		get { return this[_ID] == DBNull.Value ? -1 : Convert.ToInt32(this[_ID]); }
		set { this[_ID] = Convert.ToInt32(value); _id = ID.ToString(); }
	}
	public int IDREC
	{
		get { return this[_idRec] == DBNull.Value ? 0 : Convert.ToInt32(this[_idRec]); }
		set { this[_idRec] = Convert.ToInt32(value); }
	}
	public string SUMMARY
	{
		get { return this[_Summ].ToString().Replace("\n", String.Empty).Replace("\r", String.Empty); }
		set { this[_Summ] = value; }
	}
	public string SMODIFIER
	{
		get { return this[_sMod].ToString().Trim(); }
		set { this[_sMod] = value; }
	}
	public string DISPO
	{
		get { return this[_Disp].ToString(); }
		set { this[_Disp] = Convert.ToInt32(value); }
	}
	public int ESTIM
	{
		get { return this[_Est] == DBNull.Value ? 0 : Convert.ToInt32(this[_Est]); }
		set
		{
			if (value == 0)
			{
				this[_Est] = DBNull.Value;
			}
			else
			{
				this[_Est] = value;
			}
		}
	}
	protected virtual void OnBackOrderChanged()
	{
	}
	public int BACKORDER
	{
		get { return this[_BackOrder] == DBNull.Value ? -1 : Convert.ToInt32(this[_BackOrder]); }
		set
		{
			if (BACKORDER != value)
			{
				OnBackOrderChanged();
			}
			if (value == -1)
			{
				this[_BackOrder] = DBNull.Value;
			}
			else
			{
				this[_BackOrder] = value;
			}
		}
	}
	public int ORDER
	{
		get { return this[_Order] == DBNull.Value ? -1 : Convert.ToInt32(this[_Order]); }
		set
		{
			if (ORDER != value)
			{
				this[_sMod] = CurrentContext.User.EMAIL;
			}
			if (value == -1)
			{
				this[_Order] = DBNull.Value;
			}
			else
			{
				this[_Order] = value;
			}
		}
	}
	public string AUSER
	{
		get { return this[_AsUser] == DBNull.Value ? "" : this[_AsUser].ToString(); }
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				this[_AsUser] = DBNull.Value;
				return;
			}
			this[_AsUser] = Convert.ToInt32(value);
		}
	}
	public string COMP
	{
		get { return this[_Comp].ToString(); }
		set { this[_Comp] = Convert.ToInt32(value); }
	}
	public string DATE
	{
		get { return (this[_Date] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(this[_Date])).ToString(defDateFormat, CultureInfo.InvariantCulture); }
		set { this[_Date] = Convert.ToDateTime(value, CultureInfo.InvariantCulture); }
	}

	protected override void OnProcessComplexColumn(string col, object val)
	{
		if (col == _BackOrder)
		{
			string sqlupdate = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", _Tabl, _Order, BACKORDER, _idRec, IDREC);
			SQLExecute(sqlupdate);
			return;
		}
		if (col == _Order)
		{
			if (val == DBNull.Value)
			{
				string sqlupdate = string.Format("UPDATE {0} SET {1} = NULL WHERE {3} = {4}", _Tabl, _Order, val, _idRec, IDREC);
				SQLExecute(sqlupdate);
			}
			else
			{
				int ord = Convert.ToInt32(val);
				List<int> wl = DefectDispo.EnumWorkable();
				string ids = string.Join(",", wl);

				string sql = string.Format(@"
				SELECT MIN({0}) FROM
				(
				SELECT TOP {5} * FROM 
				(SELECT {0} FROM {1} WHERE {2} = {6} AND {0} IS NOT NULL AND {3} IN ({4}) GROUP BY {0}) T
				ORDER BY 1 DESC
				) A
			", _Order, _Tabl, _AsUser, _Disp, ids, ord, AUSER);

				object o = GetValue(sql);
				if (o != DBNull.Value)
				{
					string sqlupdate = string.Format("UPDATE {0} SET {1} = {1} + 1 WHERE {1} > {2} AND {3} = {4} AND {5} IN ({6})", _Tabl, _Order, Convert.ToInt32(o), _AsUser, AUSER, _Disp, ids);
					SQLExecute(sql);
					sqlupdate = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", _Tabl, _Order, Convert.ToInt32(o) + 1, _idRec, IDREC);
					SQLExecute(sqlupdate);
				}
				else
				{
					string sqlupdate = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", _Tabl, _Order, ord, _idRec, IDREC);
					SQLExecute(sql);
				}
			}
			return;
		}
		base.OnProcessComplexColumn(col, val);
	}
	protected override string OnTransformCol(string col)
	{
		if (col == _Order)
		{
			List<int> wl = DefectDispo.EnumWorkable();
			return string.Format("(CASE WHEN {1}.{0} IS NULL THEN NULL ELSE (SELECT COUNT(*) + 1 FROM {1} D2 WHERE D2.IDUSR = {1}.IDUSR AND D2.{0} > {1}.{0} AND {3} in ({4}))END) {2}", _Order, _Tabl, _Order, _Disp, string.Join(",", wl));
		}
		else if (col == _BackOrder)
		{
			return string.Format("({0}) {1}", _Order, _BackOrder);
		}
		return base.OnTransformCol(col);
	}
	protected override bool IsColumnComplex(string col)
	{
		if (col == _Order || col == _BackOrder)
			return true;

		return base.IsColumnComplex(col);
	}

	public DefectBase()
		: base(_Tabl, _allcols, "0", _ID, false)
	{
	}
	public DefectBase(int ttid)
		: base(_Tabl,
					_allcols,
					ttid.ToString(),
					_ID)
	{
	}
	public DefectBase(string table, string[] columns, string id, string pcname = "ID", bool doload = true)
		: base(table, columns, id, pcname, doload)
	{
	}
	public List<DefectBase> EnumPlanShort(int userid)
	{
		return EnumPlanLim(userid, 10);
	}
	public List<DefectBase> EnumPlan(int userid)
	{
		return EnumPlanLim(userid, 0);
	}
	public List<DefectBase> EnumPlanLim(int userid, int max) //zero for unlimited number
	{
		List<int> wl = DefectDispo.EnumWorkable();
		string w_where = "";
		if (wl.Count > 0)
		{
			w_where = string.Format(" AND  ({0} in ({1}))", _Disp, string.Join(",", wl));
		}

		List<DefectBase> ls = new List<DefectBase>();
		string where = string.Format(" WHERE (({0} = {1}) AND ({2} is not null) {3}) ORDER BY {4}.{2} DESC", _AsUser, userid, _Order, w_where, _Tabl);
		foreach (DataRow r in GetRecords(where, max))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			ls.Add(d);
		}
		return ls;
	}
	public List<DefectBase> EnumUnPlan(int userid)
	{
		List<int> wl = DefectDispo.EnumWorkable();
		string w_where1 = "";
		if (wl.Count > 0)
		{
			w_where1 = string.Format(" AND  ({0} in ({1}))", _Disp, string.Join(",", wl));
		}

		List<int> pl = DefectSeverity.EnumPlanable();
		string w_where2 = "";
		if (pl.Count > 0)
		{
			w_where2 = string.Format(" AND  ({0} in ({1}))", _Seve, string.Join(",", pl));
		}

		List<DefectBase> ls = new List<DefectBase>();
		string where = string.Format(" WHERE (({0} = {1}) AND ({2} is null) {3} {4}) ORDER BY {5} DESC", _AsUser, userid, _Order, w_where1, w_where2, _ID);
		foreach (DataRow r in GetRecords(where))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			ls.Add(d);
		}
		return ls;
	}
	public List<DefectBase> Enum(DefectsFilter f, int maxrecs = 200)
	{
		List<string> lswhere = new List<string>();
		if (f.dispositions != null && f.dispositions.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _Disp, string.Join(",", f.dispositions)));
		}
		if (f.users != null && f.users.Count > 0)
		{
			lswhere.Add(string.Format(" AND  ({0} in ({1}))", _AsUser, string.Join(",", f.users)));
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
			lswhere.Add(string.Format(" AND  ({0} <= '{1}')", _Date, DateTime.ParseExact(f.endDateEnter, defDateFormat, CultureInfo.InvariantCulture).ToString(DBHelper.SQLDateFormat)));
		}

		List<DefectBase> ls = new List<DefectBase>();
		string where = string.Format(" WHERE ({0} > 0 {1}) ORDER BY {0} DESC", _ID, string.Join(string.Empty, lswhere));
		foreach (DataRow r in GetRecords(where, maxrecs))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			ls.Add(d);
		}
		return ls;
	}
	public List<DefectBase> EnumCloseVacations(string startdate)
	{
		DefectsFilter f = new DefectsFilter();
		f.components = new List<int>() { DefectComp.GetVacationRec() };
		f.startDateEnter = startdate;
		f.endDateEnter = DateTime.ParseExact(startdate, defDateFormat, CultureInfo.InvariantCulture).AddDays(15).ToString(defDateFormat);//two weeks adnvance
		return Enum(f, 2000);
	}
}
public class DefectsFilter
{
	public DefectsFilter() { }
	public List<int> dispositions;
	public List<int> users;
	public List<int> components;
	public string startDateEnter;
	public string endDateEnter;
}
public class Defect : DefectBase
{
	static ConcurrentDictionary<string, LockEvent> locker = new ConcurrentDictionary<string, LockEvent>();
	static Object thisLock = new Object();

	static protected string _Desc = "DESCR";
	static protected string _Specs = "ReproSteps";
	static protected string _Type = "idType";
	static protected string _Prod = "idProduct";
	static protected string _Ref = "Reference";
	static protected string _Prio = "idPriority";
	static protected string _Crea = "idCreateBy";
	static string[] _allcols = new string[] { _ID, _Specs, _Summ, _Desc, _idRec, _Type, _Prod, _Ref, _Disp, _Prio, _Comp, _Seve, _Date, _Crea, _Est, _Order, _AsUser, _sMod, _BackOrder };
	static string[] _allcolsNames = new string[] { _ID, "Specification", "Summary", "Details", _idRec, "Type", "Product", "Reference", "Disposition", "Priority", "Component", "Severity", "Date", "Created By", "Estimation", "Schedule Order", "Assigned User", "", "Schedule Order" };
	public static string _RepTable = "[TT_RES].[DBO].[REPORTBY]";

	public static void UnLocktask(string ttid, string lockid)
	{
		lock (thisLock)
		{
			if (locker.Keys.Contains(ttid))
			{
				LockEvent ev = locker[ttid];
				if (ev.lockid == lockid)
				{
					locker.TryRemove(ttid, out ev);
				}
			}
		}
	}
	public static bool Locked(string ttid)
	{
		if (!locker.Keys.Contains(ttid))
			return false;

		LockEvent ev = locker[ttid];
		return !ev.Obsolete;
	}
	public static LockInfo Locktask(string ttid, string lockid)
	{
		lock (thisLock)
		{
			if (locker.Keys.Contains(ttid))
			{
				LockEvent ev = locker[ttid];
				if (ev.Obsolete)
				{
					LockEvent newev = new LockEvent(lockid);
					locker[ttid] = newev;
					return new LockInfo(newev.usr, newev.lockid);
				}
				else
				{
					if (ev.lockid == lockid)
					{
						ev.Prolongate();
						locker[ttid] = ev;
					}
					return new LockInfo(ev.usr, ev.lockid);
				}
			}
			else
			{
				LockEvent ev = new LockEvent(lockid);
				locker[ttid] = ev;
				return new LockInfo(ev.usr, ev.lockid);
			}
		}
	}
	public static int GetRepRecByTTID(int id)
	{
		return Convert.ToInt32(GetRecdata(_RepTable, _idRec, "IDDEFREC", id));
	}
	string _HistoryChanges = "";
	protected override void PreStore()
	{
		_HistoryChanges = "";
	}
	protected override void PostStore()
	{
		if (!string.IsNullOrEmpty(_HistoryChanges))
		{
			DefectHistory.AddHisotoryByTask(IDREC, _HistoryChanges);
			_HistoryChanges = "";
		}
	}
	protected override void OnChangeColumn(string col, string val)
	{
		for (int i = 0; i < _allcols.Length; i++)
		{
			if (_allcols[i] == col && !string.IsNullOrEmpty(_allcolsNames[i]))
			{
				if (string.IsNullOrEmpty(_HistoryChanges))
				{
					_HistoryChanges = "Fields changed: " + _allcolsNames[i];
				}
				else
				{
					_HistoryChanges += ", " + _allcolsNames[i];
				}
				return;
			}
		}
		base.OnChangeColumn(col, val);
	}
	protected override void OnProcessComplexColumn(string col, object val)
	{
		if (col == _Desc)
		{
			string sql = string.Format("UPDATE {2} SET DESCRPTN = '{0}' WHERE IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {1})", val.ToString().Replace("'", "\""), _id, _RepTable, _Tabl);
			SQLExecute(sql);
			return;
		}
		if (col == _Specs)
		{
			string sql = string.Format("UPDATE {2} SET REPROSTEPS = '{0}' WHERE IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {1})", val.ToString().Replace("'", "\""), _id, _RepTable, _Tabl);
			SQLExecute(sql);
			return;
		}
		else if (col == _Est)
		{
			DefectEvent.AddEventByTask(IDREC, DefectEvent.Eventtype.estimated, "", ESTIM);
			return;
		}
		else if (col == _AsUser)
		{
			DefectEvent.AddEventByTask(IDREC, DefectEvent.Eventtype.assigned, "", -1, Convert.ToInt32(AUSER));
			return;
		}
		else
		{
			base.OnProcessComplexColumn(col, val);
		}
	}
	protected override string OnTransformCol(string col)
	{
		if (col == _Desc)
		{
			return string.Format("(SELECT R.DESCRPTN FROM {2} R WHERE R.IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {0})) {1}", _id, _Desc, _RepTable, _Tabl);
		}
		else if (col == _Specs)
		{
			return string.Format("(SELECT R.REPROSTEPS FROM {2} R WHERE R.IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {0})) {1}", _id, _Specs, _RepTable, _Tabl);
		}
		return base.OnTransformCol(col);
	}
	protected override bool IsColumnComplex(string col)
	{
		if (col == _Est || col == _AsUser)
			return true;

		return base.IsColumnComplex(col);
	}
	public string DESCR
	{
		get { return this[_Desc].ToString(); }
		set { this[_Desc] = value; }
	}
	public string SPECS
	{
		get { return this[_Specs].ToString(); }
		set { this[_Specs] = value; }
	}
	public string CREATEDBY
	{
		get { return this[_Crea].ToString(); }
		set { this[_Crea] = Convert.ToUInt32(value); }
	}
	public string PRIO
	{
		get { return this[_Prio].ToString(); }
		set { this[_Prio] = Convert.ToInt32(value); }
	}
	public string REFERENCE
	{
		get { return this[_Ref].ToString(); }
		set { this[_Ref] = value; }
	}
	public string TYPE
	{
		get { return this[_Type].ToString(); }
		set { this[_Type] = Convert.ToInt32(value); }
	}
	public string PRODUCT
	{
		get { return this[_Prod].ToString(); }
		set { this[_Prod] = Convert.ToInt32(value); }
	}
	protected override void OnBackOrderChanged()
	{
		this[_sMod] = CurrentContext.User.EMAIL;
	}
	public Defect()
		: base(_Tabl, _allcols, "0", _ID, false)
	{
	}
	public Defect(int ttid)
		: base(_Tabl,
					_allcols,
					ttid.ToString(),
					_ID)
	{
	}
	public static int GetIDbyTT(int tt)
	{
		return Convert.ToInt32(GetRecdata(_Tabl, _idRec, _ID, tt));
	}
}