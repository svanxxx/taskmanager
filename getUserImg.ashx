<%@ WebHandler Language="C#" Class="getUserImg" %>

using System;
using System.Web;
using System.Drawing;
using System.IO;

public class getUserImg : IHttpHandler
{
	void error(HttpContext context)
	{
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddHours(24));
		context.Response.Cache.SetMaxAge(new TimeSpan(24, 0, 0));
		context.Response.ContentType = "image/png";
		string file = context.Server.MapPath("images/img_avatar.png");
		context.Response.AddHeader("Content-Length", (new FileInfo(file)).Length.ToString());
		context.Response.WriteFile(file);
	}
	public void ProcessRequest(HttpContext context)
	{
		object sz = context.Request.QueryString["sz"];
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
		int id;
		if (!int.TryParse(sid, out id) || id < 1)
		{
			error(context);
			return;
		}
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddDays(10));
		context.Response.Cache.SetMaxAge(new TimeSpan(10, 0, 0, 0));
		context.Response.ContentType = "image/jpg";
		int? isz = null;
		if (sz != null)
		{
			isz = Convert.ToInt32(sz);
		};
		LoadImageFile(context, id, isz);
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
	static object _Lock = new object();
	void LoadImageFile(HttpContext context, int id, int? size)
	{
		string dir = $"images/cache/{ReferenceVersion.REFSVERSION()}";
		string newfilename = context.Server.MapPath($"{dir}scaled-id-{size}-sz-{id}.jpg");
		string origfilename = context.Server.MapPath($"{dir}orig-id-{id}.jpg");
		if (!File.Exists(origfilename) || !File.Exists(newfilename))
		{
			lock (_Lock)
			{
				if (!File.Exists(origfilename))
				{
					MPSUser u = new MPSUser(id);
					byte[] data = u.GetImage();
					if (data == null)
					{
						error(context);
						return;
					}
					File.WriteAllBytes(origfilename, data);
				}
				if (size == null)
				{
					File.Copy(origfilename, newfilename);
				}
				else
				{
					using (Bitmap orig = new Bitmap(origfilename))
					{
						using (Bitmap newb = new Bitmap((Image)orig, new Size(size.Value, size.Value)))
						{
							newb.Save(newfilename);
						}
					}
				}
			}
		}
		context.Response.AddHeader("Content-Length", (new FileInfo(newfilename)).Length.ToString());
		context.Response.WriteFile(newfilename);
		context.Response.Flush();
		context.Response.Close();
		context.Response.End();
	}
}