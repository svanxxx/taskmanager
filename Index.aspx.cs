using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Windows.Forms;
using GTOHELPER;
using System.Text.RegularExpressions;

public partial class _Default : GTOHelper
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Calendar1.SelectedDate = ReportDate;
		HyperLink2.NavigateUrl = "http://mps.resnet.com/tr/index.aspx";
		LoadData();
	}
	protected void LoadData()
	{
		LoadComment();
		LoadLabelText();
		LoadReport();

		EnableControls();
	}
	protected void EnableControls()
	{
		TodayButton.Enabled = (Calendar1.SelectedDate != DateTime.Today);
	}
	protected void LoadComment()
	{
		string strSQL = "SELECT MESSAGE FROM RSTATUSES WHERE SDATE = " +
								GetMSAccessDate(Calendar1.SelectedDate);
		System.Data.DataSet ds = GetDataSet(strSQL);
		TMESSAGET.Text = "";
		if (ds.Tables[0].Rows.Count > 0)
			TMESSAGET.Text = ds.Tables[0].Rows[0][0].ToString();
	}
	protected void LoadLabelText()
	{
		LatePersons.Text = "";
		MissingPersons.Text = "";
		Label1.Text = "For the date: " + Calendar1.SelectedDate.ToShortDateString();
	}
	protected DateTime ReportDate
	{
		get
		{
			if (Request.QueryString["date"] == null)
				return DateTime.Today;
			return new DateTime(Convert.ToInt64(Request.QueryString["date"].ToString()));
		}
	}
	protected void Calendar1_SelectionChanged(object sender, EventArgs e)
	{
		SetReportDate(Calendar1.SelectedDate);
	}
	protected void SetReportDate(DateTime dt)
	{
		Response.Redirect(CurrentPageName + "?date=" + dt.Ticks.ToString());
	}
	protected void LoadReport()
	{
		DateTime dt = Calendar1.SelectedDate;
		string strCurDate = GetMSAccessDate(dt);

		string strSQL =
		"select" +
		"	p.PERSON_NAME," +
		"	rep.TIME_START," +
		"	rep.TIME_END," +
		"	rep.PREV_REPORT_DONE," +
		"	rep.CUR_REPORT_DONE," +
		"	rep.REPORT_FUTURE " +
		"from (" +
		"	select" +
		"		r_cur.PERSON_ID as PERSON_ID," +
		"		r_cur.TIME_START AS TIME_START," +
		"		r_cur.TIME_END AS TIME_END," +
		"		r_prev.REPORT_DONE AS PREV_REPORT_DONE," +
		"		r_cur.REPORT_DONE AS CUR_REPORT_DONE," +
		"		r_cur.REPORT_FUTURE AS REPORT_FUTURE" +
		"	from" +
		"		REPORTS r_cur left join REPORTS r_prev on r_cur.PERSON_ID = r_prev.PERSON_ID AND r_prev.REPORT_DATE = (select top 1 REPORT_DATE from REPORTS where REPORT_DATE < r_cur.REPORT_DATE order by REPORT_DATE desc)" +
		"	where" +
		"		r_cur.REPORT_DATE = " + strCurDate +
		"	) rep right join PERSONS p on p.PERSON_ID = rep.PERSON_ID where p.IN_WORK = 1 ORDER BY p.PERSON_NAME";
		ReportDataSource2.SelectCommand = strSQL;
		ReportDataSource2.ConnectionString = ConnString;
	}
	protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		//in case memo text is trf text - converting it to plain text.
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			DateTime dt;
			if (e.Row.Cells[4].Text == "&nbsp;")
			{
				if (MissingPersons.Controls.Count == 0)
				{
					System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
					lbl.Text = "Missing Today: ";
					MissingPersons.Controls.Add(lbl);
				}
				HyperLink hr = new HyperLink();
				hr.NavigateUrl = "#" + e.Row.Cells[0].Text.Replace(" " ,"_");
				hr.Text = " " + e.Row.Cells[0].Text + "; ";
				MissingPersons.Controls.Add(hr);
			}
			if (e.Row.Cells[1].Text != "&nbsp;" &&
				DateTime.TryParse(e.Row.Cells[1].Text, out dt) && dt.Hour >= 10)
			{
				if (LatePersons.Controls.Count == 0)
				{
					System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
					lbl.Text = "Late Today: ";
					LatePersons.Controls.Add(lbl);
				}
				HyperLink hr = new HyperLink();
				hr.NavigateUrl = "#" + e.Row.Cells[0].Text.Replace(" ", "_");
				hr.Text = " " + e.Row.Cells[0].Text + "; ";
				LatePersons.Controls.Add(hr);
			}
			string strToday = e.Row.Cells[4].Text;
			for (int i = 3; i < 6; i++)
			{
				e.Row.Cells[i].Text = e.Row.Cells[i].Text.Trim();
				e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("\n", "<br/>");
				string[] arrItems = Regex.Split(ReplaceTT(RTFConvert(e.Row.Cells[i].Text)), "<br/>");
				e.Row.Cells[i].Text = "<div>";
				e.Row.Cells[i].Text += "<div>";
				int iCounter = 0;
				if (i == 5 && arrItems.Length == 1 && arrItems[0] == "&nbsp;")
				{
					string plan = MergePlanWithToday(GetPlan(GetUserIDByDispName(e.Row.Cells[0].Text)), strToday);
					arrItems = Regex.Split(ReplaceTT(plan), "<br/>");
				}
				foreach (string line in arrItems)
				{
					e.Row.Cells[i].Text += line + "<br/>";
					iCounter++;
					if (iCounter == 9)
					{
						e.Row.Cells[i].Text += "</div>";
						e.Row.Cells[i].Text += "<div id=main_" + e.Row.RowIndex + ">";
						e.Row.Cells[i].Text += " <button id=btn_" + e.Row.RowIndex + " type='button' " +
													"onClick={" +
													"getElementById('child_" + e.Row.RowIndex + "').style.display='inline';" +
													"getElementById('btn_" + e.Row.RowIndex + "').style.display='none'" +
													"}>...</button> ";
						e.Row.Cells[i].Text += "<div id=child_" + e.Row.RowIndex + " style='display:none'>";
					}
				}
				if (iCounter > 8)
				{
					e.Row.Cells[i].Text += "</div>";
					e.Row.Cells[i].Text += "</div>";
				}
				else
					e.Row.Cells[i].Text += "</div>";
				e.Row.Cells[i].Text += "</div>";
			}
			Literal lt = new Literal();
			lt.Text = "<a HREF='DailySearch.aspx?name=" + e.Row.Cells[0].Text + "' name=\"" + 
						e.Row.Cells[0].Text.Replace(" ", "_") + "\">" + e.Row.Cells[0].Text + "</a>";
			e.Row.Cells[0].Controls.Add(lt);
		}
	}
	protected void TodayButton_Click(object sender, EventArgs e)
	{
		SetReportDate(DateTime.Today);
	}
	protected void BackButton_Click(object sender, EventArgs e)
	{
		SetReportDate(Calendar1.SelectedDate.AddDays(-1));
	}
	protected void NextButton_Click(object sender, EventArgs e)
	{
		SetReportDate(Calendar1.SelectedDate.AddDays(1));
	}
	protected void SaveButton_Click(object sender, ImageClickEventArgs e)
	{
		string strSQL = "DELETE FROM RSTATUSES WHERE SDATE = " + GetMSAccessDate(Calendar1.SelectedDate);
		SQLExecute(strSQL);
		strSQL = "INSERT INTO RSTATUSES (SDATE, MESSAGE) VALUES (" + GetMSAccessDate(Calendar1.SelectedDate) + ", '" + Request.Form["TMESSAGET"] + "')";
		SQLExecute(strSQL);
		LoadComment();
		SwitchEditing();
		UpdatePanel1.Update();
	}
	protected void SwitchEditing()
	{
		SaveButton.Visible = !SaveButton.Visible;
		TMESSAGET.ReadOnly = !TMESSAGET.ReadOnly;
	}
	protected void EditButton_Click(object sender, ImageClickEventArgs e)
	{
		SwitchEditing();
	}
}