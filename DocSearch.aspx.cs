using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GTOHELPER;

public partial class DocSearch : GTOHelper
{
	protected string Search
	{
		get
		{
			return Request.QueryString["text"];
		}
	}
   protected void Page_Load(object sender, EventArgs e)
    {
		 if (!string.IsNullOrEmpty(Search))
			 SearchText.Text = Search;

		 Image1.Visible = string.IsNullOrEmpty(Search);
		 if (!string.IsNullOrEmpty(Search))
		 {
			 string strText = "(FREETEXT(*,'" + SearchText.Text.Trim() + "'))";
			 string strQuery = "SELECT System.ItemPathDisplay, System.Title FROM SystemIndex where " + strText + "ORDER BY System.Title Asc";
			 
			 string connstring = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";

			 System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connstring);
			 conn.Open();

			 System.Data.OleDb.OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter(strQuery, conn);

			 System.Data.DataSet testDataSet = new System.Data.DataSet();

			 cmd.Fill(testDataSet, "SearchResults");
			 DataView source = new DataView(testDataSet.Tables[0]);

			 if (testDataSet.Tables[0].Rows.Count == 1)
			 {
				 Response.Redirect(GetDocURL((string)testDataSet.Tables[0].Rows[0][0]));
			 }

			 GridView1.DataSource = source;
			 GridView1.DataBind();
		 }
    }
	protected void SearchButton_Click(object sender, EventArgs e)
	 {
		 string strText = Request.Form[SearchText.ID];

		 if (string.IsNullOrEmpty(strText))
			 Response.End();

		 Response.Redirect(
									 CurrentPageName +
									 (string.IsNullOrEmpty(strText) ? "" : "?text=" + strText)
								 );
	 }
	protected void SearchText_TextChanged(object sender, EventArgs e)
	 {
		SearchButton_Click(sender, e);
	 }
	protected string GetDocURL(string strPath)
	{
		string strRoot = System.Configuration.ConfigurationManager.AppSettings["DocsDirectory"].ToUpper();
		string str1 = strPath.ToUpper().Replace(strRoot, "");
		return "http://mps.resnet.com/" + str1.Replace("\\", "/");
	}
	protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			DataRowView drv = (System.Data.DataRowView)e.Row.DataItem;
			string strRoot = System.Configuration.ConfigurationManager.AppSettings["DocsDirectory"].ToUpper();
			HyperLink hp = new HyperLink();
			string head = drv["System.Title"].ToString();
			hp.Text = string.IsNullOrEmpty(head) ? System.IO.Path.GetFileName(e.Row.Cells[0].Text) : head;
			hp.NavigateUrl = GetDocURL(e.Row.Cells[0].Text);
			e.Row.Cells[0].Controls.Add(hp);
		}
	}
}
