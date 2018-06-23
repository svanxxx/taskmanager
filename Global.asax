<%@ Application Language="C#" %>

<script RunAt="server">
	void Application_Start(object sender, EventArgs e)
	{
		System.Web.Optimization.BundleTable.EnableOptimizations = true;
		string[] files = new string[] { "showtask", "mytr", "ttrep", "editplan", "users", "dispositions", "severities", "dailyreport", "machines","components" };
		foreach (string file in files)
		{
			System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.ScriptBundle(string.Format("~/bundles/{0}_js", file)).Include(
				string.Format("~/scripts/references.js", file)
				,string.Format("~/scripts/{0}.js", file)
				));
			System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.StyleBundle(string.Format("~/bundles/{0}_css", file)).Include(
				string.Format("~/css/{0}.css", file)
				));
		}
	}
	void Application_End(object sender, EventArgs e)
	{
	}
	void Session_Start(object sender, EventArgs e)
	{
		Application.Lock();

		StringDictionary item = Application["USERSONLINE"] as StringDictionary;
		if (item == null)
		{
			item = new StringDictionary();
			Application["USERSONLINE"] = item;
		}
		HttpCookie co = Request.Cookies["userid"];
		item[Session.SessionID] = co == null ? "" : co.Value;

		//loading testtrack data ids vs. numbers
		System.Collections.Generic.Dictionary<int, int> mapIDs = new System.Collections.Generic.Dictionary<int, int>();
		System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection("Provider=SQLOLEDB;Data Source=192.168.0.1;Initial Catalog=tt_res;Persist Security Info=True;User ID=sa;Password=prosuite");
		conn.Open();
		System.Data.DataSet ds = new System.Data.DataSet();
		System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter("select DefectNum, idRecord from DEFECTS", conn);
		adapter.Fill(ds);
		adapter.Dispose();
		conn.Close();
		conn.Dispose();
		foreach (System.Data.DataRow rowCur in ds.Tables[0].Rows)
		{
			mapIDs.Add(Convert.ToInt32(rowCur[0]), Convert.ToInt32(rowCur[1]));
		}
		Application["MAPTTIDS"] = mapIDs;

		Application.UnLock();
	}
	void Application_Error(object sender, EventArgs e)
	{

	}
	void Session_End(object sender, EventArgs e)
	{
		Application.Lock();
		StringDictionary item = Application["USERSONLINE"] as StringDictionary;
		if (item == null)
		{
			item = new StringDictionary();
			Application["USERSONLINE"] = item;
		}
		item.Remove(Session.SessionID);
		Application.UnLock();
	}
</script>
