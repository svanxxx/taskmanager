using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTOHELPER;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;

public partial class DailySearch : GTOHelper
{
	protected int rowNum = 0;
	protected string Search
	{
		get
		{
			return Request.QueryString["text"];
		}
	}
	protected string Name
	{
		get
		{
			return Request.QueryString["name"];
		}
	}
	protected string All
	{
		get
		{
			return "All";
		}
	}
	protected string MarkUp(string strText, string strHighlight)
	{
		if (string.IsNullOrEmpty(strHighlight))
			return strText;
		return ReplaceEx(strText, strHighlight, "<b>" + strHighlight + "</b>");
	}

	protected void Page_Load(object sender, EventArgs e)
   {
		PersonsList.Items.Clear();
		DataSet dtst = GetDataSet("SELECT PERSON_NAME FROM PERSONS WHERE IN_WORK = 1");
		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			PersonsList.Items.Add(rowCur["PERSON_NAME"].ToString());
		}
		PersonsList.Items.Add(All);
		if (PersonsList.Items.IndexOf(new ListItem(Name)) == -1)
			PersonsList.Items.Add(Name);

		PersonsList.Text = string.IsNullOrEmpty(Name) ? All : Name;

		if (Request.QueryString["opt"] != null)
			OptionsList.Text = Request.QueryString["opt"].ToString();
		else
			OptionsList.Text = All;

		if (!string.IsNullOrEmpty(Search))
			SearchText.Text = Search;

		Image1.Visible = string.IsNullOrEmpty(Search) && string.IsNullOrEmpty(Name);
		SqlDataSource1.ConnectionString = ConnString;
		if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Search))
		 {
			 SqlDataSource1.SelectCommand =
				"SELECT " +
				"REPORTS.TIME_START, " +
				"REPORTS.TIME_END, " +
				"REPORTS.REPORT_DATE, " +
				"REPORTS.REPORT_DONE, " +
				"REPORTS.REPORT_FUTURE, " +
				"PERSONS.PERSON_NAME " +
				"FROM " +
				"PERSONS , REPORTS " +
				"WHERE " +
				"PERSONS.PERSON_ID = -1";
		 }
		 else
		 {
			string strFields = "";
			switch (OptionsList.SelectedIndex)
			{
				case 0:
					strFields = " AND ( REPORTS.REPORT_DONE LIKE '%" + Search + "%' ) ";
					break;
				case 1:
					strFields = " AND ( REPORTS.REPORT_FUTURE LIKE '%" + Search + "%') ";
					break;
				default:
					strFields = " AND ( REPORTS.REPORT_DONE LIKE '%" + Search + "%' OR REPORTS.REPORT_FUTURE LIKE '%" + Search + "%') ";
					break;
			}

			 SqlDataSource1.SelectCommand =
				@"SELECT
				REPORTS.TIME_START,
				REPORTS.TIME_END,
				REPORTS.REPORT_DATE,
				REPORTS.REPORT_DONE,
				REPORTS.REPORT_FUTURE,
				PERSONS.PERSON_NAME
				FROM
				PERSONS , REPORTS
				WHERE
				PERSONS.PERSON_ID = REPORTS.PERSON_ID " +

				(string.IsNullOrEmpty(Name) ? "" : " AND PERSONS.PERSON_NAME = '" + Name + "' ") +
				(string.IsNullOrEmpty(Search) ? "" : strFields) +
				"ORDER BY REPORTS.REPORT_DATE DESC";
		 }
		 SearchGridView.DataBind();
    }
	protected void SearchGridView_RowDataBound(object sender, GridViewRowEventArgs e)
	 {
		 //in case memo text is trf text - converting it to plain text.
		 if (e.Row.RowType == DataControlRowType.DataRow)
		 {
				HyperLink hr = new HyperLink();
				hr.NavigateUrl = "?name=" + e.Row.Cells[1].Text + "&text=" + Search;
				hr.Text = e.Row.Cells[1].Text;
				e.Row.Cells[1].Controls.Add(hr);

				for (int i = 3; i <= 4; i++)
				{
					string[] arrItems = Regex.Split(MarkUp(ReplaceTT(RTFConvert(e.Row.Cells[i].Text)), Search), "<br/>");
					e.Row.Cells[i].Text = "<div>";
					e.Row.Cells[i].Text += "<div>";
					int iCounter = 0;
					foreach (string line in arrItems)
					{
						e.Row.Cells[i].Text += line + "<br/>";
						iCounter++;
						if (iCounter == 9)
						{
							e.Row.Cells[i].Text += "</div>";
							e.Row.Cells[i].Text += "<div id=main_"+e.Row.RowIndex+">";
							e.Row.Cells[i].Text += " <button id=btn_" + e.Row.RowIndex + " type='button' "+
														"onClick={"+
														"getElementById('child_" + e.Row.RowIndex + "').style.display='inline';"+
														"getElementById('btn_" + e.Row.RowIndex +"').style.display='none'" +
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

				e.Row.Cells[0].Text = (++rowNum).ToString();

				e.Row.Cells[2].Text = "";
				Label lb = new Label();
				lb.Font.Name = "Arial";
				lb.Font.Size = new FontUnit("10pt");

				DataRowView drv = (System.Data.DataRowView)e.Row.DataItem;
				lb.Text = Convert.ToDateTime(drv["REPORT_DATE"]).ToString("dd.MM.yyyy");
				e.Row.Cells[2].Controls.Add(lb);

				Literal lt = new Literal();
				lt.Text = "<br/>";
				e.Row.Cells[2].Controls.Add(lt);

				lb = new Label();
				lb.Font.Name = "Arial";
				lb.Font.Size = new FontUnit("7pt");
			 
				lb.Text =	Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "TIME_START")).ToString("HH:mm") + 
								" - " +
								Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "TIME_END")).ToString("HH:mm");
				e.Row.Cells[2].Controls.Add(lb);

				lt = new Literal();
				lt.Text = "<br/>";
				e.Row.Cells[2].Controls.Add(lt);

				lb = new Label();
				lb.Font.Name = "Arial";
				lb.Font.Size = new FontUnit("7pt");

				TimeSpan span = (Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "TIME_END"))).Subtract
										(
										Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "TIME_START"))
										);
			   lb.Text = span.Hours.ToString() + ":" + span.Minutes.ToString();
				if (span.Hours < 9)
					lb.BackColor = Color.Yellow;
				e.Row.Cells[2].Controls.Add(lb);

		 }
	 }
	protected void SearchButton_Click(object sender, EventArgs e)
	{
		string strOpt = Request.Form[OptionsList.ID];
		string strText = Request.Form[SearchText.ID];
		string strName = Request.Form[PersonsList.ID];

		if (string.IsNullOrEmpty(strText) && strName == "All")
			Response.Redirect(CurrentPageName);

		Response.Redirect(
									CurrentPageName + "?" +
									(string.IsNullOrEmpty(strText) ? "" : "text=" + strText + "&") +
									(strName == "All" ? "" : "name=" + strName + "&") +
									(string.IsNullOrEmpty(strOpt) ? "" : "opt=" + strOpt)
								, false);
		Context.ApplicationInstance.CompleteRequest();
	}
	protected void SearchText_TextChanged(object sender, EventArgs e)
	{
		SearchButton_Click(sender, e);
	}
}
