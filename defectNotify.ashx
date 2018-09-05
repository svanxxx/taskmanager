<%@ WebHandler Language="C#" Class="DefectNotify" %>

using System;
using System.Web;

public class DefectNotify : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/event-stream";
		context.Response.CacheControl = "no-cache";
		context.Response.Flush();

		int userid = int.Parse(context.Request.QueryString["userid"]);
		DateTime dtStart = DefectEventsHandler.GetUserUpdateTime(userid);

		while (context.Response.IsClientConnected)
		{
			try
			{
				DateTime newdt = DefectEventsHandler.GetUserUpdateTime(userid);
				if (DateTime.Compare(newdt, dtStart) != 0)
				{
					dtStart = newdt;
					context.Response.Write("data: true\n\n");
					context.Response.Flush();
				}
			}
			catch (Exception /*e*/) //connection is closed
			{
			}
			System.Threading.Thread.Sleep(1000);
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