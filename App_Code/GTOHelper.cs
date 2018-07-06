using System;
using System.Windows;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Management;

/// <summary>
/// Summary description for GTOHelper
/// </summary>
namespace GTOHELPER
{
	public class GTOHelper : System.Web.UI.Page
	{
		protected static string LoginPageName
		{
			get
			{
				return "login.aspx";
			}
		}
		private System.Windows.Forms.RichTextBox m_ctrl1;
		protected System.Windows.Forms.RichTextBox GetTRTCtrl
		{
			get
			{
				if (m_ctrl1 == null)
					m_ctrl1 = new System.Windows.Forms.RichTextBox();
				return m_ctrl1;
			}
		}
		
		//working with users
		public static string UserCookie
		{
			get {return "userid";}
		}
		private int m_ISERID = -1;
		public int UserID
		{
			get
			{
				return -1;
				if (LoginPageName.ToUpper() == CurrentPageName.ToUpper())
					return -1;

				if (m_ISERID != -1)
					return m_ISERID;

				if (Request.Params["susername"] != null && Request.Params["suserpass"] != null)
				{
					m_ISERID = FindUser(Request.Params["susername"].ToString(), Request.Params["suserpass"].ToString());
					Session["userid"] = m_ISERID;
					if (m_ISERID == -1)
					{
						Response.Redirect(LoginPageName + "?ReturnUrl=" + CurrentPageName);
						return -1;
					}
					return m_ISERID;
				}		
				HttpCookie cookie = Request.Cookies.Get(UserCookie);
				if (cookie == null || string.IsNullOrEmpty(cookie.Value))
				{
					Response.Redirect(LoginPageName + "?ReturnUrl=" + CurrentPageName, false);
					Context.ApplicationInstance.CompleteRequest();
					return -1;
				}

				m_ISERID = Convert.ToInt32(cookie.Value);
				Session["userid"] = m_ISERID;
				return m_ISERID;
			}
		}
		public int FindUser(string strUserName, string strPassword)
		{
			DataSet ds = GetDataSet("SELECT PERSON_ID FROM PERSONS WHERE PERSON_LOGIN = '" + strUserName + "' AND USER_PASSWORD = '" + strPassword + "'");
			foreach (DataRow rowCur in ds.Tables[0].Rows)
			{
				return (Int32)rowCur[0];
			}
			return -1;
		}

		public string GetUserName(long nID)
		{
			DataSet dtst = GetDataSet("SELECT PERSON_NAME FROM PERSONS WHERE PERSON_ID = " + nID.ToString());
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["PERSON_NAME"].ToString();
			}
			return "";
		}
		public static string GetTTText(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"SELECT R.DESCRPTN 
				FROM 
				[TT_RES].[DBO].[REPORTBY] R
				WHERE R.IDDEFREC = (SELECT IDRECORD FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["DESCRPTN"].ToString();
			}
			return "";
		}
		public static string GetTTTitle(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"(SELECT SUMMARY FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["SUMMARY"].ToString();
			}
			return "";
		}
		public static DateTime GetTTDate(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"(SELECT dateEnter FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return Convert.ToDateTime(rowCur["dateEnter"]);
			}
			return DateTime.Now;
		}
		public static int GetTTEst(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"(SELECT estim FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return Convert.IsDBNull(rowCur["estim"]) ? 0 : Convert.ToInt32(rowCur["estim"]);
			}
			return 0;
		}
		public static string GetTTDispo(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"SELECT DI.DESCRIPTOR DISP FROM [TT_RES].[DBO].[FLDDISPO] DI WHERE DI.IDRECORD IN
					(SELECT D.IDDISPOSIT FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["DISP"].ToString();
			}
			return "";
		}
		public static string GetTTPrio(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"SELECT PR.DESCRIPTOR DISP FROM [TT_RES].[DBO].[FLDPRIOR] PR WHERE PR.IDRECORD IN
					(SELECT D.IDPRIORITY FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["DISP"].ToString();
			}
			return "";
		}		
		public static string GetTTUsr(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"SELECT P.PERSON_NAME NAME FROM PERSONS P WHERE P.WORK_EMAIL = 
(select RTRIM(D.USR) FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["NAME"].ToString();
			}
			return "";
		}
		public static string GetTTCreateUsr(string TTID)
		{
			DataSet dtst = GetDataSet(string.Format(@"SELECT P.PERSON_NAME NAME FROM PERSONS P WHERE P.WORK_EMAIL = 
(SELECT U.EMAILADDR FROM [TT_RES].[DBO].[USERS] U WHERE U.IDRECORD =
					(SELECT TOP 1 D.IDCREATEBY FROM [TT_RES].[DBO].[DEFECTS] D
					WHERE D.DEFECTNUM = {0}))", TTID));
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["NAME"].ToString();
			}
			return "";
		}
		public static string GetUserNameByEmail(string email)
		{
			DataSet dtst = GetDataSet("SELECT PERSON_NAME FROM PERSONS WHERE WORK_EMAIL = '" + email + "'");
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["PERSON_NAME"].ToString();
			}
			return "";
		}
		public static string GetUserEmail(long nID)
		{
			DataSet dtst = GetDataSet("SELECT WORK_EMAIL FROM PERSONS WHERE PERSON_ID = " + nID.ToString());
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return rowCur["WORK_EMAIL"].ToString();
			}
			return "";
		}
		public int GetUserID(string strLogin)
		{
			DataSet dtst = GetDataSet("SELECT PERSON_ID FROM PERSONS WHERE PERSON_LOGIN = '" + strLogin + "'");
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return (Int32)rowCur["PERSON_ID"];
			}
			return -1;
		}
		public static string GetUserLevel(int ID)
		{
			return
			GetDataSet("select level_name from levels where level_id in (select level_id from persons where person_id = " + ID.ToString() + ")").Tables[0].Rows[0][0].ToString();
		}
		public int GetUserIDByDispName(string strName)
		{
			DataSet dtst = GetDataSet("SELECT PERSON_ID FROM PERSONS WHERE PERSON_NAME = '" + strName + "'");
			foreach (DataRow rowCur in dtst.Tables[0].Rows)
			{
				return (Int32)rowCur["PERSON_ID"];
			}
			return -1;
		}
		public bool IsMyBirthday()
		{
			DataSet ds = GetDataSet("SELECT PERSON_BIRTHDAY FROM PERSONS WHERE PERSON_ID = " + UserID.ToString());
			foreach (DataRow rowCur in ds.Tables[0].Rows)
			{
				DateTime dtb = (DateTime)rowCur[0];
				DateTime dtt = DateTime.Today;
				return dtb.Month == dtt.Month && dtb.Day == dtt.Day;
			}
			return false;
		}
		public string Birthdays(DateTime dt)
		{
			string strKey = "Birthdays_key_for_" + dt.ToShortDateString();
			string strRes = "";
			if (Application[strKey] != null)
			{
				strRes = Application[strKey].ToString();
			}
			else
			{
				DataSet ds = GetDataSet("SELECT PERSON_NAME FROM PERSONS WHERE IN_WORK = 1 AND DAY(PERSON_BIRTHDAY) = " + dt.Day + " and MONTH(PERSON_BIRTHDAY) = " + dt.Month);
				foreach (DataRow rowCur in ds.Tables[0].Rows)
				{
					if (strRes == "")
						strRes += "<center>" + "Birthdays:" + "<center/>" + "<center>" + (string)rowCur[0] + "</center>";
					else
						strRes += "<br/>" + "<center>" + (string)rowCur[0] + "</center>";
				}

			}
			if (Application[strKey] == null)
			{
				Application.Lock();
				Application[strKey] = strRes;
				Application.UnLock();
			}
			return strRes;
		}

		public static string CurrentPageName
		{
			get
			{
				System.IO.FileInfo oInfo = new System.IO.FileInfo(HttpContext.Current.Request.Url.AbsolutePath);
				return oInfo.Name;
			}
		}

		//visualization
		protected void UpdateControls(System.Web.UI.ControlCollection WorkControls)
		{
			int total = WorkControls.Count;
			for (int i = 0; i < total; i++)
			{
				System.Web.UI.Control c = WorkControls[i] as System.Web.UI.Control;
				System.Web.UI.WebControls.Button b = WorkControls[i] as System.Web.UI.WebControls.Button;
				if (b != null && string.IsNullOrEmpty(b.CssClass))
				{
					b.CssClass = "button";
				}
				if (c != null)
					UpdateControls(c.Controls);
			}
		}
		protected void MPage_Load(object sender, EventArgs e)
		{
			int lUserID  = UserID; // initializes security
/*			var link = new HtmlLink();
			link.Attributes.Add("type", "text/css");
			link.Attributes.Add("rel", "stylesheet");
			link.Href = ResolveClientUrl("TR.css");
			Header.Controls.Add(link);*/
			UpdateControls(Page.Controls);
		}

		public GTOHelper()
		{
			PreRenderComplete  += new EventHandler(MPage_Load);
		}
		public bool IsAdmin
		{
			get
				{
					string strAdmin = "";
					if (Session["IsAdmin"] != null)
						strAdmin = Session["IsAdmin"].ToString();
					if (string.IsNullOrEmpty(strAdmin))
					{
						DataSet dtst = GetDataSet("SELECT IS_ADMIN FROM PERSONS WHERE PERSON_ID = " + UserID.ToString());
						foreach (DataRow rowCur in dtst.Tables[0].Rows)
						{
							bool b = (bool)rowCur["IS_ADMIN"];
							strAdmin = b.ToString();
							break;
						}
						if (string.IsNullOrEmpty(strAdmin))
							strAdmin = false.ToString();
					}
					Session["IsAdmin"] = strAdmin;
					return Convert.ToBoolean(strAdmin);
				}
		}
		public static string ConnString
		{
			get
			{
				return "Provider=SQLOLEDB;Data Source=192.168.0.1;Initial Catalog=TASKS;Persist Security Info=True;User ID=sa;Password=prosuite";
				//return "Provider=SQLOLfaEDB;Data Source=192.168.0.1;Initial Catalog=TASKS;Persist Security Info=True;User ID=sa;Password=prosuite";
			}
		}
		public static OleDbConnection NewConnection
		{
			get { return new OleDbConnection(ConnString); }
		}
		public static DataSet GetDataSet(string strSQL)
		{
			OleDbConnection conn = NewConnection;
			conn.Open();
			DataSet ds = new DataSet();
			OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, conn);
			adapter.Fill(ds);
			conn.Close();
			return ds;
		}
		public static DataSet GetTableDataSet(string strTableAndCondition)
		{
			OleDbConnection conn = NewConnection;
			conn.Open();
			DataSet ds = new DataSet();
			OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM " + strTableAndCondition, conn);
			adapter.Fill(ds);
			conn.Close();
			return ds;
		}
		public static void SQLExecute(string strSQL)
		{
			using (OleDbConnection conn = NewConnection)
			{
				conn.Open();
				OleDbCommand cmd = new OleDbCommand(strSQL, conn);
				cmd.ExecuteNonQuery();
			}
		}
		public static string GetMAC(string machinename)
		{
			string res = "";
			try
			{
				string scope = string.Format("\\\\{0}\\root\\CIMV2", machinename);
				ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, "SELECT * FROM Win32_NetworkAdapterConfiguration");
				foreach (ManagementObject queryObj in searcher.Get())
				{
					object o = queryObj["MACAddress"];
					if (o == null)
					{
						continue;
					}
					if (string.IsNullOrEmpty(res))
						res = o.ToString().Replace(":", "");
					else
						res += " " + o.ToString().Replace(":", "");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			return res;
		}
		public string GetMSAccessDate(DateTime dt)
		{
			CultureInfo cultures = CultureInfo.InvariantCulture;
			return "'" + dt.ToString("MM/dd/yyyy", cultures) + "'";
		}
		public string TheLatestVersion
		{
			get
			{
				string strLastVer = (Application["TheLatestVersion"] == null) ? "" : Application["TheLatestVersion"] as string;
				string strLastVerT = (Application["TheLatestVersionT"] == null) ? "" : Application["TheLatestVersionT"] as string;
				if (string.IsNullOrEmpty(strLastVerT))
					strLastVerT = DateTime.Now.AddDays(-1).ToString();

				TimeSpan diff = DateTime.Now - Convert.ToDateTime(strLastVerT);

				if (diff.TotalMinutes > 30)
				{
					DirectoryInfo di = new DirectoryInfo("\\\\192.168.0.1\\FiP Installations\\");
					DirectoryInfo[] dirs = di.GetDirectories("FIELDPRO V7J*");
					string strMax = "";
					foreach (DirectoryInfo d1 in dirs)
					{
						if (d1.Name.CompareTo(strMax) > 0)
							strMax = d1.Name;
					}
					Application.Lock();
					Application["TheLatestVersion"] = strMax;
					Application["TheLatestVersionT"] = DateTime.Now.ToString();
					Application.UnLock();
				}
				return Application["TheLatestVersion"] as string;
			}
		}
		public string TheLatestDemoVersion
		{
			get
			{
				string strLastVer = (Application["TheLatestDMVersion"] == null) ? "" : Application["TheLatestDMVersion"] as string;
				string strLastVerT = (Application["TheLatestDMVersionT"] == null) ? "" : Application["TheLatestDMVersionT"] as string;
				if (string.IsNullOrEmpty(strLastVerT))
					strLastVerT = DateTime.Now.AddDays(-1).ToString();

				TimeSpan diff = DateTime.Now - Convert.ToDateTime(strLastVerT);

				if (diff.TotalMinutes > 30)
				{
					DirectoryInfo di = new DirectoryInfo("\\\\\\192.168.0.1\\bst\\DEMO\\");
					DirectoryInfo[] dirs = di.GetDirectories("DEMO_Ver-*");
					string strMax = "";
					foreach (DirectoryInfo d1 in dirs)
					{
						if (d1.Name.ToUpper().IndexOf("EXCEPTION") > -1)
							continue;
						if (d1.Name.CompareTo(strMax) > 0)
							strMax = d1.Name;
					}
					Application.Lock();
					Application["TheLatestDMVersion"] = strMax;
					Application["TheLatestDMVersionT"] = DateTime.Now.ToString();
					Application.UnLock();
				}
				return Application["TheLatestDMVersion"] as string;
			}
		}

		public string ReplaceTT(string strTT)
		{
			return Regex.Replace(strTT, "TT\\d+", TTEvaluator);
		}
		public string RTFConvert(string str)
		{
			if (str.StartsWith("{\\rtf1\\"))
			{
				GetTRTCtrl.Rtf = str;
				return GetTRTCtrl.Text;
			}
			else
				return str;
		}
		public static string ReplaceEx(string original, string pattern, string replacement)
		{
			int count, position0, position1;
			count = position0 = position1 = 0;
			string upperString = original.ToUpper();
			string upperPattern = pattern.ToUpper();
			int inc = (original.Length / pattern.Length) *
						 (replacement.Length - pattern.Length);
			char[] chars = new char[original.Length + Math.Max(0, inc)];
			while ((position1 = upperString.IndexOf(upperPattern,
														 position0)) != -1)
			{
				for (int i = position0; i < position1; ++i)
					chars[count++] = original[i];
				for (int i = 0; i < replacement.Length; ++i)
					chars[count++] = replacement[i];
				position0 = position1 + pattern.Length;
			}
			if (position0 == 0) return original;
			for (int i = position0; i < original.Length; ++i)
				chars[count++] = original[i];
			return new string(chars, 0, count);
		}
		private static string TTEvaluator(Match match)
		{
			return "<a href='" + GetTTLink(match.Groups[0].Value) + "'>" + match.Groups[0].Value + "</a>";
		}
		public static string GetTTLink(string TTID)
		{
			return "http://mps.resnet.com:8080/scripts/ttcgi.exe?command=hyperlink&project=Res&uname=br&table=dfct&recordID=" +
				TTNum2ID(Convert.ToInt32(TTID.Replace("TT", "")));
		}
		public static string ScheduleVacation(string useremail, DateTime date)
		{
			DataSet ds  = GetDataSet(string.Format(@"
					SELECT top 1 DefectNum 
					FROM 
					[TT_RES].[DBO].DEFECTS D 
					WHERE 
					D.DATEENTER = '{0}-12-31'
					AND IDCOMPON = 96
					AND RTRIM(D.USR) = '{1}'
					ORDER BY SUMMARY", date.Year, useremail));
			if (ds.Tables[0].Rows.Count < 1)
				return "";
			string ttid = Convert.ToString(ds.Tables[0].Rows[0][0]);
			SQLExecute(string.Format("UPDATE [TT_RES].[DBO].DEFECTS SET DATEENTER = '{0}-{1}-{2}' where defectnum = {3}", date.Year, date.Month, date.Day, ttid));
			return ttid;
		}
		public static void UpdateTask(string TTID, DateTime date, string dispo, string text, string prio)
		{
			string query = string.Format(@"
				update 
				 [tt_res].[dbo].DEFECTS 
				set 
				  dateEnter = '{0}-{1}-{2}'
				 ,idDisposit = (select D.idRecord from tt_res.dbo.FLDDISPO D where d.Descriptor = '{4}')
				 ,idPriority = (select P.idRecord from tt_res.dbo.FLDPRIOR P where P.Descriptor = '{5}')
				where 
				 DefectNum = {3}
			", 
			date.Year, date.Month, date.Day, TTID, dispo, prio);
			SQLExecute(query);

			using (OleDbConnection conn = NewConnection)
			{
				conn.Open();
				query = string.Format(@"UPDATE [TT_RES].[DBO].[REPORTBY] set DESCRPTN = ?
												WHERE IDDEFREC = (SELECT IDRECORD FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.DEFECTNUM = {0})", TTID);
				OleDbCommand cmd = new OleDbCommand(query, conn);
				cmd.Parameters.AddWithValue("DESCRPTN", text);
				cmd.ExecuteNonQuery();
			}
		}
		public static string TTNum2ID(int iNum)
		{
			Dictionary<int, int> mapIDs = HttpContext.Current.Application["MAPTTIDS"] as Dictionary<int, int>;
			if (mapIDs.ContainsKey(iNum))
				return mapIDs[iNum].ToString();

			OleDbConnection conn = new OleDbConnection("Provider=SQLOLEDB;Data Source=192.168.0.1;Initial Catalog=tt_res;Persist Security Info=True;User ID=sa;Password=prosuite");
			conn.Open();
			DataSet ds = new DataSet();
			OleDbDataAdapter adapter = new OleDbDataAdapter("select idRecord from DEFECTS where DefectNum = " + iNum.ToString(), conn);
			adapter.Fill(ds);
			adapter.Dispose();
			conn.Close();
			conn.Dispose();
			
			if (ds.Tables[0].Rows.Count < 1) //someone took invalid tt id
				return iNum.ToString();

			HttpContext.Current.Application.Lock();
			mapIDs.Add(iNum, Convert.ToInt32(ds.Tables[0].Rows[0][0]));
			HttpContext.Current.Application.UnLock();
			return Convert.ToInt32(ds.Tables[0].Rows[0][0]).ToString();
		}
		public static DataSet GetPlanData(int user, int count = 80)
		{ 
			string sql = 
			string.Format(@"
				select top {1}
				d.DefectNum 
				,d.Summary
				,d.sModifier
				,d.estim as est
				,D.IORDER IORDER
				,D.IDDISPOSIT IDDISP
				,(select s.Descriptor from [tt_res].[dbo].[FLDSEVER] s where s.idRecord = d.idSeverity) sev
				,d.idSeverity idsev
				,RTRIM(D.USR) as usr
				,D.DATEENTER DATEENTER 
				from
				[tt_res].[dbo].[DEFECTS] d
				where
				RTRIM(D.USR) = '{0}'
				and (d.idDisposit <> 3 and d.idDisposit <> 17 and d.idDisposit <> 12)
				order by 
				d.iOrder desc,
				case when d.idSeverity = 0 then 0 else 1 end DESC,
				sev asc,
				d.idDisposit desc,
				est", GetUserEmail(user), count);
			return GetDataSet(sql);
		}
		public static DataSet Get2DoCount()
		{
			return GetDataSet(@"
				SELECT 
				(SELECT P.PERSON_NAME FROM PERSONS P WHERE P.WORK_EMAIL = USR) NAM
				,NUM
				FROM
				(SELECT 
				COUNT(DEFECTNUM) NUM
				, USR
				FROM 
				(SELECT 
				D.DEFECTNUM
				,RTRIM(D.USR) AS USR
				FROM [TT_RES].[DBO].[DEFECTS] D
				WHERE 
				IDSEVERITY IN (SELECT S.IDRECORD FROM [TT_RES].[DBO].[FLDSEVER] S WHERE S.DESCRIPTOR LIKE 'A%')
				AND D.IORDER IS NULL
				AND D.IDDISPOSIT = 1) TBL
				GROUP BY USR) TBL
			");
		}
		public static string[] GetPlan(int iuserID)
		{
			string strLevel = GetUserLevel(iuserID);

			DataSet dsTasks;
			if (strLevel.CompareTo("TESTER") == 0)
			{
				string strSQLTasks = @"
				SELECT top 20 
				d.DefectNum, d.Summary, d.idDisposit iddisp
				FROM [tt_res].[dbo].[DEFECTS] D 
				where 
				d.idDisposit = 3 and d.idCompon <> 96 
				order by dateModify desc";
				dsTasks = GetDataSet(strSQLTasks);
			}
			else
			{
				dsTasks = GetPlanData(iuserID);
			}

			string[] strResult = new string[dsTasks.Tables[0].Rows.Count];
			foreach (DataTable table in dsTasks.Tables)
			{
				int iIndex = 0;
				foreach (DataRow row in table.Rows)
				{
					if (Convert.ToInt32(row["idDisp"]) == 4)
						strResult[iIndex] = strResult[iIndex] + "<span class='reject'>Reject:</span> ";
					if (Convert.ToInt32(row["idDisp"]) == 10)
						strResult[iIndex] = strResult[iIndex] + "<span class='process'>Finishing:</span> ";
					if (Convert.ToInt32(row["idDisp"]) == 14)
						strResult[iIndex] = strResult[iIndex] + "<span class='onbst'>On BST:</span> ";

					strResult[iIndex] += "TT" + 
												row["DefectNum"].ToString() + 
												(strLevel.CompareTo("TESTER") == 0 ? " " :	" (" +
												(Convert.IsDBNull(row["est"]) ? 0 : Convert.ToInt32(row["est"])).ToString() + 
																											") ") +
												row["Summary"].ToString();
					iIndex++;
				}
			}
			return strResult;
		}
		public static int[] GetUpdatedTasks(int iuserID, DateTime dt)
		{
			string email = GetUserEmail(iuserID);
			DataSet ds = GetDataSet(@"			SELECT 
								(SELECT U.EMAILADDR FROM [TT_RES].[DBO].[USERS] U WHERE U.IDRECORD = L.USR) EML
								,(SELECT D.DEFECTNUM FROM [TT_RES].[DBO].[DEFECTS] D WHERE D.IDRECORD = L.TTID) TTID
								FROM
								(SELECT IDUSER USR
										,MAX(DATELOG) DATECHANGE
										,PARENTID TTID
									FROM [TT_RES].[DBO].[DEFLOG]
									WHERE DATELOG >= '" + dt.ToString("yyyy-MM-dd") + @"'
									GROUP BY PARENTID, IDUSER) L WHERE eml = '" + email + "'");
			int[] arr = new int[ds.Tables[0].Rows.Count];
			for (int i = 0; i < ds.Tables[0].Rows.Count; i ++)			
				arr[i] = Convert.ToInt32(ds.Tables[0].Rows[i][1]);
			return arr;
		}
		public string GetCommandLine(string strCommand)
		{
			System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strCommand);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true; 

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo = procStartInfo;
			proc.Start();

            var output = new List<string>();

            while (proc.StandardOutput.Peek() > -1)
            {
                output.Add(proc.StandardOutput.ReadLine());
            }

            while (proc.StandardError.Peek() > -1)
            {
                output.Add(proc.StandardError.ReadLine());
            }
            proc.WaitForExit();
            output.Reverse();
            return String.Join("<br/>", output.ToArray());;
		}
		public string GetVersionChanges()
		{
            string strV = Application["STRVER"] as string;
            if (string.IsNullOrEmpty(strV))
            {
                strV = GetCommandLine("git archive --remote=\\\\\\\\192.168.0.1\\\\git\\\\v8\\\\ HEAD:Projects.32 ChangeLog.txt");
                Application.Lock();
                Application["STRVER"] = strV;
                Application.UnLock();
            }
            return strV;
        }
		public static string GetTTID(string text)
		{
			Regex r = new Regex("TT\\d+", RegexOptions.IgnoreCase);
			Match m = r.Match(text);
			if (m.Success)
				return m.Value.Remove(0, 2);
			return "";
		}
		public List<int> GetTTIDsFromText(string text)
		{
			Regex r = new Regex("TT\\d+", RegexOptions.IgnoreCase);
			Match m = r.Match(text);
			List<int> ids = new List<int>();
			while (m.Success)
			{
				ids.Add(Convert.ToInt32(m.Value.Remove(0, 2)));
				m = m.NextMatch();
			}			
			return ids;
		}
		public string MergePlanWithToday(string[] plan, string today)
		{
			Regex r = new Regex("TT\\d+", RegexOptions.IgnoreCase);
			Match m = r.Match(today);
			List<int> removing = new List<int>();
			while (m.Success)
			{
				for (int i = 0; i < plan.Length; i++)
				{
					if (plan[i].IndexOf(m.Value) != -1)
					{
						if (!removing.Contains(i))
							removing.Add(i);
					}
				}
				m = m.NextMatch();
			}
			string[] res = new string[plan.Length - removing.Count];
			int ind = 0;
			for (int i = 0; i < plan.Length; i++)
			{
				if (removing.Contains(i))
					continue;
				res[ind] = plan[i];
				ind++;
			}
			return string.Join("<br/>", res);
		}
		public static DataSet GetOutDays(DateTime DT1, DateTime DT2)
		{			
			return GetDataSet(@"
					SELECT 
					 D.DEFECTNUM
					,D.DATEENTER
					,RTRIM(D.USR) AS USR
					,D.SUMMARY SUMMARY
					,(CASE WHEN D.SUMMARY LIKE '%SICK%' THEN 1 ELSE 0 END) SICK
					,(CASE WHEN D.SUMMARY LIKE '%UNPAID%' THEN 1 ELSE 0 END) UNPAID
					,8 est
					,(SELECT S.DESCRIPTOR FROM [TT_RES].[DBO].[FLDSEVER] S WHERE S.IDRECORD = D.IDSEVERITY) SEV
					,D.IDSEVERITY IDSEV
					,D.IDDISPOSIT IDDISP
					,D.IORDER IORDER
					FROM
					 [TT_RES].[DBO].[DEFECTS] D
					,[TT_RES].[DBO].[FLDCOMP] C
					WHERE
					D.IDCOMPON = C.IDRECORD
					AND C.DESCRIPTOR LIKE '%VACATION%'
					AND D.DATEENTER >= '" + DT1.ToString("yyyy-MM-dd") +  
					"' AND D.DATEENTER <= '" + DT2.ToString("yyyy-MM-dd") +
					"' ORDER BY D.DATEENTER");
			}
	}
}