using System;
using System.Globalization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class LockInfo
{
	public LockInfo() { }
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
	public LockEvent(string id, string userid)
	{
		Prolongate();
		lockid = id;
		usr = userid;
	}
	public void Prolongate()
	{
		locktime = DateTime.Now;
	}
	public bool Obsolete
	{
		get
		{
			return DateTime.Now.Subtract(locktime).TotalSeconds > 10;
		}
	}
	DateTime locktime { get; set; }
	public string lockid { get; set; }
	public string usr { get; set; }
}
public partial class DefectBase : IdBasedObject
{
	public static string _idRec = "idRecord";
	public static string _ID = "DefectNum";
	protected static string _Summ = "Summary";
	protected static string _Disp = "idDisposit";
	protected static string _Est = "Estim";
	protected static string _Order = "iOrder";
	protected static string _BackOrder = "BackOrder";
	protected static string _AsUser = "idUsr";
	protected static string _Seve = "idSeverity";
	protected static string _sMod = "sModifier";
	protected static string _sModTRID = "sModifierTRID";
	protected static string _Comp = "idCompon";
	protected static string _Date = "dateEnter";
	protected static string _Created = "dateCreate";
	protected static string _CreaBy = "idCreateBy";
	protected static string _Type = "idType";
	protected static string _Prod = "idProduct";
	protected static string _Ref = "Reference";
	protected static string _Prio = "idPriority";
	protected static string _OrderDate = "IOrderDate";
	protected static string _ModDate = "dateModify";
	protected static string _ModBy = "idModifyBy";

	public static string _Tabl = "[TT_RES].[DBO].[DEFECTS]";

	protected static string[] _allBaseCols = new string[] { _ID, _Summ, _idRec, _Disp, _Est, _Order, _AsUser, _Seve, _sMod, _BackOrder, _Comp, _Date, _Created, _CreaBy, _Type, _Prod, _Ref, _Prio, _OrderDate, _ModDate, _ModBy, _sModTRID};
	protected static string[] _allBaseColsNames = new string[] { _ID, "Summary", _idRec, "Disposition", "Estimation", "Schedule Order", "Assigned User", "Severity", "", "Schedule Order", "Component", "Date Entered", "Date Created", "Created By", "Type", "Product", "Reference", "Priority", "Schedule Date", "", "", ""};

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
	public bool SICK
	{
		get { return SUMMARY.ToUpper().Contains("SICK"); }
		set { }
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
	public int SMODTRID
	{
		get { return this[_sModTRID] == DBNull.Value ? -1 : Convert.ToInt32(this[_sModTRID]); }
		set { this[_sModTRID] = Convert.ToInt32(value); _id = ID.ToString(); }
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
		get { return GetAsDate(_Date); }
		set { SetAsDate(_Date, value); }
	}
	public string CREATED
	{
		get { return GetAsDate(_Created); }
		set { SetAsDate(_Created, value); }
	}
	public string MODIFIED
	{
		get { return GetAsDate(_ModDate); }
		set { SetAsDate(_ModDate, value); }
	}
	public decimal MODIFIEDBY
	{
		get { return Convert.ToDecimal(this[_ModBy]); }
		set { this[_ModBy] = value; }
	}
	public decimal CREATEDBY
	{
		get { return Convert.ToDecimal(this[_CreaBy]); }
		set { this[_CreaBy] = Convert.ToDecimal(value); }
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

	protected override void OnProcessComplexColumn(string col, object val)
	{
		if (col == _sModTRID)
		{
			return;//readonly
		}
		else if (col == _BackOrder)
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
		if (col == _sModTRID)
		{
			return string.Format("(SELECT P.{0} FROM {1} P WHERE UPPER(P.{2}) = UPPER({3})) {4}", MPSUser._pid, MPSUser._Tabl, MPSUser._email, _sMod, _sModTRID);
		}
		else if (col == _Order)
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
		if (col == _Order || col == _BackOrder || col == _sModTRID)
			return true;

		return base.IsColumnComplex(col);
	}
	protected override void PostStore()
	{
		NotifyHub.NotifyPlanChange((new DefectUser(int.Parse(AUSER)).TRID));
	}

	public DefectBase()
		: base(_Tabl, _allBaseCols, "0", _ID, false)
	{
	}
	public DefectBase(int ttid)
		: base(_Tabl,
					_allBaseCols,
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
		return EnumPlanLim(userid, 20);
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
	public static string GetTaskDispName(int ttid)
	{
		return DBHelper.GetValue(string.Format("SELECT 'TT{3} ' + {0} FROM {1} WHERE {2} = {3}", _Summ, _Tabl, _ID, ttid)).ToString();
	}
	static DefectsFilter UnusedVacations()
	{
		DefectsFilter f = new DefectsFilter();
		f.components = new List<int>(DefectComp.GetVacationRec());
		f.dispositions = new List<int>(DefectDispo.EnumCannotStart());
		return f;
	}
	public static List<DefectBase> EnumUnusedVacations()
	{
		return (new DefectBase()).Enum(UnusedVacations(), 2000);
	}
	public static int CountUnusedVacations()
	{
		return (new DefectBase()).EnumCount(UnusedVacations());
	}
	public static List<DefectBase> EnumCloseVacations(string startdate, int days = 15)
	{
		DefectsFilter f = new DefectsFilter();
		f.components = new List<int>(DefectComp.GetVacationRec());
		f.dispositions = new List<int>(DefectDispo.EnumCanStart());
		f.startDateEnter = startdate;
		f.endDateEnter = DateTime.ParseExact(startdate, defDateFormat, CultureInfo.InvariantCulture).AddDays(days).ToString(defDateFormat);//two weeks adnvance
		return (new DefectBase()).Enum(f, 2000);
	}
	public static List<DefectBase> EnumScheduled(string date, string email)
	{
		DefectsFilter f = new DefectsFilter();
		f.startDateScheduled = date;
		f.endDateScheduled = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(defDateFormat);
		f.orderer = email;
		DefectBase temp = new DefectBase();
		return temp.Enum(f, 100);
	}
	public static List<DefectBase> EnumCreated(string date, string email)
	{
		DefectsFilter f = new DefectsFilter();
		f.startDateCreated = date;
		f.endDateCreated = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(defDateFormat);
		DefectUser u = DefectUser.FindByEmail(email);
		if (u != null)
		{
			f.createdUsers = new List<int>() { u.ID };
		}
		DefectBase temp = new DefectBase();
		return temp.Enum(f, 100);
	}
	public static List<DefectBase> EnumModified(string date, string email)
	{
		DefectsFilter f = new DefectsFilter();
		f.startDateModified = date;
		f.endDateModified = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(defDateFormat);
		DefectUser u = DefectUser.FindByEmail(email);
		if (u != null)
		{
			f.modifiedUsers = new List<int>() { u.ID };
		}
		DefectBase temp = new DefectBase();
		return temp.Enum(f, 100);
	}
}
public partial class Defect : DefectBase
{
	static ConcurrentDictionary<string, LockEvent> locker = new ConcurrentDictionary<string, LockEvent>();
	static Object thisLock = new Object();

	static public string _DescInt = "DESCRPTN";
	static protected string _Desc = "DESCR";
	static protected string _Specs = "ReproSteps";
	static protected string _workar = "Workaround";

	static string[] _allcols = _allBaseCols.Concat(new string[] { _Specs, _Desc, _workar }).ToArray();
	static string[] _allcolsNames = _allBaseColsNames.Concat(new string[] { "Specification", "Details", "BST steps" }).ToArray();
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
	public static LockInfo Locktask(string ttid, string lockid, string userid)
	{
		lock (thisLock)
		{
			if (locker.Keys.Contains(ttid))
			{
				LockEvent ev = locker[ttid];
				if (ev.Obsolete)
				{
					LockEvent newev = new LockEvent(lockid, userid);
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
				LockEvent ev = new LockEvent(lockid, userid);
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
		if (IsModified())
		{
			MODIFIEDBY = CurrentContext.User.TTUSERID;
			MODIFIED = DateTime.UtcNow.ToString(defDateFormat, CultureInfo.InvariantCulture);
			if (IsModifiedCol(_Disp) && !IsModifiedCol(_Date))
			{
				var com = Convert.ToInt32(COMP);
				var i = DefectComp.GetVacationRec().FindIndex(x => x == com);
				if (i == -1)//change date enter all except vacations.
				{
					DATE = DateTime.Today.ToString(defDateFormat, CultureInfo.InvariantCulture);
				}
			}
		}
		_HistoryChanges = "";
	}
	protected override void PostStore()
	{
		if (!string.IsNullOrEmpty(_HistoryChanges))
		{
			DefectHistory.AddHisotoryByTask(IDREC, _HistoryChanges);
			_HistoryChanges = "";
		}
		base.PostStore();
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
			string sql = string.Format("UPDATE {2} SET DESCRPTN = '{0}' WHERE IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {1})", val.ToString().Replace("'", "''"), _id, _RepTable, _Tabl);
			SQLExecute(sql);
			return;
		}
		if (col == _Specs)
		{
			string sql = string.Format("UPDATE {2} SET REPROSTEPS = '{0}' WHERE IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {1})", val.ToString().Replace("'", "''"), _id, _RepTable, _Tabl);
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
	public string BST
	{
		get { return this[_workar].ToString(); }
		set { this[_workar] = value; }
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
	public static int GetTTbyID(int id)
	{
		return Convert.ToInt32(GetRecdata(_Tabl, _ID, _idRec, id));
	}
}