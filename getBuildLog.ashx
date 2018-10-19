<%@ WebHandler Language="C#" Class="getBuildLog" %>

using System;
using System.Web;

public class getBuildLog : IHttpHandler
{

	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";
		string file = Settings.CurrentSettings.BUILDLOGSDIR + context.Request.QueryString["id"].ToString() + ".log";
		if (System.IO.File.Exists(file))
		{
			context.Response.WriteFile(file);
		}
		else
		{
			context.Response.Write("File not found.");
		}
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

}