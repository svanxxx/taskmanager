using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTOHELPER;
using System.Globalization;

public partial class MonthlySum : GTOHelper
{
	protected string Year
	{
		get
		{
			return Request.QueryString["Year"] == null ? DateTime.Today.Year.ToString() : Request.QueryString["Year"].ToString();
		}
	}
	protected string Month
	{
		get
		{
			return Request.QueryString["Month"] == null ? DateTime.Today.Month.ToString() : Request.QueryString["Month"].ToString();
		}
	}
	private int GetWorkingDays()
	{
		DateTime dtmStart = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), 1);
		DateTime dtmEnd = dtmStart.AddMonths(1).AddDays(-1);

		// This function includes the start and end date if it falls on a weekday 
		int dowStart = ((int)dtmStart.DayOfWeek == 0 ? 7 : (int)dtmStart.DayOfWeek);
		int dowEnd = ((int)dtmEnd.DayOfWeek == 0 ? 7 : (int)dtmEnd.DayOfWeek);
		TimeSpan tSpan = dtmEnd - dtmStart;
		if (dowStart <= dowEnd)
		{
			return (((tSpan.Days / 7) * 5) + Math.Max((Math.Min((dowEnd + 1), 6) - dowStart), 0));
		}
		else
		{
			return (((tSpan.Days / 7) * 5) + Math.Min((dowEnd + 6) - Math.Min(dowStart, 6), 5));
		}
	}
    protected void Page_Load(object sender, EventArgs e)
    {
		 DateTime dt = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), 1);

		 TitleLabel.Text = "Summary hours for " + dt.ToString("MMMM yyyy") + " ("+GetWorkingDays().ToString()+" working days)";
		 SqlDataSource1.ConnectionString = ConnString;
		 SqlDataSource1.SelectCommand =
			"SELECT *, (HOURS / 8) AS DAYS FROM ( " +
			"SELECT PERSONS.PERSON_NAME AS NAME, SUM(CAST(TIME_END as FLOAT) - CAST(TIME_START as FLOAT) - 0.041666666666666666666666666666667)*24 AS HOURS " +
			"FROM  "+
			"PERSONS, REPORTS "+
			"WHERE PERSONS.PERSON_ID = REPORTS.PERSON_ID AND  " +
			"MONTH(REPORT_DATE) = " + Month + " and YEAR(REPORT_DATE) = " + Year + " GROUP BY PERSONS.PERSON_NAME " +
			") tmp";
		 GridView1.DataBind();
	 }
	 protected void PrevButton_Click(object sender, EventArgs e)
	 {
		 string strMonth = Month;
		 string strYear = Year;
		 if (Convert.ToInt32(strMonth) == 1)
		 {
			 strMonth = (12).ToString();
			 strYear = (Convert.ToInt32(strYear) - 1).ToString();
		 }
		 else
			 strMonth = (Convert.ToInt32(strMonth) - 1).ToString();
		 Response.Redirect(CurrentPageName + "?Year=" + strYear + "&Month=" + strMonth);
	 }
	 protected void NextButton_Click(object sender, EventArgs e)
	 {
		 string strMonth = Month;
		 string strYear = Year;
		 if (Convert.ToInt32(strMonth) == 12)
		 {
			 strMonth = (1).ToString();
			 strYear = (Convert.ToInt32(strYear) + 1).ToString();
		 }
		 else
			 strMonth = (Convert.ToInt32(strMonth) + 1).ToString();
		 Response.Redirect(CurrentPageName + "?Year=" + strYear + "&Month=" + strMonth);
	 }
	 protected void ThisButton_Click(object sender, EventArgs e)
	 {
		 DateTime dt = DateTime.Today;
		 Response.Redirect(CurrentPageName + "?Year=" + dt.Year + "&Month=" + dt.Month);
	 }
	 protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	 {
		 if (e.Row.RowType == DataControlRowType.DataRow)
		 {
			 if (Convert.ToDouble(e.Row.Cells[2].Text) < GetWorkingDays())
			 {
				 for (int i = 0; i < e.Row.Cells.Count; i++)
				 {
					 e.Row.Cells[i].BackColor = System.Drawing.Color.Yellow;
				 }
			 }
			 HyperLink hr = new HyperLink();
			 hr.Text = e.Row.Cells[1].Text;
			 hr.NavigateUrl = "DailySearch.aspx?name=" + e.Row.Cells[1].Text;
			 e.Row.Cells[1].Controls.Add(hr);
		 }
	 }
}