<%@ WebHandler Language="C#" Class="getUserImg" %>

using System;
using System.Web;

public class getUserImg : IHttpHandler
{
	void error(HttpContext context)
	{
		context.Response.ContentType = "text/plain";
		context.Response.Write("no image");
		context.Response.Flush();
		context.Response.Close();
		context.Response.End();
	}
	public void ProcessRequest(HttpContext context)
	{
		int id = Convert.ToInt32(context.Request.QueryString["id"]);
		if (id < 1)
		{
			error(context);
			return;
		}
		MPSUser u = new MPSUser(id);
		byte[] data = u.GetImage();
		if (data == null)
		{
			error(context);
			return;
		}
		else
		{
			context.Response.Cache.SetCacheability(HttpCacheability.Public);
			context.Response.Cache.SetExpires(DateTime.Now.AddHours(2));
			context.Response.Cache.SetMaxAge(new TimeSpan(2, 0, 0));
			context.Response.ContentType = "image/jpg";
			context.Response.AddHeader("Content-Length", data == null ? "0" : data.Length.ToString());
			context.Response.BinaryWrite(data);
		}
		context.Response.Flush();
		context.Response.Close();
		context.Response.End();
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}