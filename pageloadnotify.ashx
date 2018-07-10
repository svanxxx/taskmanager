<%@ WebHandler Language="C#" Class="pageloadnotify" %>

using System;
using System.Web;

public class pageloadnotify : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		int id = Convert.ToInt32(context.Request.QueryString["id"]);
		while (PageLoadNofify.IsLoading(id))
		{
				System.Threading.Thread.Sleep(1000);
		}
		context.Response.ContentType = "text/plain";
		context.Response.Write("OK");
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

}