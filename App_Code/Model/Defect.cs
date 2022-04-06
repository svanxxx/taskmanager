using System;
using System.Globalization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
	protected static string _PrjID = "ProjectID";
	protected static string _Summ = "Summary";
	protected static string _Stat = "Status";
	protected static string _StatI = "InitStatus";
	public static string _Disp = "idDisposit";
	protected static string _Est = "Estim";
	protected static string _PrimaryHours = "PrimaryHours";
	protected static string _PrimaryEstim = "PrimaryEstim";
	protected static string _Spent = "Spent";
	protected static string _EstId = "idEstim";
	protected static string _Order = "iOrder";
	protected static string _BackOrder = "BackOrder";
	protected static string _AsUser = "idUsr";
	protected static string _AdLoc = "AddLocat";
	public static string _Seve = "idSeverity";
	protected static string _sMod = "sModifier";
	protected static string _sModTRID = "sModifierTRID";
	protected static string _Comp = "idCompon";
	protected static string _Date = "dateEnter";
	protected static string _DateT = "dateTimer";
	protected static string _Created = "dateCreate";
	protected static string _CreaBy = "idCreateBy";
	protected static string _Type = "idType";
	protected static string _Prod = "idProduct";
	protected static string _idEntr = "idEnterBy";
	protected static string _Ref = "Reference";
	protected static string _Prio = "idPriority";
	protected static string _OrderDate = "IOrderDate";
	protected static string _ModDate = "dateModify";
	protected static string _ModBy = "idModifyBy";
	protected static string _branch = "branch";
	protected static string _branchBST = "branchBST";
	protected static string _buildP = "iBuildPriority";
	protected static string _vers = "Version";
	protected static string _edd = "EDD";

	public static string _Tabl = "[TT_RES].[DBO].[DEFECTS]";

	protected static string[] _allBaseCols = new string[] { _ID, _Summ, _idRec, _Disp, _Est, _Spent, _EstId, _Order, _AsUser, _Seve, _sMod, _BackOrder, _Comp, _Date, _Created, _DateT, _CreaBy, _Type, _Prod, _Ref, _Prio, _OrderDate, _ModDate, _ModBy, _sModTRID, _branch, _branchBST, _buildP, _vers, _edd, _PrimaryHours, _PrimaryEstim };
	protected static string[] _allBaseColsNames = new string[] { _ID, "Summary", _idRec, "Disposition", "Estimation", "", "Estimated by", "Schedule Order", "Assigned User", "Severity", "", "Schedule Order", "Component", "Date Entered", "Date Created", "Alarm", "Created By", "Type", "Product", "Reference", "Priority", "Schedule Date", "", "", "", "Branch", "BST Branch", "Test Priority", "Version", "", "Primary Hours", "PrimaryEstim" };

	MPSUser _updater;
	public MPSUser GetUpdater()
	{
		return _updater == null ? CurrentContext.User : _updater;
	}
	public void SetUpdater(MPSUser u)
	{
		_updater = u;
	}
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
	static string NormalizeText(string text)
	{
		return text.Trim().Replace("\n", String.Empty).Replace("\r", String.Empty).Replace("“", "\"").Replace("”", "\"");
	}
	public string SUMMARY
	{
		get { return NormalizeText(this[_Summ].ToString()); }
		set { this[_Summ] = NormalizeText(value); }
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
		set
		{
			if (value != DISPO)
			{
				int ival = Convert.ToInt32(value);
				this[_Disp] = ival;
			}
		}
	}
	public int? PRIMARYHOURS
	{
		get 
		{
			var val = this[_PrimaryHours];
			if (val == DBNull.Value)
			{
				return null;
			}
			return Convert.ToInt32(val); 
		}
		set
		{
			if (value == null)
			{
				this[_PrimaryHours] = DBNull.Value;
			}
			else
			{
				this[_PrimaryHours] = value;
			}
		}
	}
	public int? PRIMARYESTIM
	{
		get
		{
			var val = this[_PrimaryEstim];
			if (val == DBNull.Value)
			{
				return null;
			}
			return Convert.ToInt32(val);
		}
		set
		{
			if (value == null)
			{
				this[_PrimaryEstim] = DBNull.Value;
			}
			else
			{
				this[_PrimaryEstim] = value;
			}
		}
	}
	public int ESTIM
	{
		get { return this[_Est] == DBNull.Value ? 0 : Convert.ToInt32(this[_Est]); }
		set
		{
			if (value < 0 && this[_Est] != DBNull.Value)
			{
				this[_Est] = DBNull.Value;
			}
			else
			{
				this[_Est] = value;
			}
		}
	}
	public int SPENT
	{
		get { return this[_Spent] == DBNull.Value ? 0 : Convert.ToInt32(this[_Spent]); }
		set
		{
			if (value < 0 && this[_Spent] != DBNull.Value)
			{
				this[_Spent] = DBNull.Value;
			}
			else
			{
				this[_Spent] = value;
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
			if (value < 0)
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
				this[_sMod] = GetUpdater().EMAIL;
			}
			if (value < 0)
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
		set
		{
			if (DATE != value)
			{
				SetAsDate(_Date, value);
			}
		}
	}
	public bool FIRE
	{
		get
		{
			object o = this[_DateT];
			if (o == DBNull.Value)
			{
				return false;
			}
			DateTime d = Convert.ToDateTime(this[_DateT]);
			return (DateTime.Today - d).TotalDays <= 0;
		}
		set { }
	}
	public string TIMER
	{
		get { return GetAsDate(_DateT, ""); }
		set
		{
			if (TIMER != value)
			{
				SetAsDate(_DateT, value);
			}
		}
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
	public string EDD
	{
		get
		{
			if (this[_edd] == DBNull.Value)
			{
				return "";
			}
			return Convert.ToDateTime(this[_edd]).ToShortDateString();
		}
	}
	public void SetEDD(DateTime dt)
	{
		object o = this[_edd];
		if (o != DBNull.Value && Convert.ToDateTime(o).Date == dt.Date)
		{
			return;
		}
		//direct sql - no history records should be stored, no notifications as mass update of database
		SQLExecute($"UPDATE {_Tabl} SET {_edd} = '{dt.ToString(DBHelper.SQLDateFormat)}' WHERE {_idRec} = {IDREC}");
		return;
	}
	public decimal ESTIMBY
	{
		get
		{
			if (this[_EstId] == DBNull.Value)
			{
				return GetUpdater().TTUSERID;
			}
			return Convert.ToDecimal(this[_EstId]);
		}
		set { this[_EstId] = value; }
	}
	public string BRANCH
	{
		get
		{
			string val = this[_branch].ToString().Trim();
			return (string.IsNullOrEmpty(val)) ? "TT" + ID.ToString() : val;
		}
		set
		{
			if (value != "TT" + ID.ToString())
			{
				this[_branch] = value;
			}
		}
	}
	public string BRANCHBST
	{
		get
		{
			return this[_branchBST].ToString().Trim();
		}
		set
		{
			if (value != BRANCHBST)
			{
				this[_branchBST] = value;
			}
		}
	}
	public string VERSION
	{
		get
		{
			return GetAsString(_vers);
		}
		set
		{
			this[_vers] = value;
		}
	}
	public string TESTPRIORITY
	{
		get { return GetAsInt(_buildP, 4).ToString(); }
		set { this[_buildP] = int.Parse(value); }
	}
	public DateTime? GetCreated()
	{
		return ToDateTime(_Created);
	}
	public DateTime? GetEDD()
	{
		return ToDateTime(_edd);
	}
	public int GetDispo()
	{
		return GetAsInt(_Disp);
	}
	protected override void OnProcessComplexColumn(string col, object val)
	{
		if (col == _sModTRID)
		{
			return;//readonly
		}
		else if (col == _BackOrder)
		{
			//do not change order of unassigned tasks. skip order modification
			if (string.IsNullOrEmpty(AUSER) || IsModifiedCol(_Order))
			{
				return;
			}
			string sqlupdate = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", _Tabl, _Order, BACKORDER, _idRec, IDREC);
			SQLExecute(sqlupdate);
			return;
		}
		if (col == _Order)
		{
			if (string.IsNullOrEmpty(AUSER))
			{
				return;
			}
			if (val == DBNull.Value)
			{
				string sqlupdate = string.Format("UPDATE {0} SET {1} = NULL WHERE {3} = {4}", _Tabl, _Order, val, _idRec, IDREC);
				SQLExecute(sqlupdate);
			}
			else
			{
				int ord = Convert.ToInt32(val);
				string where1 = DefectDispo.PlanableDefectFilter();
				string where2 = DefectSeverity.PlanableDefectFilter();
				string sql = $@"
				SELECT MIN({_Order}) FROM
				(
					SELECT TOP {ord} * FROM 
					(SELECT {_Order} FROM {_Tabl} WHERE {_AsUser} = {AUSER} AND {_Order} IS NOT NULL {where1} {where2} AND {_idRec} <> {IDREC} GROUP BY {_Order}) T
					ORDER BY 1 DESC
				) A";

				object o = GetValue(sql);
				if (o != DBNull.Value)
				{
					string sqlupdate = $"UPDATE {_Tabl} SET {_Order} = {_Order} + 1 WHERE {_Order} > {Convert.ToInt32(o)} AND {_AsUser} = {AUSER} {where1} {where2}";
					SQLExecute(sqlupdate);
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
		else if (col == _EstId)
		{
			return _EstId;
		}
		else if (col == _Order)
		{
			string where1 = DefectDispo.PlanableDefectFilter();
			string where2 = DefectSeverity.PlanableDefectFilter();
			return $"(CASE WHEN {_Tabl}.{_Order} IS NULL THEN NULL ELSE (SELECT COUNT(*) + 1 FROM {_Tabl} D2 WHERE D2.IDUSR = {_Tabl}.IDUSR AND D2.{_Order} > {_Tabl}.{_Order} {where1} {where2})END) {_Order}";
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
	string _reassignedFrom = "";
	protected override void OnSetColFromCopy(string col, string val, string newval)
	{
		if (col == _AsUser)
		{
			_reassignedFrom = val;
		}
		base.OnSetColFromCopy(col, val, newval);
	}
	private static readonly object _lockLGM = new object();
	private static DateTime _LGMDate = DateTime.Now.AddDays(-1);
	public static DateTime LastGlobalModifier
	{
		set
		{
			lock (_lockLGM)
			{
				_LGMDate = value;
			}
		}
		get
		{
			lock (_lockLGM)
			{
				return _LGMDate;
			}
		}
	}
	protected override void PostStore()
	{
		if (this.IsModified())
		{
			LastGlobalModifier = DateTime.Now;
		}
		if (IsModifiedCol(_AsUser))
		{
			if (!string.IsNullOrEmpty(_reassignedFrom))
			{
				NotifyHub.NotifyPlanChange((new DefectUser(int.Parse(_reassignedFrom)).TRID));
			}

			//when task is reassigned order in new list should be changed.
			DefectBase d = new DefectBase(ID);
			if (d.ORDER != ORDER)
			{
				d.ORDER = ORDER;
				d.Store();
				return;
			}
		}

		if (!string.IsNullOrEmpty(AUSER))
		{
			NotifyHub.NotifyPlanChange((new DefectUser(int.Parse(AUSER)).TRID));
		}
		NotifyHub.NotifyDefectChange(ID);
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
	public DefectBase(string ttid)
		 : base(_Tabl,
						 _allBaseCols,
						 ttid,
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
		string w_where = DefectDispo.PlanableDefectFilter();
		string w_where2 = DefectSeverity.PlanableDefectFilter();
		List<DefectBase> ls = new List<DefectBase>();
		string where = string.Format(" WHERE (({0} = {1}) AND ({2} is not null) {3} {5}) ORDER BY {4}.{2} DESC", _AsUser, userid, _Order, w_where, _Tabl, w_where2);
		foreach (DataRow r in GetRecords(where, max))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			d._id = r[_ID].ToString();
			ls.Add(d);
		}
		return ls;
	}
	public List<DefectBase> EnumUnPlan(int userid)
	{
		string w_where1 = DefectDispo.PlanableDefectFilter();
		string w_where2 = DefectSeverity.PlanableDefectFilter();
		List<DefectBase> ls = new List<DefectBase>();
		string where = string.Format(" WHERE (({0} = {1}) AND ({2} is null) {3} {4}) ORDER BY {5} DESC", _AsUser, userid, _Order, w_where1, w_where2, _ID);
		foreach (DataRow r in GetRecords(where))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			d._id = r[_ID].ToString();
			ls.Add(d);
		}
		return ls;
	}
	public static string GetTaskDispName(int ttid)
	{
		if (ttid <= 0)
		{
			return "";
		}
		return DBHelper.GetValue(string.Format("SELECT {0} FROM {1} WHERE {2} = {3}", _Summ, _Tabl, _ID, ttid)).ToString();
	}
	public static string GetTaskEstim(int ttid)
	{
		return DBHelper.GetValue(string.Format("SELECT {0} FROM {1} WHERE {2} = {3}", _Est, _Tabl, _ID, ttid)).ToString();
	}
	public string GetTaskUserName()
	{
		if (AUSER == "")
		{
			return "Unassigned";
		}
		DefectUser u = new DefectUser(AUSER);
		return u.FULLNAME;
	}
	public string GetTaskDispoName()
	{
		if (DISPO == "")
		{
			return "";
		}
		DefectDispo d = new DefectDispo(int.Parse(DISPO));
		return d.DESCR;
	}
	static DefectsFilter UnusedVacations()
	{
		DefectsFilter f = new DefectsFilter();
		f.components = new List<int>(DefectComp.GetVacationRec());
		f.dispositions = new List<int>(DefectDispo.EnumCannotStartIDs());
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

	static protected string _bstsep = "====bst_data_separator====";
	static public string _DescInt = "DESCRPTN";
	static protected string _Desc = "DESCR";
	static public string _Specs = "ReproSteps";
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
	public static LockInfo Locktask(string ttid, string lockid, string userid, bool force = false)
	{
		lock (thisLock)
		{
			if (locker.Keys.Contains(ttid))
			{
				LockEvent ev = locker[ttid];
				if (ev.Obsolete || force)
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
		base.PreStore();
		if (IsModified())
		{
			MODIFIEDBY = GetUpdater().TTUSERID;
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
			if (IsModifiedCol(_Disp))
			{
				int ival = int.Parse(DISPO);
				if (!PRIMARYESTIM.HasValue && DefectDispo.EnumWorkNow().Where(x => x.ID == ival).FirstOrDefault() != null)
				{
					PRIMARYESTIM = ESTIM;
				}
			}
			if (IsModifiedCol(_Est))
			{
				if (PRIMARYESTIM.HasValue && PRIMARYESTIM.Value > 0 && ESTIM > 0 && ESTIM / PRIMARYESTIM.Value >= 2.0)
				{
					TasksBot.EstimationAlarm(this.ID, PRIMARYESTIM.Value, ESTIM);
				}
			}
		}
		_HistoryChanges = "";
	}
	protected override void PostStore()
	{
		if (REQUESTRESET)
		{
			DefectHistory.DelHisotoryByTask(IDREC);
			DefectEvent.DelHisotoryByTask(IDREC);
			_HistoryChanges = "Task was reset.";
		}
		if (!string.IsNullOrEmpty(_HistoryChanges))
		{
			DefectHistory.AddHisotoryByTask(IDREC, _HistoryChanges, GetUpdater().TTUSERID.ToString());
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
		else if (col == _Est || col == _EstId)
		{
			DefectEvent.AddEventByTask(IDREC, DefectEvent.Eventtype.estimated, ESTIMBY, "", ESTIM);
			return;
		}
		else if (col == _AsUser)
		{
			if (!string.IsNullOrEmpty(AUSER))
			{
				DefectEvent.AddEventByTask(IDREC, DefectEvent.Eventtype.assigned, GetUpdater().TTUSERID, "", -1, Convert.ToInt32(AUSER));
			}
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
	public bool REQUESTRESET
	{
		get; set;
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
	public string BSTBATCHES
	{
		get
		{
			string[] vals = this[_workar].ToString().Split(new string[] { _bstsep }, StringSplitOptions.None);
			if (vals.Length > 0)
			{
				return vals[0].Trim();
			}
			return "";
		}
		set
		{
			List<string> vals = new List<string>(this[_workar].ToString().Split(new string[] { _bstsep }, StringSplitOptions.None));
			if (vals.Count > 0)
			{
				vals[0] = value.Trim();
			}
			else
			{
				vals.Add(value.Trim());
			}
			this[_workar] = string.Join(_bstsep, vals.ToArray());
		}
	}
	public string BSTCOMMANDS
	{
		get
		{
			string[] vals = this[_workar].ToString().Split(new string[] { _bstsep }, StringSplitOptions.None);
			if (vals.Length > 1)
			{
				return vals[1].Trim();
			}
			return "";
		}
		set
		{
			List<string> vals = new List<string>(this[_workar].ToString().Split(new string[] { _bstsep }, StringSplitOptions.None));
			if (vals.Count > 1)
			{
				vals[1] = value.Trim();
			}
			else
			{
				vals.Add(value.Trim());
			}
			this[_workar] = string.Join(_bstsep, vals.ToArray());
		}
	}
	public void AddMessage(string mess, int userid)
	{
		MPSUser u = new MPSUser(userid);
		DESCR = $@"
<task-message clr='undefined'  userid='{userid}' user='{u.LOGIN}' time='{DateTime.Now.ToString()}'>
{mess}
</task-message>
" + DESCR.Trim();
	}
	protected override void OnBackOrderChanged()
	{
		this[_sMod] = GetUpdater().EMAIL;
	}
	public void From(Defect d)
	{
		ESTIM = d.ESTIM;
		TYPE = d.TYPE;
		PRODUCT = d.PRODUCT;
		PRIO = d.PRIO;
		COMP = d.COMP;
		SEVE = d.SEVE;
		DATE = d.DATE;
		AUSER = d.AUSER;
		SUMMARY = d.SUMMARY;
		DESCR = d.DESCR;
		SPECS = d.SPECS;
		BSTBATCHES = d.BSTBATCHES;
		BSTCOMMANDS = d.BSTCOMMANDS;
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
	public Defect(string ttid)
		 : base(_Tabl,
						 _allcols,
						 ttid,
						 _ID)
	{
	}
	public static bool GetIDbyTT(int tt, out int id)
	{
		id = -1;
		object o = GetRecdata(_Tabl, _idRec, _ID, tt);
		if (o == null)
		{
			return false;
		}
		id = Convert.ToInt32(o);
		return true;
	}
	public static int GetIDbyTT(int tt)
	{
		return Convert.ToInt32(GetRecdata(_Tabl, _idRec, _ID, tt));
	}
	public static int GetTTbyID(int id)
	{
		return Convert.ToInt32(GetRecdata(_Tabl, _ID, _idRec, id));
	}
	static object _lockobject = new object();
	public static int New(string summary)
	{
		lock (_lockobject)
		{
			string sql = $@"
			INSERT INTO {_Tabl}
			(
				{_PrjID}, 
				{_idRec}, 
				{_Created}, 
				{_CreaBy}, 
				{_ModDate}, 
				{_ModBy}, 
				{_ID}, 
				{_Summ}, 
				{_Stat}, 
				{_StatI}, 
				{_Type}, 
				{_Prod}, 
				{_idEntr}, 
				{_Disp}, 
				{_Prio}, 
				{_Comp}, 
				{_Seve}, 
				{_Date}, 
				{_AdLoc})
			values
			(
			   1
			 , (SELECT MAX(T1.{_idRec}) + 1 FROM {_Tabl} T1)
			 , GETUTCDATE()
			 , {CurrentContext.TTUSERID}
			 , GETUTCDATE()
			 , {CurrentContext.TTUSERID}
			 , (SELECT MAX(T1.{_ID}) + 1 FROM {_Tabl} T1)
			 , ?
			 , 1
			 , 1
			 , {DefectDefaults.CurrentDefaults.TYPE}
			 , {DefectDefaults.CurrentDefaults.PRODUCT}
			 , {CurrentContext.TTUSERID}
			 , {DefectDefaults.CurrentDefaults.DISP}
			 , {DefectDefaults.CurrentDefaults.PRIO}
			 , {DefectDefaults.CurrentDefaults.COMP}
			 , {DefectDefaults.CurrentDefaults.SEVR}
			 , GETUTCDATE()
			 , 1
			)";

			SQLExecute(sql, new object[] { summary });
			string sqlid = string.Format("SELECT TOP 1 {0} FROM {1} WHERE {2} = {3} ORDER BY {0} DESC", _ID, _Tabl, _CreaBy, CurrentContext.TTUSERID);
			int ttid = Convert.ToInt32(GetValue(sqlid));
			int recid = Defect.GetIDbyTT(ttid);

			string sqlrep = string.Format(@"
				INSERT INTO {0}(IDRECORD, PROJECTID, IDFOUNDBY, DATEFOUND, ORDERNUM, IDREPROD, TSTCONTYPE, IDCONFIG, IDDEFREC)
				VALUES((SELECT MAX(R2.IDRECORD) + 1 FROM {0} R2), 1, {1}, GETUTCDATE(), 1, 0, 1, 4294967293, {2})
			", _RepTable, CurrentContext.TTUSERID, recid);

			SQLExecute(sqlrep);

			return ttid;
		}
	}
}
public class DefectPlan
{
	public DefectPlan() { }
	public DefectPlan(DefectBase db)
	{
		this.ID = db.ID;
		this.SUMMARY = db.SUMMARY;
		this.SMODTRID = db.SMODTRID;
		this.SMODIFIER = db.SMODIFIER;
		this.DISPO = int.Parse(db.DISPO);
		this.SPENT = db.SPENT;
		this.IDREC = db.IDREC;
		this.ESTIM = db.ESTIM;
		this.FIRE = db.FIRE;
		this.VERSION = db.VERSION;
		this.AUSER = db.AUSER;
		this.ORDER = db.ORDER;
		this.EDD = db.EDD;
		this.PRIMARYESTIM = db.PRIMARYESTIM;
		this.TYPE = db.TYPE;
	}
	public bool FIRE { get; set; }
	public int ESTIM { get; set; }
	public int IDREC { get; set; }
	public int SPENT { get; set; }
	public string AUSER { get; set; }
	public int SMODTRID { get; set; }
	public string SMODIFIER { get; set; }
	public int DISPO { get; set; }
	public int ID { get; set; }
	public string SUMMARY { get; set; }
	public string VERSION { get; set; }
	public int ORDER { get; set; }
	public string EDD { get; set; }
	public int? PRIMARYESTIM { get; set; }
	public string TYPE { get; set; }
	static public List<DefectPlan> Convert2Plan(List<DefectBase> ls)
	{
		List<DefectPlan> lsout = new List<DefectPlan>();
		foreach (var def in ls)
		{
			lsout.Add(new DefectPlan(def));
		}
		return lsout;
	}
	private static readonly CancellationTokenSource _cancelEDD = new CancellationTokenSource();
	public static void StopUpdaterEDD()
	{
		_cancelEDD.Cancel();
	}
	public static void SatrtUpdaterEDD()
	{
		Task.Run(async () =>
		{
			await Task.Delay(TimeSpan.FromMinutes(1), _cancelEDD.Token);
			while (true)
			{
				try
				{
					UpdateEDD();
				}
				catch (Exception e) { Logger.Log(e); };
				await Task.Delay(TimeSpan.FromMinutes(20), _cancelEDD.Token);
			}
		}, _cancelEDD.Token);
	}
	private static readonly object _lockEDD = new object();
	private static DateTime _EDDDate = DateTime.Now;
	public static void UpdateEDD()
	{
		//one mass update per time
		lock (_lockEDD)
		{
			if (DateTime.Compare(_EDDDate, DefectBase.LastGlobalModifier) > 0)
			{
				return;
			}

			//cache all vacations
			List<DefectBase> vacs = Vacations.EnumCloseVacations(DateTime.Today, 365);

			DefectBase d = new DefectBase();
			foreach (var u in DefectUser.Enum())
			{
				if (!u.ACTIVE)
				{
					continue;
				}
				string sid = u.ID.ToString();
				List<DateTime> vacDates = new List<DateTime>();
				foreach (var v in vacs)
				{
					if (v.AUSER == sid)
					{
						vacDates.Add(DateTime.ParseExact(v.DATE, IdBasedObject.defDateFormat, CultureInfo.InvariantCulture));
					}
				}

				DateTime dat = DateTime.Today.AddDays(1);

				foreach (var task in d.EnumPlan(u.ID))
				{
					while (dat.DayOfWeek == DayOfWeek.Saturday || dat.DayOfWeek == DayOfWeek.Sunday)
					{
						dat = dat.AddDays(1);
					}
					while (vacDates.Exists(x => dat.Date.CompareTo(x) == 0))
					{
						dat = dat.AddDays(1);
					}
					int hours = Math.Max(task.ESTIM - task.SPENT, 0) + dat.Hour;
					dat = dat.AddHours(-dat.Hour);
					int days = hours / 8;
					hours = hours % 8;
					dat = dat.AddDays(days);
					dat = dat.AddHours(hours);
					task.SetEDD(dat.Date);
				}
			}
			_EDDDate = DateTime.Now;
		}
	}
}