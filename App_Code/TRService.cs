using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using GTOHELPER;
using System.Net.Mail;
using System.Globalization;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class TRService : System.Web.Services.WebService
{
	static public string defDateFormat = "MM-dd-yyyy";
	static public string SQLDateFormat = "yyyy-MM-dd HH:mm:ss";
	public struct DoStruct
	{
		public string name;
		public string count;
		public DoStruct(DataRow dr) 
		{
			name = dr["NAM"].ToString();
			count = dr["NUM"].ToString();
		}
	}
	public struct TTUser
	{
		public string name;
		public string email;
		public string id;
		public string vacation_ttid;
		public string vacations_count;
		public string online;
		public TTUser(DataRow dr)
		{
			id = dr["PERSON_ID"].ToString();
			name = dr["PERSON_NAME"].ToString();
			email = dr["WORK_EMAIL"].ToString();
			vacation_ttid = dr["VACATION_TTID"].ToString();
			vacations_count = dr["VACATIONS_COUNT"].ToString();
			online = dr["USER_ONLINE"].ToString();
		}
	}
	public struct PlanData
	{
		public string ttid;
		public string summary;
		public string usr;
		public string est;
		public string sev;
		public string idsev;
		public string dateenter;
		public string iddisp;
		public string iorder;
		public PlanData(DataRow dr)
		{
			ttid = dr["DefectNum"].ToString();
			summary = dr["Summary"].ToString();
			est = dr["est"].ToString();
			sev = dr["sev"].ToString();
			idsev = dr["idsev"].ToString();
			usr = dr["usr"].ToString();
			iddisp = dr["iddisp"].ToString();
			iorder = dr["iorder"].ToString();
			dateenter = Convert.ToDateTime(DBNull.Value == dr["dateenter"] ? DateTime.Now : dr["dateenter"]).ToString(defDateFormat);
		}
	}
	public struct SevSummary
	{
		public string disp;
		public string num;
		public string est;
		public string iddisp;
		public SevSummary(DataRow dr)
		{
			iddisp = dr["IDDISPOSIT"].ToString();
			disp = dr["disp"].ToString();
			num = dr["num"].ToString();
			est = dr["est"].ToString();
		}
	}
	public struct KeyVal
	{
		public string key;
		public string val;
		public KeyVal(DataRow dr)
		{
			key = dr["id"].ToString();
			val = dr["val"].ToString();
		}
	}
	public struct TWorkDay
	{
		public TWorkDay(DataRow dr)
		{
			date = Convert.ToDateTime(dr["date"]).ToString(defDateFormat, CultureInfo.InvariantCulture);
			day = Convert.ToDateTime(dr["date"]).DayOfYear - 1;
			email = dr["email"].ToString();
		}
		public int day;
		public string date;
		public string email;
	}
	public struct TT
	{
		public string title;
		public string text;
		public string dispo;
		public string prio;
		public string user;
		public string creat_user;
		public string date;
		public string est;
		public TT(string ti, string tx, string dis, string prio, string us, string cr_us, DateTime date, int est)
		{
			this.title = ti;
			this.text = tx;
			this.dispo = dis;
			this.prio = prio;
			this.user = us;
			this.creat_user = cr_us;
			var baseDate = new DateTime(1970, 1, 1);
			TimeSpan ts = new TimeSpan(date.Ticks - baseDate.Ticks);
			this.date = ts.TotalMilliseconds.ToString();
			this.est = est.ToString();
		}
	}
	public struct TTShort
	{
		public string ttid;
		public string summary;
		public string userid;
		public string username;
		public TTShort(DataRow dr)
		{
			ttid = dr["TTID"].ToString();
			summary = dr["SUMMARY"].ToString();
			userid = dr["USRID"].ToString();
			username = dr["USRNAME"].ToString();
		}
	}
	public struct Objective
	{
		public string id;
		public string x;
		public string y;
		public string sizex;
		public string sizey;
		public string idsever;
		public string namesever;
		public string emailed;
		public Objective(DataRow dr)
		{
			this.id = dr["ID"].ToString();
			this.x = dr["X"].ToString();
			this.y = dr["Y"].ToString();
			this.sizex = dr["sizex"].ToString();
			this.sizey = dr["sizey"].ToString();
			this.idsever = dr["idseverity"].ToString();
			this.namesever = dr["namesever"].ToString();
			this.emailed = dr["emailed"].ToString();
		}
	}
	public struct LogEntry
	{
		public string defectnum;
		public string notes;
		public string person_id;
		public LogEntry(DataRow dr)
		{
			defectnum = dr["defectnum"].ToString();
			notes = dr["notes"].ToString();
			person_id = dr["person_id"].ToString();
		}
	}
	public struct DailyRecord
	{
		public string user;
		public string notes;
		public string vacation;
		public DailyRecord(DataRow dr)
		{
			user = dr["WORK_EMAIL"].ToString();
			notes = dr["REPORT_DONE"].ToString();
			vacation = dr["VACATION"].ToString();
		}
	}
	private void EstimateTask(string ttid, string useremail, string est)
	{
		int old = GTOHelper.GetTTEst(ttid);
		if (string.IsNullOrEmpty(useremail) || old == Convert.ToInt32(est))
			return;

		string sql = string.Format(@"
			insert into [tt_res].[dbo].[DEFECTEVTS]
			(ProjectID, idRecord, EvtDefID, OrderNum, ParentID, EvtMUParnt, idUser, dateEvent, Notes, TimeSpent, RsltState, RelVersion, AsgndUsers, GenByType, CreatorID, DefAsgEff, OvrWF, OvrWFUsrID)
			values
			(
			 1 
			 ,(select max(idRecord) + 1 from [tt_res].[dbo].[DEFECTEVTS] E1 where idRecord < 3000000)
			 ,2
			 ,(select max(E1.OrderNum) + 1 from [tt_res].[dbo].[DEFECTEVTS] E1 where E1.ParentID = (select D1.idRecord from [tt_res].[dbo].[DEFECTS] D1 where D1.DefectNum = {0}))
			 ,(select D2.idRecord from [tt_res].[dbo].[DEFECTS] D2 where D2.DefectNum = {0})
			 ,4294967295
			 ,(select U.idRecord from [tt_res].[dbo].[USERS] U where U.EMailAddr = '{1}')
			 ,GETUTCDATE()
			 ,'update from web by user {1}'
			 ,{2}
			 ,0
			 ,' '
			 ,' '
			 ,0
			 ,4294967295
			 ,2
			 ,0
			 ,0
			)		
		", ttid, useremail, est);
		GTOHelper.SQLExecute(sql);
		AddTaskLogEntry(ttid, useremail, "TT" + ttid + ": task was estimated from web interface.");
	}
	private void AddTaskLogEntry(string ttid, string useremail, string text)
	{
		string sql = string.Format(@"
			INSERT INTO [TT_RES].[DBO].[DEFLOG] 
			(IDRECORD, PROJECTID, IDUSER, DATELOG, NOTES, PARENTID)
			VALUES
			(
				(SELECT MAX(DLI.IDRECORD) + 1 FROM [TT_RES].[DBO].[DEFLOG] DLI WHERE DLI.IDRECORD < 3000000)
				,1
				,(SELECT IDRECORD FROM [TT_RES].[DBO].[USERS] WHERE EMAILADDR = '{0}')
				,GETUTCDATE()
				,'{2}'
				,(SELECT IDRECORD FROM [TT_RES].[DBO].[DEFECTS] WHERE DEFECTNUM = {1})
			)
		", useremail, ttid, text);
		GTOHelper.SQLExecute(sql);
	}

	public TRService()
	{
		//Uncomment the following line if using designed components 
		//InitializeComponent(); 
	}
	[WebMethod(EnableSession = true)]
	public void UpdateCheckOut(string userid)
	{
		if (string.IsNullOrEmpty(userid))
			return;

		GTOHelper.SQLExecute("UPDATE REPORTS SET TIME_END = GETDATE() where CAST(REPORT_DATE as DATE) = cast(GETDATE() as date) and PERSON_ID = " + Convert.ToInt32(userid));
	}
	[WebMethod(EnableSession = true)]
	public void UpdatePlan(string user, string tasks, string skiptasks, string modifiedtasks)
	{
		if (string.IsNullOrEmpty(tasks) && string.IsNullOrEmpty(skiptasks))
			return;

		if (string.IsNullOrEmpty(user))
			throw new Exception("user name is empty");

		if (!string.IsNullOrEmpty(skiptasks))
		{
			GTOHelper.SQLExecute("UPDATE [tt_res].[dbo].[DEFECTS] SET IORDER = NULL WHERE DEFECTNUM IN (" + skiptasks.Trim(',') + ")");
		}

		if (!string.IsNullOrEmpty(modifiedtasks))
		{
			GTOHelper.SQLExecute("UPDATE [tt_res].[dbo].[DEFECTS] SET sModifier = '" + GTOHelper.GetUserEmail(Convert.ToInt32(user)) + "' WHERE DEFECTNUM IN (" + modifiedtasks.Trim(',') + ")");
		}

		if (!string.IsNullOrEmpty(tasks))
		{
			string[] ttids = tasks.Split(',');
			string SQL = "UPDATE [tt_res].[dbo].[DEFECTS] SET IORDER = {0} WHERE DEFECTNUM = {1}";
			for (int i = 0; i < ttids.Length; i++)
			{
				int itt;
				if (int.TryParse(ttids[i], out itt))
					GTOHelper.SQLExecute(string.Format(SQL, ttids.Length - i, itt.ToString()));
			}
		}
	}
	[WebMethod(EnableSession = true, CacheDuration = 60)]
	public List<DoStruct> Get2Do()
	{
		List<DoStruct> lst = new List<DoStruct>();
		foreach (DataRow rowCur in GTOHelper.Get2DoCount().Tables[0].Rows)
			lst.Add(new DoStruct(rowCur));
		return lst;
	}
	[WebMethod(EnableSession = true)]
	public string GetTTURL(string TTID)
	{
		return GTOHelper.GetTTLink(TTID);
	}
	[WebMethod(EnableSession = true, CacheDuration = 300)]
	public string GetUserName(string email)
	{
		return GTOHelper.GetUserNameByEmail(email);
	}
	[WebMethod(EnableSession = true)]
	public TT GetTTText(string TTID)
	{
		return new TT(GTOHelper.GetTTTitle(TTID)
							, GTOHelper.GetTTText(TTID)
							, GTOHelper.GetTTDispo(TTID)
							, GTOHelper.GetTTPrio(TTID)
							, GTOHelper.GetTTUsr(TTID)
							, GTOHelper.GetTTCreateUsr(TTID)
							, GTOHelper.GetTTDate(TTID)
							, GTOHelper.GetTTEst(TTID));
	}
	[WebMethod(EnableSession = true, CacheDuration = 10)]
	public string GetTasksUpdateTime()
	{
		string s = GTOHelper.GetDataSet(@"
							use tt_res;
							SELECT    
								max(last_user_update)
							FROM    
								sys.dm_db_index_usage_stats
							WHERE    
								database_id = DB_ID('tt_res')
								and (OBJECT_NAME(object_id) = 'DEFECTS'
								or OBJECT_NAME(object_id) = 'DEFECTEVTS')").Tables[0].Rows[0][0].ToString();
		return s;
	}
	[WebMethod(EnableSession = true)]
	public string GetTRUpdateTime()
	{
		string s = GTOHelper.GetDataSet("SELECT REPUPDATED FROM TRACKER").Tables[0].Rows[0][0].ToString();
		return s;
	}
	[WebMethod(EnableSession = true)]
	public string GetObjUpdateTime()
	{
		string s = GTOHelper.GetDataSet(@"
							SELECT    
								max(last_user_update)
							FROM    
								sys.dm_db_index_usage_stats
							WHERE    
								database_id = DB_ID('TASKS')
								and (OBJECT_NAME(object_id) = 'Objectivies')").Tables[0].Rows[0][0].ToString();
		return s;
	}
	[WebMethod(EnableSession = true)]
	public List<PlanData> GetPlanData(string userid, string count)
	{
		List<PlanData> ls = new List<PlanData>();
		DataSet ds = GTOHelper.GetPlanData(Convert.ToInt32(userid), Convert.ToInt32(count));
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new PlanData(dr));
		return ls;
	}
	[WebMethod(EnableSession = true)]
	public List<string> GetPlan(string userid)
	{
		return new List<string>(GTOHelper.GetPlan(Convert.ToInt32(userid)));
	}
	[WebMethod(EnableSession = true)]
	public void SetTaskStatus(string ttid, string stat, string refadd, string refrepl, string userid)
	{
		string stradd = string.IsNullOrEmpty(refadd) ? "''" : "ISNULL(REFERENCE, '')";
		refadd = string.IsNullOrEmpty(refadd) ? refrepl : refadd;
		GTOHelper.SQLExecute(string.Format("UPDATE [tt_res].[dbo].[DEFECTS] SET IDDISPOSIT = {0}, REFERENCE = '{1}' + {2} WHERE DEFECTNUM IN ({3})", stat, refadd, stradd, GTOHelper.GetTTID(ttid)));
		AddTaskLogEntry(GTOHelper.GetTTID(ttid), GTOHelper.GetUserEmail(Convert.ToInt32(userid)), string.Format("Status changed by user from web interface to: {0}", refadd));
	}
	private string sObjSql = "SELECT *, (SELECT S.DESCRIPTOR FROM [TT_RES].[DBO].[FLDSEVER] S WHERE S.IDRECORD = IDSEVERITY) NAMESEVER FROM OBJECTIVIES";
	[WebMethod(EnableSession = true, CacheDuration = 300)]
	public List<Objective> EnumObectivies()
	{
		List<Objective> ls = new List<Objective>();
		DataSet ds = GTOHelper.GetDataSet(sObjSql);
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new Objective(dr));
		return ls;
	}
	[WebMethod(EnableSession = true)]
	public Objective NewObective(string id)
	{
		string name = Guid.NewGuid().ToString();
		GTOHelper.SQLExecute(string.Format("INSERT INTO OBJECTIVIES ([NAME],[idSeverity]) VALUES ('{0}',{1})", name, id));
		DataSet ds = GTOHelper.GetDataSet(string.Format(sObjSql + " WHERE NAME = '{0}'", (name)));
		foreach (DataRow dr in ds.Tables[0].Rows)
			return new Objective(dr);
		return new Objective();
	}
	[WebMethod(EnableSession = true)]
	public Objective GetObective(string id)
	{
		DataSet ds = GTOHelper.GetDataSet(sObjSql + " WHERE ID = " + id);
		foreach (DataRow dr in ds.Tables[0].Rows)
			return new Objective(dr);
		return new Objective();
	}
	[WebMethod(EnableSession = true)]
	public void ObectiveOffset(string id, string x, string y)
	{
		GTOHelper.SQLExecute(string.Format("UPDATE OBJECTIVIES SET X = {0}, Y = {1} WHERE ID = {2}", x, y, id));
	}
	[WebMethod(EnableSession = true)]
	public void ObectiveSize(string id, string x, string y)
	{
		GTOHelper.SQLExecute(string.Format("UPDATE OBJECTIVIES SET SIZEX = {0}, SIZEY = {1} WHERE ID = {2}", x, y, id));
	}
	[WebMethod(EnableSession = true)]
	public void ObectiveDelete(string id)
	{
		GTOHelper.SQLExecute(string.Format("DELETE FROM OBJECTIVIES WHERE ID = {0}", id));
	}
	[WebMethod(EnableSession = true, CacheDuration = 300)]
	public List<KeyVal> LoadSeverities()
	{
		List<KeyVal> ls = new List<KeyVal>();
		DataSet ds = GTOHelper.GetDataSet("select idRecord id, Descriptor val from [tt_res].[dbo].[FLDSEVER] order by Descriptor");
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new KeyVal(dr));
		return ls;
	}
	[WebMethod(EnableSession = true, CacheDuration = 300)]
	public List<KeyVal> LoadDispositions()
	{
		List<KeyVal> ls = new List<KeyVal>();
		DataSet ds = GTOHelper.GetDataSet("select idRecord id, Descriptor val from [tt_res].[dbo].[FLDDISPO] order by Descriptor");
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new KeyVal(dr));
		return ls;
	}
	[WebMethod(EnableSession = true, CacheDuration = 300)]
	public List<KeyVal> LoadPriorities()
	{
		List<KeyVal> ls = new List<KeyVal>();
		DataSet ds = GTOHelper.GetDataSet("SELECT IDRECORD ID, DESCRIPTOR VAL FROM [TT_RES].[DBO].[FLDPRIOR] ORDER BY DESCRIPTOR");
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new KeyVal(dr));
		return ls;
	}
	[WebMethod(EnableSession = true)]
	public List<SevSummary> LoadSummary2Severity(string id)
	{
		List<SevSummary> ls = new List<SevSummary>();
		DataSet ds = GTOHelper.GetDataSet(string.Format(@"
SELECT 
IDDISPOSIT
,(SELECT DI.DESCRIPTOR FROM [TT_RES].[DBO].[FLDDISPO] DI WHERE DI.IDRECORD = IDDISPOSIT) AS DISP
,COUNT(*) NUM
,SUM(EST) AS EST
FROM (SELECT 
 IDDISPOSIT
,(SELECT TOP 1 E.TIMESPENT FROM [TT_RES].[DBO].DEFECTEVTS E WHERE E.EVTDEFID = 2 AND E.PARENTID = D.IDRECORD ORDER BY E.ORDERNUM DESC) AS EST
FROM [TT_RES].[DBO].[DEFECTS] D 
WHERE D.IDSEVERITY = {0} AND D.STATUS = 1) T
GROUP BY IDDISPOSIT", id));
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new SevSummary(dr));
		return ls;
	}
	[WebMethod(EnableSession = true)]
	public List<TTShort> GetFilteredTTShort(string idsever, string iddisp)
	{
		List<TTShort> ls = new List<TTShort>();
		DataSet ds = GTOHelper.GetDataSet(string.Format(@"
SELECT TTID, SUMMARY
,(SELECT P.PERSON_ID FROM PERSONS P WHERE P.WORK_EMAIL = EML) USRID
,(SELECT P.PERSON_NAME FROM PERSONS P WHERE P.WORK_EMAIL = EML) USRNAME
FROM (
SELECT 
 D.DEFECTNUM TTID
,D.SUMMARY SUMMARY
,RTRIM(D.USR) EML
FROM [TT_RES].[DBO].[DEFECTS] D
WHERE
D.IDDISPOSIT = {0} AND D.IDSEVERITY = {1}) T", iddisp, idsever));
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new TTShort(dr));
		return ls;
	}
	[WebMethod(EnableSession = true)]
	public string EmailSeverity(string idsever, string userid)
	{
		MailMessage mail = new MailMessage();
		string eml = GTOHelper.GetUserEmail(Convert.ToInt32(userid));
		mail.To.Add(new MailAddress("evin@resnet.com"));
		mail.CC.Add(new MailAddress("mike@resnet.com"));
		mail.CC.Add(new MailAddress("dwk@resnet.com"));
		mail.CC.Add(new MailAddress("minsk@resnet.com"));
		//		mail.To.Add(new MailAddress("svan@resnet.com"));
		DataSet ds = GTOHelper.GetDataSet("SELECT S.DESCRIPTOR FROM TT_RES.DBO.FLDSEVER S WHERE IDRECORD = " + idsever);
		string sev = ds.Tables[0].Rows[0][0].ToString();
		mail.Subject = "Severity (" + sev + ") has been completed.";
		mail.From = new MailAddress(GTOHelper.GetUserEmail(Convert.ToInt32(userid)));
		mail.Body = "Severity: " + sev + @" has been completed.
Please confirm we should perform any final checks or / and should continue tracking of these tasks.
Thanx, " + GTOHelper.GetUserNameByEmail(eml);
		SmtpClient smtp = new SmtpClient();

		smtp.Host = "smtp.resnet.com";
		smtp.Port = 109;
		smtp.SendCompleted += (s, e) =>
		{
			smtp.Dispose();
			mail.Dispose();
		};
		try
		{
			smtp.Send(mail);
			GTOHelper.SQLExecute("UPDATE OBJECTIVIES SET EMAILED = 1 WHERE IDSEVERITY = " + idsever);
			return "";
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}
	[WebMethod(EnableSession = true)]
	public void SkipEmail2Severity(string idsever)
	{
		GTOHelper.SQLExecute("UPDATE OBJECTIVIES SET EMAILED = 0 WHERE IDSEVERITY = " + idsever);
	}
	[WebMethod(EnableSession = true)]
	public void CloseSeverity(string idsever)
	{
		GTOHelper.SQLExecute("UPDATE OBJECTIVIES SET EMAILED = 1 WHERE IDSEVERITY = " + idsever);
	}
	[WebMethod(EnableSession = true)]
	public void UpdateTask(string TTID, DateTime date, string est, string dispo, string text, string userid, string prio)
	{
		string email = GTOHelper.GetUserEmail(Convert.ToInt32(userid));
		GTOHelper.UpdateTask(TTID, date, dispo, text, prio);
		EstimateTask(TTID, email, est);
		AddTaskLogEntry(TTID, email, "TT" + TTID.ToString() + ": task changed by user from web interface.");
	}
	[WebMethod(EnableSession = true)]
	public string GetScheduledTasks(string userid)
	{
		string email = GTOHelper.GetUserEmail(Convert.ToInt32(userid));
		DataSet ds = GTOHelper.GetDataSet(string.Format(@"
				SELECT D.DEFECTNUM FROM 
					[tt_res].[dbo].[DEFECTS] D 
				WHERE
				CAST(D.IOrderDate AS DATE) = CAST(GETDATE() AS DATE)
				AND D.sModifier = '{0}'
				ORDER BY D.DEFECTNUM
		", email));
		string res = "";
		foreach (DataRow dr in ds.Tables[0].Rows)
		{
			if (!string.IsNullOrEmpty(res))
				res += ", ";
			res += "TT" + Convert.ToString(dr[0]);
		}
		return res;
	}
	[WebMethod(EnableSession = true)]
	public string GetUpdatedTasks(string userid)
	{
		string email = GTOHelper.GetUserEmail(Convert.ToInt32(userid));
		DataSet ds = GTOHelper.GetDataSet(string.Format(@"
				SELECT D.DEFECTNUM FROM 
				 [tt_res].[dbo].[DEFLOG] L
				,[tt_res].[dbo].[USERS] U
				,[tt_res].[dbo].[DEFECTS] D 
				WHERE
				L.IDUSER = U.IDRECORD
				AND D.IDRECORD = L.PARENTID
				AND CAST(L.DATELOG AS DATE) = CAST(GETDATE() AS DATE)
				AND U.EMAILADDR = '{0}'
				GROUP BY D.DEFECTNUM
				ORDER BY D.DEFECTNUM
		", email));
		string res = "";
		foreach (DataRow dr in ds.Tables[0].Rows)
		{
			if (!string.IsNullOrEmpty(res))
				res += ", ";
			res += "TT" + Convert.ToString(dr[0]);
		}
		return res;
	}
	[WebMethod(EnableSession = true)]
	public string ScheduleVacation(string useremail, DateTime date)
	{
		return GTOHelper.ScheduleVacation(useremail, date);
	}
	[WebMethod(EnableSession = true)]
	public List<TWorkDay> GetWorkDays(string year)
	{
		List<TWorkDay> ls = new List<TWorkDay>();
		DataSet ds = GTOHelper.GetDataSet(string.Format(@"
				SELECT
				R.REPORT_DATE DATE,
				(SELECT P.WORK_EMAIL FROM PERSONS P WHERE P.PERSON_ID = R.PERSON_ID) EMAIL
				FROM 
				REPORTS R 
				WHERE
				YEAR(R.REPORT_DATE) = {0}
				ORDER BY EMAIL, DATE", year));
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new TWorkDay(dr));
		return ls;
	}
	[WebMethod(EnableSession = true, CacheDuration = 30)]
	public List<TWorkDay> GetWorkDaysFromDate(string date)
	{
		DateTime dt1 = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		DateTime dt2 = dt1.AddYears(1);

		List<TWorkDay> ls = new List<TWorkDay>();
		DataSet ds = GTOHelper.GetDataSet(string.Format(@"
				SELECT
				R.REPORT_DATE DATE,
				(SELECT P.WORK_EMAIL FROM PERSONS P WHERE P.PERSON_ID = R.PERSON_ID) EMAIL
				FROM 
				REPORTS R 
				WHERE
				CAST(R.REPORT_DATE as date) >= '{0}' and CAST(R.REPORT_DATE as date) <= '{1}' 
				ORDER BY EMAIL, DATE", dt1.ToString(SQLDateFormat), dt2.ToString(SQLDateFormat)));
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new TWorkDay(dr));
		return ls;
	}
	[WebMethod(EnableSession = true)]
	public List<PlanData> GetOutDays(string datefrom, string dateto)
	{
		DateTime dt1 = DateTime.ParseExact(datefrom, defDateFormat, CultureInfo.InvariantCulture);
		DateTime dt2 = DateTime.ParseExact(dateto, defDateFormat, CultureInfo.InvariantCulture);

		List<PlanData> ls = new List<PlanData>();
		DataSet ds = GTOHelper.GetOutDays(dt1, dt2);
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new PlanData(dr));
		return ls;
	}
	[WebMethod]
	public void SendVacationEmail()
	{
		DataSet ds = GTOHelper.GetDataSet(@"
							SELECT D.Summary
							FROM [tt_res].[dbo].[DEFECTS] d
							WHERE
							D.dateEnter = CONVERT(DATE, GETDATE())
							AND D.idCompon = 96
				");
		string str = "";
		foreach (DataRow rowCur in ds.Tables[0].Rows)
		{
			str += rowCur[0].ToString() + Environment.NewLine;
		}
		if (!string.IsNullOrEmpty(str))
		{
			MailMessage mail = new MailMessage();
			mail.To.Add(new MailAddress("evin@resnet.com"));
			mail.CC.Add(new MailAddress("mike@resnet.com"));
			mail.CC.Add(new MailAddress("dwk@resnet.com"));
			mail.CC.Add(new MailAddress("minsk@resnet.com"));
			mail.Subject = "MPS Vacation notification.";
			mail.From = new MailAddress("bst_tester@resnet.com");
			mail.Body = "Following vacation tasks are entered today: " + Environment.NewLine + Environment.NewLine + str + Environment.NewLine + "Thanx.";
			SmtpClient smtp = new SmtpClient();
			smtp.Host = "smtp.resnet.com";
			smtp.Port = 109;
			smtp.SendCompleted += (s, e) =>
			{
				smtp.Dispose();
				mail.Dispose();
			};
			smtp.Send(mail);
		}
	}
	[WebMethod(EnableSession = true)]
	public void AddToday(string userid, bool copy)
	{
		string sql = string.Format("SELECT count(*) FROM REPORTS where person_id = {0} and CAST(REPORT_DATE as DATE) = CAST(GETDATE() AS DATE)", userid);
		int inum = Convert.ToInt32(GTOHelper.GetDataSet(sql).Tables[0].Rows[0][0]);
		if (inum > 0)
			return;

		if (!copy)
			sql = string.Format("INSERT INTO REPORTS (PERSON_ID) values ({0})", userid);
		else
			sql = string.Format(@"INSERT INTO REPORTS 
										(PERSON_ID, REPORT_DONE)
										SELECT R1.PERSON_ID, R1.REPORT_DONE FROM REPORTS R1 WHERE R1.PERSON_ID = {0} 
										AND R1.REPORT_DATE = (SELECT MAX(R2.REPORT_DATE) FROM REPORTS R2 WHERE R2.PERSON_ID = {0})", userid);

		GTOHelper.SQLExecute(sql);
	}
	[WebMethod(EnableSession = true)]
	public void NewRec(string userid, bool copy, string date)
	{
		var dateTime = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		string sqlFormattedDate = dateTime.ToString(SQLDateFormat);

		string sql = string.Format("SELECT count(*) FROM REPORTS where person_id = {0} and CAST(REPORT_DATE as DATE) = CAST('{1}' AS DATE)", userid, sqlFormattedDate);
		int inum = Convert.ToInt32(GTOHelper.GetDataSet(sql).Tables[0].Rows[0][0]);
		if (inum > 0)
			return;

		if (!copy)
			sql = string.Format("INSERT INTO REPORTS (PERSON_ID, REPORT_DATE) values ({0}, CAST('{1}' AS DATE))", userid, sqlFormattedDate);
		else
			sql = string.Format(@"INSERT INTO REPORTS 
										(PERSON_ID, REPORT_DONE, REPORT_DATE)
										SELECT R1.PERSON_ID, R1.REPORT_DONE, CAST('{1}' AS DATE) DT FROM REPORTS R1 WHERE R1.PERSON_ID = {0} 
										AND R1.REPORT_DATE = (SELECT MAX(R2.REPORT_DATE) FROM REPORTS R2 WHERE R2.PERSON_ID = {0})", userid, sqlFormattedDate);

		GTOHelper.SQLExecute(sql);
	}
	[WebMethod(EnableSession = true)]
	public void DelRec(string userid, string date)
	{
		var dateTime = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		string sqlFormattedDate = dateTime.ToString(SQLDateFormat);
		string sql = string.Format("DELETE FROM REPORTS where person_id = {0} and CAST(REPORT_DATE as DATE) = CAST('{1}' AS DATE)", userid, sqlFormattedDate);
		GTOHelper.SQLExecute(sql);
	}
	[WebMethod(EnableSession = true, CacheDuration = 30)]
	public List<TTUser> GetUsers()
	{
		List<TTUser> ls = new List<TTUser>();
		DataSet ds = GTOHelper.GetDataSet(@"
			SELECT 
			  P.PERSON_ID
			 ,P.WORK_EMAIL 
			 ,P.PERSON_NAME
			 ,(SELECT TOP 1 D.DEFECTNUM FROM TT_RES.DBO.DEFECTS D WHERE CAST(D.DATEENTER AS DATE) = CAST(GETDATE() AS DATE) AND RTRIM(D.USR) = P.WORK_EMAIL AND D.IDCOMPON = 96) VACATION_TTID
			 ,(SELECT COUNT(*) FROM TT_RES.DBO.DEFECTS D WHERE CAST(D.DATEENTER AS DATE) = CAST(GETDATE() AS DATE) AND RTRIM(D.USR) = P.WORK_EMAIL AND D.IDCOMPON = 96) VACATIONS_COUNT
			 ,(CASE CAST(R.REPORT_DATE AS DATE) WHEN CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS USER_ONLINE
			FROM 
			 PERSONS P LEFT OUTER JOIN 
			(SELECT * FROM REPORTS WHERE CAST(REPORT_DATE AS DATE) = CAST(GETDATE() AS DATE)) R 
				ON R.PERSON_ID = P.PERSON_ID
			WHERE P.IN_WORK = 1
			ORDER BY P.PERSON_NAME
		");
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new TTUser(dr));
		return ls;
	}
	[WebMethod(EnableSession = true, CacheDuration = 10)]
	public List<LogEntry> GetEventsLog()
	{
		List<LogEntry> ls = new List<LogEntry>();
		DataSet ds = GTOHelper.GetDataSet(@"
			SELECT TOP 15
			 D.DEFECTNUM
			 ,L.NOTES
			 ,P.PERSON_ID
			FROM 
			 TT_RES.DBO.DEFLOG L 
			 ,TT_RES.DBO.USERS U
			 ,PERSONS P
			 ,TT_RES.DBO.DEFECTS D
			WHERE 
			 D.IDRECORD = L.PARENTID
			 AND U.IDRECORD = L.IDUSER
			 AND P.WORK_EMAIL = U.EMAILADDR
			 AND L.DATELOG > DATEADD(DAY, -1, GETDATE())
			ORDER BY 
			 L.DATELOG DESC
		");
		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new LogEntry(dr));
		return ls;
	}
	[WebMethod(EnableSession = true, CacheDuration = 10)]
	public List<DailyRecord> GetDailyReport(string date)
	{
		var dateTime = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		string sqlFormattedDate = dateTime.ToString(SQLDateFormat);

		List<DailyRecord> ls = new List<DailyRecord>();
		DataSet ds = GTOHelper.GetDataSet(string.Format(@"
				SELECT 
				*
				,(SELECT D.DEFECTNUM FROM TT_RES.DBO.DEFECTS D WHERE CAST(D.DATEENTER AS DATE) = CAST('{0}' AS DATE) AND D.IDCOMPON = 96 AND D.USR = T.WORK_EMAIL) VACATION
				FROM 
				(
				SELECT 
				 PRS.WORK_EMAIL
				,REPS.REPORT_DONE
				,PRS.PERSON_NAME
				FROM
				(SELECT * FROM PERSONS P WHERE P.IN_WORK = 1) PRS
				LEFT OUTER JOIN (SELECT * FROM REPORTS R WHERE R.REPORT_DATE = CAST('{0}' AS DATE)) REPS
				ON PRS.PERSON_ID = REPS.PERSON_ID
				) T
				ORDER BY T.PERSON_NAME", sqlFormattedDate));

		foreach (DataRow dr in ds.Tables[0].Rows)
			ls.Add(new DailyRecord(dr));
		return ls;
	}

	//================================================================================================
	//================================================================================================
	//================================================================================================
	[WebMethod(EnableSession = true)]
	public List<DefectBase> getplanned(string userid)
	{
		if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
			return null;

		DefectBase d = new DefectBase();
		return d.EnumPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> getunplanned(string userid)
	{
		if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
			return null;

		DefectBase d = new DefectBase();
		return d.EnumUnPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid));
	}
	[WebMethod(EnableSession = true)]
	public Defect gettask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		Defect d = new Defect(Convert.ToInt32(ttid));
		if (!d.IsLoaded())
			return null;
		return d;
	}
	[WebMethod(EnableSession = true)]
	public string settask(Defect d)
	{
		Defect dstore = new Defect(d.ID);
		dstore.FromAnotherObject(d);
		if (dstore.IsModified())
		{
			dstore.Store();
		}
		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public string setdispos(List<DefectDispo> dispos)
	{
		foreach (DefectDispo d in dispos)
		{
			DefectDispo dstore = new DefectDispo(d.ID);
			dstore.FromAnotherObject(d);
			if (dstore.IsModified())
			{
				dstore.Store();
			}
		}
		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public List<DefectHistory> gettaskhistory(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectHistory.GetHisotoryByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectAttach> getattachsbytask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectAttach.GetAttachsByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectEvent> gettaskevents(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectEvent.GetEventsByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectType> gettasktypes()
	{
		return DefectType.Enum();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectProduct> gettaskproducts()
	{
		return DefectProduct.Enum();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectDispo> gettaskdispos()
	{
		return DefectDispo.Enum();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectPriority> gettaskpriorities()
	{
		return DefectPriority.Enum();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectComp> gettaskcomps()
	{
		return DefectComp.Enum();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectSeverity> gettasksevers()
	{
		return DefectSeverity.Enum();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectUser> gettaskusers()
	{
		return DefectUser.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void newfileupload(string ttid, string filename, string data)
	{
		DefectAttach.AddAttachByTask(Convert.ToInt32(ttid), filename, data.Remove(0, data.IndexOf("base64,") + 7));
	}
	[WebMethod(EnableSession = true)]
	public LockInfo locktask(string ttid, string lockid)
	{
		if (!CurrentContext.Valid)
		{
			return null;
		}
		return Defect.Locktask(ttid, lockid);
	}
	[WebMethod(EnableSession = true)]
	public void unlocktask(string ttid, string lockid)
	{
		Defect.UnLocktask(ttid, lockid);
	}
	[WebMethod(EnableSession = true)]
	public List<Defect> gettasks()
	{
		//Defect filter
		return Defect.Enum();
	}
	[WebMethod(EnableSession = true)]
	public TRRec gettrrec(string date)
	{
		DateTime d = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		return r;
	}
	[WebMethod(EnableSession = true)]
	public void settrrec(TRRec rec)
	{
		TRRec store = new TRRec(rec.ID);
		store.FromAnotherObject(rec);
		if (store.IsModified())
		{
			store.Store();
		}
	}
	[WebMethod(EnableSession = true)]
	public void deltrrec(int id)
	{
		TRRec.DelRec(id);
	}
	[WebMethod(EnableSession = true)]
	public void todayrrec(string lastday)
	{
		DateTime d = DateTime.Today;
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		if (r == null)
		{
			TRRec.NewRec(d, CurrentContext.User.ID, lastday == "True");
		}
	}
	[WebMethod(EnableSession = true)]
	public void addrec(string date, string lastday)
	{
		DateTime d = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		if (r == null)
		{
			TRRec.NewRec(d, CurrentContext.User.ID, lastday == "True");
		}
	}
	[WebMethod(EnableSession = true)]
	public MPSUser getcurrentuser()
	{
		return CurrentContext.User;
	}
	[WebMethod(EnableSession = true)]
	public DefectBase settaskdispo(string ttid, string disp)
	{
		if (Defect.Locked(ttid))
			return null;
		Defect d = new Defect(Convert.ToInt32(ttid));
		d.DISPO = disp;
		d.Store();
		return new DefectBase(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<MPSUser> getMPSusers()
	{
		return MPSUser.EnumAllUsers();
	}
	[WebMethod(EnableSession = true)]
	public string setusers(List<MPSUser> users)
	{
		foreach (MPSUser u in users)
		{
			MPSUser ustore = new MPSUser(u.ID);
			ustore.FromAnotherObject(u);
			if (ustore.IsModified())
			{
				ustore.Store();
			}
		}
		return "OK";
	}
	public class TTBackOrder
	{
		public int ttid { get; set; }
		public int backorder { get; set; }
		public bool moved { get; set; }
	}
	[WebMethod(EnableSession = true)]
	public void setschedule(List<TTBackOrder> ttids, List<string> unschedule)
	{
		if (!CurrentContext.Valid)
		{
			return;
		}

		foreach (var ttid in unschedule)
		{
			Defect d = new Defect(Convert.ToInt32(ttid));
			d.ORDER = -1;
			d.Store();
		}

		foreach (var ttid in ttids)
		{
			DefectBase d;
			if (ttid.moved)
			{
				d = new Defect(ttid.ttid); //will add history record about moving
			}
			else
			{
				d = new DefectBase(ttid.ttid);
			}
			d.BACKORDER = Convert.ToInt32(ttid.backorder);
			d.Store();
		}
	}
}