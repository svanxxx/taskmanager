<%@ WebHandler Language="C#" Class="defectNotify" %>

using System;
	using System.Web;

	public class defectNotify : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/event-stream";
			context.Response.CacheControl = "no-cache";
			context.Response.Flush();
			DateTime dtStart = DateTime.Now;
			int userid = int.Parse(context.Request.QueryString["userid"]);
			while (context.Response.IsClientConnected)
			{
				try
				{
					DateTime d = DefectEventsHandler.GetUserUpdateTime(userid);
					if (DateTime.Compare(d, dtStart) != 0)
					{
						dtStart = d;
						context.Response.Write("event: userchanged\ndata: true\n\n");
						context.Response.Flush();
					}
				}
				catch (Exception /*e*/) //connection is closed
				{
				}
				System.Threading.Thread.Sleep(10000);
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