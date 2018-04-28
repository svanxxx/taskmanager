using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Data.OleDb;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using GTOHELPER;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

public partial class dailyentry : GTOHelper
{
	protected string urlCUR_DATE = "date";
	protected DateTime mSelectedDate;

	protected void Page_Load(object sender, EventArgs e)
	{
		string strCUR_DATE = Request.QueryString[urlCUR_DATE];
		if (string.IsNullOrEmpty(strCUR_DATE))
			mSelectedDate = DateTime.Today;
		else
			mSelectedDate = DateTime.ParseExact(strCUR_DATE, "MM-dd-yyyy", CultureInfo.InvariantCulture);

		if (!IsPostBack)
		{
			ReloadControls();
		}
	}

	protected System.Web.UI.Control FindControlbyID(string ID, System.Web.UI.Control parent = null)
	{
		if (parent == null)
		{
			parent = this;
			return FindControlbyID(ID, parent);
		}

		foreach (System.Web.UI.Control ctrl in parent.Controls)
		{
			if (ctrl.ID == ID)
				return ctrl;

			System.Web.UI.Control ct = FindControlbyID(ID, ctrl);
			if (ct != null)
				return ct;
		}
		return null;
	}
	protected void ReloadControls()
	{
		bool bEnable = false;

		string strIMG = IsMyBirthday() ? "~/IMAGES/PERSONAL/BIRTHDAY.JPG" : "~/IMAGES/PERSONAL/" + UserID.ToString() + ".JPG";
		string strIMG2 = "~/IMAGES/PERSONAL/" + UserID.ToString() + "_desired.JPG";
		string strFile = Server.MapPath(strIMG);
		string strFile2 = strFile.Replace(".JPG", "_desired.JPG");
		PeronalImage.ImageUrl = System.IO.File.Exists(strFile2) ? strIMG2 : strIMG;
		if (!IsPostBack)
			((HtmlControl)FindControlbyID("repa")).Attributes["href"] = "~/DailySearch.aspx" + "?name=" + GetUserName(UserID); HtmlControl contentPanel1 = (HtmlControl)this.FindControl("contentPanel1");

		BirthdayLabel.Text = Birthdays(mSelectedDate);
		BirthdayLabel.Visible = !string.IsNullOrEmpty(BirthdayLabel.Text);

		string strSQL =
			"SELECT R.TIME_START, R.TIME_END, R.REPORT_DONE, R.REPORT_FUTURE, R.REPORT_BREAK " +
			"FROM REPORTS R WHERE R.REPORT_ID = " + GetReportID(mSelectedDate).ToString();

		DataSet dtst = GetDataSet(strSQL);
		if (0 < dtst.Tables[0].Rows.Count)
		{
			bEnable = true;
			DataRow rowCur = dtst.Tables[0].Rows[0];

			mTimeIN.Text = ((DateTime)rowCur["TIME_START"]).ToString("HH:mm");
			mTimeOut.Text = ((DateTime)rowCur["TIME_END"]).ToString("HH:mm");
			mBreakTime.Text = ((DateTime)rowCur["REPORT_BREAK"]).ToString("HH:mm");
			TodayText.Text = RTFConvert(rowCur["REPORT_DONE"].ToString());
			TomorrowText.Text = RTFConvert(rowCur["REPORT_FUTURE"].ToString());
		}
		else
		{
			mTimeIN.Text = "";
			mTimeOut.Text = "";
			mBreakTime.Text = "";
			TodayText.Text = "No Records Found...";
			TomorrowText.Text = "No Records Found...";
		}
		EnableEditControls(bEnable);
	}
	protected void EnableEditControls(bool bEnable)
	{
		mTimeIN.Enabled =
		mTimeOut.Enabled =
		mBreakTime.Enabled =
		TodayText.Enabled =
		TomorrowText.Enabled =
		Save.Enabled = bEnable;
	}

	protected string TI
	{
		get { return mTimeIN.Text; }
	}
	protected string TO
	{
		get { return mTimeOut.Text; }
	}
	protected string TB
	{
		get { return mBreakTime.Text; }
	}

	protected void Save_Click(object sender, ImageClickEventArgs e)
	{
		int nReportID = GetReportID(mSelectedDate);
		if (nReportID <= 0)
		{
			return;
		}

		OleDbConnection conn = NewConnection;
		conn.Open();
		try
		{
			OleDbCommand cmd = new OleDbCommand("UPDATE REPORTS SET TIME_START = ?, TIME_END = ?, REPORT_BREAK = ?, REPORT_DONE = ?, REPORT_FUTURE = ? WHERE REPORT_ID = ?", conn);

			string ti = TI;
			string to = TO;
			string tb = string.IsNullOrEmpty(Request.Form[mBreakTime.ID]) ? mBreakTime.Text : Request.Form[mBreakTime.ID];
			string tt = string.IsNullOrEmpty(Request.Form[TomorrowText.ID]) ? "" : Request.Form[TomorrowText.ID];
			string tn = string.IsNullOrEmpty(Request.Form[TodayText.ID]) ? "" : Request.Form[TodayText.ID];

			cmd.Parameters.AddWithValue("TIME_START", Convert.ToDateTime(ti).TimeOfDay);
			cmd.Parameters.AddWithValue("TIME_END", Convert.ToDateTime(to).TimeOfDay);
			cmd.Parameters.AddWithValue("REPORT_BREAK", Convert.ToDateTime(tb).TimeOfDay);
			cmd.Parameters.AddWithValue("REPORT_DONE", tn);
			cmd.Parameters.AddWithValue("REPORT_FUTURE", tt);
			cmd.Parameters.AddWithValue("REPORT_ID", nReportID);

			cmd.ExecuteNonQuery();
			if (GetUserLevel(UserID).CompareTo("TESTER") == 0)
				return;

			string[] lines = TodayText.Text.Replace("\r", "").Split('\n');
			string ttfinder = "";
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].StartsWith("Updated tasks"))
					continue;
				ttfinder += lines[i] + Environment.NewLine;
			}
			foreach (int i in GetTTIDsFromText(ttfinder))
			{
				SQLExecute(string.Format(@"
							UPDATE [tt_res].[dbo].[DEFECTS] set idDisposit = 10, Reference = 'P' + ISNULL(REFERENCE, '') 
							where idDisposit = 1 and DefectNum = {0}
						and RTRIM(USR) = '{1}'", i.ToString(), GetUserEmail(UserID)));
			}
		}
		finally
		{
			conn.Close();
		}
		Response.Redirect(GetPageURL(), false);
		Context.ApplicationInstance.CompleteRequest();
	}

	protected bool CopyLastDay()
	{
		HttpCookie co = Request.Cookies["_RTAJAX_CopyCheckBox" + UserID.ToString()];
		if (co == null)
			return false;
		return (co.Value == "true" || co.Value == "on");
	}
	protected int GetReportID(DateTime date)
	{
		string strSQl = "SELECT REPORT_ID FROM REPORTS WHERE REPORT_DATE=" + GetMSAccessDate(date) + " AND PERSON_ID=" + UserID.ToString();
		DataSet ds = GetDataSet(strSQl);
		if (0 < ds.Tables[0].Rows.Count)
			return (int)ds.Tables[0].Rows[0][0];
		return -1;
	}
	protected string GetPageURL()
	{
		return "dailyentry.aspx?" + urlCUR_DATE + "=" + mSelectedDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
	}
}