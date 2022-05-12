﻿<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Http" %>
<%@ Import Namespace="System.Web.Routing" %>

<script RunAt="server">
	void Application_Start(object sender, EventArgs e)
	{
		System.Web.Optimization.BundleTable.EnableOptimizations = true;
		string[] files = new string[] { "showtask", "mytr", "ttrep", "editplan", "users", "dispositions", "severities", "dailyreport", "machines", "components", "products", "types", "priorities", "vacations",
		"versionchanges", "statistics", "dailysearch", "builds", "branches", "settings", "commits", "builder", "defaults", "log", "merger", "tracker"};
		foreach (string file in files)
		{
			System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.ScriptBundle(string.Format("~/bundles/{0}_js", file)).Include(
				string.Format("~/scripts/references.js", file)
				, string.Format("~/scripts/defecthandlers.js", file)
				, string.Format("~/scripts/commonrefeditor.js", file)
				, string.Format("~/scripts/tables.js", file)
				, string.Format("~/scripts/{0}.js", file)
				));
			System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.StyleBundle(string.Format("~/bundles/{0}_css", file)).Include(
				string.Format("~/css/dataedit.css", file)
				, string.Format("~/css/{0}.css", file)
				));
		}
		string[] customscripts = new string[] { "buildshelper" };
		foreach (string file in customscripts)
		{
			System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.ScriptBundle(string.Format("~/bundles/{0}_js", file)).Include(
			string.Format("~/scripts/{0}.js", file)
			));
		}
		TasksBot.StartConnection();
		//SupportBot.StartConnection();
		DefectPlan.SatrtUpdaterEDD();
		RouteTable.Routes.MapHttpRoute(
			 name: "DefaultApi",
			 routeTemplate: "api/{controller}/{id}",
			 defaults: new { id = System.Web.Http.RouteParameter.Optional }
		);
	}
	void Application_End(object sender, EventArgs e)
	{
		DefectPlan.StopUpdaterEDD();
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
