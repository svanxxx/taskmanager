using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTOHELPER;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;

public partial class Vacations : GTOHelper
{
	protected int Year
	{
		get
		{
			if (Request.Params["year"] == null || string.IsNullOrEmpty(Request.Params["year"].ToString()))
				return DateTime.Today.Year;
			return Convert.ToInt32(Request.Params["year"].ToString());
		}
	}
	protected DateTime DT1
	{
		get
		{
			return new DateTime(Year, 1, 1);
		}
	}
	protected DateTime DT2
	{
		get
		{
			return new DateTime(Year, 12, 31);
		}
	}
	protected string divstart
	{
		get
		{
			return "<div class='box'>";
		}
	}
	protected string divend
	{
		get
		{
			return "</div>";
		}
	}
	protected string DivText(int number)
	{
		return number > 0 ? divstart + number + divend : divstart + divend;
	}
	protected string DivText(string str)
	{
		return divstart + str + divend;
	}
	protected int GetDivNum(string text)
	{
		if (!text.StartsWith(divstart))
			return 0;
		text = text.Replace(divstart, "");
		text = text.Replace(divend, "");
		return string.IsNullOrEmpty(text) ? 0 : Convert.ToInt32(text);
	}
	protected string DivTextInc(string text)
	{
		return DivText(GetDivNum(text) + 1);
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		LastUpdatedTR.Value = GetDataSet("SELECT REPUPDATED FROM TRACKER").Tables[0].Rows[0][0].ToString();
		DataSet dtst = GetDataSet("SELECT PERSON_NAME, WORK_EMAIL FROM PERSONS WHERE IN_WORK = 1 ORDER BY PERSON_NAME");
		TimeSpan days = DT2 - DT1;

		TableRow tr = new TableRow();
		TableCell tc = new TableCell();

		HtmlGenericControl divcontainer = new HtmlGenericControl("div");
		divcontainer.Attributes["class"] = "row2";
		divcontainer.InnerHtml = "Month";
		tc.Controls.Add(divcontainer);
		tr.Cells.Add(tc);
		SNames.Rows.Add(tr);

		tr = new TableRow();
		tc = new TableCell();
		tc.Text = "Name";
		tc.Style.Add("border-width", "2px");
		tr.Cells.Add(tc);
		SNames.Rows.Add(tr);

		//adding months
		tr = new TableRow();
		DateTime dt1stYear = new DateTime(Year, 1, 1);
		for (int i = 0; i < days.Days + 1; i++)
		{
			tc = new TableCell();
			tc.Text = DivText(0);
			tc.CssClass = "mhead";

			if (dt1stYear.AddDays(i).Day == 1)
			{
				tc.Text = DivText(dt1stYear.AddDays(i).ToString("MMMM yyyy"));
				tc.CssClass += " mheadl";
			}
			else
			{
				if (dt1stYear.AddDays(i + 1).Day == 1)
					tc.CssClass += " mheadr";
			}
			tr.Cells.Add(tc);
		}
		STable.Rows.Add(tr);

		//adding days
		tr = new TableRow();
		for (int i = 0; i < days.Days + 1; i++)
		{
			tc = new TableCell();
			tc.Text = DivText(dt1stYear.AddDays(i).Day);
			tc.CssClass = "empty";
			if (dt1stYear.AddDays(i) == DateTime.Today)
			{
				tc.BackColor = System.Drawing.Color.BlueViolet;
				tc.BorderStyle = BorderStyle.Solid;
				tc.BorderWidth = 2;
			}
			tr.Cells.Add(tc);
		}
		STable.Rows.Add(tr);

		//main load
		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			//adding person
			tr = new TableRow();
			tc = new TableCell();
			divcontainer = new HtmlGenericControl("div");
			divcontainer.Attributes["class"] = "row2";
			divcontainer.InnerHtml = rowCur["PERSON_NAME"].ToString();
			tc.Controls.Add(divcontainer);
			tc.ToolTip = rowCur["WORK_EMAIL"].ToString();
			tr.Cells.Add(tc);

			tc = new TableCell();
			tc.CssClass = "sicklight";
			tc.Text = DivText(0);
			tr.Cells.Add(tc);

			tc = new TableCell();
			tc.CssClass = "vacationlight";
			tc.Text = DivText(0);
			tr.Cells.Add(tc);			
			
			SNames.Rows.Add(tr);

			//adding simple days
			tr = new TableRow();
			for (int i = 0; i < days.Days + 1; i++)
			{
				tc = new TableCell();
				tc.Text = DivText(0);
				tc.CssClass = (dt1stYear.AddDays(i).DayOfWeek == DayOfWeek.Saturday || dt1stYear.AddDays(i).DayOfWeek == DayOfWeek.Sunday) ?
																"weekend" :
																"empty";
				tr.Cells.Add(tc);
			}
			STable.Rows.Add(tr);
		}

		//load vacations
		dtst = GetDataSet(@"
			SELECT 
			D.DefectNum
			,D.Status
			,D.idDisposit
			,D.idCompon
			,D.dateEnter
			,(case when d.Summary like '%sick%' then 1 else 0 end) sick
			,(case when d.Summary like '%unpaid%' then 1 else 0 end) unpaid
			,RTRIM(D.USR) as usr
			  FROM [tt_res].[dbo].[DEFECTS] D
			  where  
			  d.idCompon = 96 and 
			  d.dateEnter >= '" + DT1.ToString("yyyy-MM-dd") + "' and d.dateEnter <= '" + DT2.ToString("yyyy-MM-dd") + "'");
		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			int iPers = 0;
			for (iPers = 2; iPers < SNames.Rows.Count; iPers++)
			{
				if (SNames.Rows[iPers].Cells[0].ToolTip.Equals(rowCur["usr"].ToString(), StringComparison.OrdinalIgnoreCase))
					break;
			}
			if (iPers != STable.Rows.Count)
			{
				int iCell = Convert.ToDateTime(rowCur["dateEnter"]).DayOfYear;
				tc = STable.Rows[iPers].Cells[iCell - 1];
				tc.CssClass = Convert.ToInt32(rowCur["sick"]) == 1 ? "sick" :
									(
										Convert.ToInt32(rowCur["unpaid"]) == 1 ? "unpaid" : "vacation"
									);
				tc.ToolTip = rowCur["DefectNum"].ToString();
				tc.Text = DivTextInc(tc.Text);
			}
		}

		//load working days
		dtst = GetDataSet(@"
								SELECT 
								 R.REPORT_DATE
								,P.WORK_EMAIL
								FROM REPORTS R, PERSONS P
								WHERE P.PERSON_ID = R.PERSON_ID
								AND R.REPORT_DATE  >= '" + DT1.ToString("yyyy-MM-dd") + "' and R.REPORT_DATE <= '" + DT2.ToString("yyyy-MM-dd") + "'");
		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			int iPers = 0;
			for (iPers = 2; iPers < SNames.Rows.Count; iPers++)
			{
				if (SNames.Rows[iPers].Cells[0].ToolTip.Equals(rowCur["WORK_EMAIL"].ToString(), StringComparison.OrdinalIgnoreCase))
					break;
			}
			if (iPers != STable.Rows.Count)
			{
				int iCell = Convert.ToDateTime(rowCur["REPORT_DATE"]).DayOfYear;
				tc = STable.Rows[iPers].Cells[iCell - 1];
				if (GetDivNum(tc.Text) > 0)
					tc.CssClass = "problem";
				else
					tc.CssClass = "work";
			}
		}

		//change current day
		if (DateTime.Today.Year == dt1stYear.Year)
		{
			for (int i = 2; i < STable.Rows.Count; i++)
				STable.Rows[i].Cells[DateTime.Today.DayOfYear - 1].CssClass += " today";
		}
	}
}