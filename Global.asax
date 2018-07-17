﻿<%@ Application Language="C#" %>

<script RunAt="server">
	void Application_Start(object sender, EventArgs e)
	{
		System.Web.Optimization.BundleTable.EnableOptimizations = true;
		string[] files = new string[] { "showtask", "mytr", "ttrep", "editplan", "users", "dispositions", "severities", "dailyreport", "machines", "components", "products", "types", "priorities", "vacations" };
		foreach (string file in files)
		{
			System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.ScriptBundle(string.Format("~/bundles/{0}_js", file)).Include(
				string.Format("~/scripts/references.js", file)
				, string.Format("~/scripts/commonrefeditor.js", file)
				, string.Format("~/scripts/tables.js", file)
				, string.Format("~/scripts/{0}.js", file)
				));
			System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.StyleBundle(string.Format("~/bundles/{0}_css", file)).Include(
				string.Format("~/css/dataedit.css", file)
				, string.Format("~/css/{0}.css", file)
				));
		}
	}
	void Application_End(object sender, EventArgs e)
	{
	}
	void Session_Start(object sender, EventArgs e)
	{
	}
	void Application_Error(object sender, EventArgs e)
	{
	}
	void Session_End(object sender, EventArgs e)
	{
	}
</script>
