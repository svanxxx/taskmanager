using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTOHELPER;
using System.Data;

public partial class EditPlan : GTOHelper
{
	protected string UserName
	{
		get
		{
			object obj = Request.Params["user"];
			if (Request.Params["user"] == null)
				return GetUserName(UserID);
			return Request.Params["user"] as string;
		}
	}
	protected int ShowBy
	{
		get
		{
			int n;
			if (int.TryParse(Request.QueryString["showby"], out n))
			{
				return n;
			}
			else
			{
				return 150;
			}
		}
	}
	protected void LoadTasks(int userID)
	{

		DataSet ds = GetPlanData(userID, ShowBy);
		foreach (DataRow rowCur in ds.Tables[0].Rows)
		{
			TableRow tr = new TableRow();
			TableCell tc = new TableCell();

			//tt id
			int iDisp = Convert.ToInt32(rowCur["idDisp"]);
			tc.Text = rowCur["DefectNum"].ToString();
			switch (iDisp)
			{
				case 10:
					tc.CssClass = "process";
					break;
				case 4:
					tc.CssClass = "reject";
					break;
				case 14:
					tc.CssClass = "onbst";
					break;
			}			
			tr.Cells.Add(tc);

			//plan
			tc = new TableCell();
			tc.Text = Convert.IsDBNull(rowCur["iorder"]) ? "" : "*";
			if (!string.IsNullOrEmpty(tc.Text))
				tc.CssClass = "pplan";
			tr.Cells.Add(tc);

			//estimation
			tc = new TableCell();
			tc.Text = rowCur["est"].ToString();
			tr.Cells.Add(tc);

			//summary
			tc = new TableCell();
			tc.Text = rowCur["Summary"].ToString(); ;
			if (tc.Text.Length > 150)
			{
				tc.ToolTip = tc.Text;
				tc.Text = tc.Text.Substring(0, 150) + "...";
			}
			tr.Cells.Add(tc);

			//severity
			tc = new TableCell();
			tc.Text = rowCur["sev"].ToString();
			if (tc.Text.Length > 20)
			{
				tc.ToolTip = tc.Text;
				tc.Text = tc.Text.Substring(0, 20);
			}
			tr.Cells.Add(tc);

			//Modifier			
			tc = new TableCell();
			tc.Text = rowCur["sModifier"].ToString();
			tr.Cells.Add(tc);

			TTasks.Rows.Add(tr);
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
			return;

		int iUser = GetUserIDByDispName(UserName);
		if (iUser == -1)
			iUser = UserID;
		LoadTasks(iUser);
		LoadUsers(iUser);
		UserHL.NavigateUrl = "DailySearch.aspx?name=" + UserName;
	}
	private void LoadUsers(int iUser)
	{
		UsersList.Items.Clear();
		DataSet dtst = GetDataSet("SELECT PERSON_ID, PERSON_NAME FROM PERSONS WHERE IN_WORK = 1 ORDER BY LEVEL_ID, PERSON_NAME");
		TableRow tr = new TableRow();

		foreach (DataRow rowCur in dtst.Tables[0].Rows)
		{
			string str = rowCur["PERSON_NAME"].ToString();
			if (!IsAdmin && !UserName.Equals(str))
				continue;

			UsersList.Items.Add(str);

			TableCell tc = new TableCell();
			tc.Text = string.Format("<img src='IMAGES/PERSONAL/{0}.JPG' class='pim'>", rowCur["PERSON_ID"].ToString());
			tc.ToolTip = str;
			tc.HorizontalAlign = HorizontalAlign.Center;
			tc.VerticalAlign = VerticalAlign.Middle;

			if (UserName.Equals(str))
			{
				tc.CssClass = "seluser";
				UsersList.SelectedIndex = UsersList.Items.Count - 1;
			}
			tr.Cells.Add(tc);
		}
		UsersTbl.Rows.Add(tr);
	}
	protected void UsersList_SelectedIndexChanged(object sender, EventArgs e)
	{
		Response.Redirect(CurrentPageName + "?user=" + UsersList.Items[UsersList.SelectedIndex] + "&showby=" + ShowBy.ToString(), false);
		Context.ApplicationInstance.CompleteRequest();
		return;
	}
}