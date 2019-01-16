<%@ WebHandler Language="C#" Class="getUserImg" %>

using System;
using System.Web;

public class getUserImg : IHttpHandler
{
	void error(HttpContext context)
	{
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddHours(2));
		context.Response.Cache.SetMaxAge(new TimeSpan(2, 0, 0));
		context.Response.ContentType = "image/png";
		string file = context.Server.MapPath("images/img_avatar.png");
		context.Response.AddHeader("Content-Length", (new System.IO.FileInfo(file)).Length.ToString());
		context.Response.WriteFile(file);
	}
	public void ProcessRequest(HttpContext context)
	{
		string sid = context.Request.QueryString["id"];
		if (string.IsNullOrEmpty(sid))
		{
			string sttid = context.Request.QueryString["ttid"];
			if (string.IsNullOrEmpty(sttid))
			{
				string eml = context.Request.QueryString["eml"];
				if (string.IsNullOrEmpty(eml))
				{
					error(context);
					return;
				}
				DefectUser du = DefectUser.FindByEmail(eml);
				if (du == null)
				{
					error(context);
					return;
				}
				sid = du.TRID.ToString();
			}
			else
			{
				int ttid;
				if (!int.TryParse(sttid, out ttid) || ttid < 1)
				{
					error(context);
					return;
				}
				DefectUser du = new DefectUser(ttid);
				sid = du.TRID.ToString();
			}
		}
		int id = Convert.ToInt32(sid);
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