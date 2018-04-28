using GTOHELPER;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ttrep : GTOHelper
{
	const string sALL = "-All-";
	protected string SText
	{
		get
		{
			if (Request.Params["stext"] == null)
				return "";
			return Request.Params["stext"] as string;
		}
	}
	protected string FilterDate
	{
		get
		{
			if (Request.Params["date"] == null)
				return "";
			return Request.Params["date"] as string;
		}
	}
	protected string Severity
	{
		get
		{
			if (Request.Params["severity"] == null)
				return sALL;
			return Request.Params["severity"] as string;
		}
	}
	protected string RepUsr
	{
		get
		{
			if (Request.Params["repusr"] == null)
				return sALL;
			return Request.Params["repusr"] as string;
		}
	}
	protected string Disposition
	{
		get
		{
			if (Request.Params["disposition"] == null)
				return sALL;
			return Request.Params["disposition"] as string;
		}
	}
	protected string FilterDateYear
	{
		get
		{
			string str = FilterDate;
			if (string.IsNullOrEmpty(str))
				return "";
			return DateTime.ParseExact(str, "ddMMyyyy", CultureInfo.InvariantCulture).Year.ToString();
		}
	}
	protected string FilterSText
	{
		get
		{
			string str = SText;
			if (string.IsNullOrEmpty(str))
				return "";
			if (str.IndexOf('"') == 0 && str.LastIndexOf('"') == str.Length - 1)
				return string.Format(@" D.Summary like '%{0}%' ", SText.Substring(1, str.Length - 2));
			string[] vals = str.Split(' ');
			string filter = "";
			foreach (string val in vals)
			{
				val.Trim();
				if (string.IsNullOrEmpty(val))
					continue;
				if (string.IsNullOrEmpty(filter))
					filter = string.Format(@" D.Summary like '%{0}%' ", val);
				else
					filter += string.Format(@" AND D.Summary like '%{0}%' ", val);
			}
			return "( " + filter + " )";
		}
	}
	protected string FilterDateMonth
	{
		get
		{
			string str = FilterDate;
			if (string.IsNullOrEmpty(str))
				return "";
			return DateTime.ParseExact(str, "ddMMyyyy", CultureInfo.InvariantCulture).Month.ToString();
		}
	}
	protected string mainSQL
	{
		get
		{
			string sql = @"
					SELECT TOP 500 * FROM (SELECT 
			D.DEFECTNUM AS NUM
			,D.IDSEVERITY AS IDSEVERITY
			,D.IDDISPOSIT AS IDDISPOSIT
			,D.DATEENTER AS DATEENTER
			,(SELECT TOP 1 E.TIMESPENT FROM [TT_RES].[DBO].DEFECTEVTS E WHERE E.EVTDEFID = 2 AND E.PARENTID = D.IDRECORD ORDER BY E.ORDERNUM DESC) AS EST
			,D.SUMMARY
			,RTRIM(D.USR) AS USR
			,(SELECT S.DESCRIPTOR FROM [TT_RES].[DBO].[FLDSEVER] S WHERE S.IDRECORD = D.IDSEVERITY) SEVERITY
			,(SELECT DI.DESCRIPTOR FROM [TT_RES].[DBO].[FLDDISPO] DI WHERE DI.IDRECORD = D.IDDISPOSIT) DISPOSITION
			,D.DATECREATE
			FROM 
			[TT_RES].[DBO].[DEFECTS] D WHERE D.IDDISPOSIT <> 12) D ";

			if (Request.Params.Count < 1)
				return sql;

			string[] where = new string[Request.Params.Count];
			if (!string.IsNullOrEmpty(Severity) && Severity != sALL)
			{
				int ind = Array.FindIndex(where, i => i == null || i.Length == 0);
				where[ind] = string.Format("(D.IDSEVERITY IN (SELECT S.IDRECORD FROM [tt_res].[dbo].[FLDSEVER] S WHERE S.DESCRIPTOR LIKE '%{0}%'))", Severity);
			}
			if (!string.IsNullOrEmpty(Disposition) && Disposition != sALL)
			{
				int ind = Array.FindIndex(where, i => i == null || i.Length == 0);
				where[ind] = string.Format("(D.IDDISPOSIT IN (SELECT DI.IDRECORD FROM [tt_res].[dbo].[FLDDISPO] DI WHERE DI.DESCRIPTOR LIKE '%{0}%'))", Disposition);
			}
			if (!string.IsNullOrEmpty(RepUsr) && RepUsr != sALL)
			{
				int ind = Array.FindIndex(where, i => i == null || i.Length == 0);
				where[ind] = string.Format("(RTRIM(D.USR) IN (SELECT P.WORK_EMAIL FROM PERSONS P WHERE P.PERSON_NAME LIKE '%{0}%'))", RepUsr);
			}
			if (!string.IsNullOrEmpty(FilterDate))
			{
				int ind = Array.FindIndex(where, i => i == null || i.Length == 0);
				where[ind] = string.Format(@"((DATEPART(M, D.DATECREATE) = {0} AND DATEPART(YYYY, D.DATECREATE) = {1})
														OR (DATEPART(M, D.DATEENTER) = {0} AND DATEPART(YYYY, D.DATEENTER) = {1}
														AND D.IDDISPOSIT != 1))", FilterDateMonth, FilterDateYear);
			}
			if (!string.IsNullOrEmpty(FilterSText))
			{
				int ind = Array.FindIndex(where, i => i == null || i.Length == 0);
				where[ind] = FilterSText;
			}
			int count = Array.FindIndex(where, i => i == null || i.Length == 0);
			string strwhere = "";
			for (int i = 0; i < count; i++)
			{
				strwhere += (i == 0 ? " WHERE " : " AND ") + where[i];
			}
			return sql + strwhere + " order by NUM desc";
		}
	}

	protected void LoadUsers()
	{
		TTUser.Items.Clear();
		TTUser.Items.Add(sALL);
		DataSet dtst = GetDataSet(@"
				SELECT 
					IDRECORD
					,(SELECT P.PERSON_NAME FROM PERSONS P WHERE P.WORK_EMAIL = U.EMAILADDR) NAME
				FROM 
					[TT_RES].[DBO].[USERS] U
				WHERE
					U.EMAILADDR IN (SELECT P.WORK_EMAIL FROM PERSONS P WHERE P.IN_WORK = 1)");

		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			string str = rowCur["NAME"].ToString();
			TTUser.Items.Add(str);
			if (RepUsr.Equals(str))
			{
				TTUser.SelectedIndex = TTUser.Items.Count - 1;
			}
		}
	}
	protected void LoadSeverity()
	{
		TTSeverity.Items.Clear();
		TTSeverity.Items.Add(sALL);
		DataSet dtst = GetDataSet(@"
						SELECT [idRecord]
								,[Descriptor]
							FROM [tt_res].[dbo].[FLDSEVER]
						order by [Descriptor]");

		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			string str = rowCur["Descriptor"].ToString();
			TTSeverity.Items.Add(str);
			if (Severity.Equals(str))
			{
				TTSeverity.SelectedIndex = TTSeverity.Items.Count - 1;
			}
		}
	}
	protected void LoadDisposition()
	{
		TTDisposition.Items.Clear();
		TTDisposition.Items.Add(sALL);
		DataSet dtst = GetDataSet(@"
						SELECT [idRecord]
								,[Descriptor]
							FROM [tt_res].[dbo].[FLDDISPO]
						order by [Descriptor]");

		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			string str = rowCur["Descriptor"].ToString();
			TTDisposition.Items.Add(str);
			if (Disposition.Equals(str))
			{
				TTDisposition.SelectedIndex = TTDisposition.Items.Count - 1;
			}
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		LoadUsers();
		LoadSeverity();
		LoadDisposition();
		LoadTable();
	}
	private void LoadTable()
	{
		DataSet dtst = GetDataSet(mainSQL);
		int iNum = 0;
		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			TableRow tr = new TableRow();
			TableCell tc = new TableCell();
			tc.Text = (++iNum).ToString();
			tr.Cells.Add(tc);

			foreach (DataColumn dc in dtst.Tables[0].Columns)
			{
				if (dc.ColumnName.ToUpper().StartsWith("ID"))
					continue;

				if (!string.IsNullOrEmpty(Severity) && Severity != sALL && dc.ColumnName.ToUpper().Equals("SEVERITY"))
					continue;

				if (!string.IsNullOrEmpty(Disposition) && Disposition != sALL && dc.ColumnName.ToUpper().Equals("DISPOSITION"))
					continue;

				if (!string.IsNullOrEmpty(RepUsr) && RepUsr != sALL && dc.ColumnName.ToUpper().Equals("USR"))
					continue;

				tc = new TableCell();
				object obj = rowCur[dc];
				if (obj.GetType() == typeof(DateTime))
					tc.Text = Convert.ToDateTime(obj).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
				else if (obj.GetType() == typeof(decimal))
					tc.Text = Convert.ToInt32(obj).ToString();
				else
					tc.Text = obj.ToString();

				if (dc.ColumnName.ToUpper().Equals("DISPOSITION") && tc.Text.Length > 10)
				{
					tc.ToolTip = tc.Text;
					tc.Text = tc.Text.Substring(0, 10);
				}
				else if (tc.Text.Length > 125)
				{
					tc.ToolTip = tc.Text;
					tc.Text = tc.Text.Substring(0, 125);
				}
				tr.Cells.Add(tc);
				
				string disp = rowCur["DISPOSITION"].ToString().ToUpper();
				if (disp.IndexOf("REJECT") != -1)
				{
					tr.CssClass = "reject";
				}
				else if (disp.IndexOf("PROCESS") != -1)
				{
					tr.CssClass = "process";
				}
				else if (disp.IndexOf("BST") != -1)
				{
					tr.CssClass = "onbst";
				}
				else if (disp.IndexOf("REQUESTED") != -1)
				{
					tr.CssClass = "requested";
				}
				else if (disp.IndexOf("FINISHED") != -1)
				{
					tr.CssClass = "finished";
				}
				else if (disp.IndexOf("TESTED") != -1)
				{
					tr.CssClass = "tested";
				}
			}
			TTTable.Rows.Add(tr);
		}
	}
}