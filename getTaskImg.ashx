<%@ WebHandler Language="C#" Class="getTaskImg" %>

using System;
using System.IO;
using System.Web;
using System.Drawing;

public class getTaskImg : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
	public void ProcessRequest(HttpContext context)
	{
		string basefile = context.Server.MapPath("images/taskicon.png");

		HttpRequest Request = context.Request;
		HttpResponse Response = context.Response;

		Response.ClearContent();
		Response.ClearHeaders();
		Response.ContentType = "image/png";
		Response.Cache.SetCacheability(HttpCacheability.Public);
		Response.Cache.SetExpires(DateTime.Now.AddDays(1));
		Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0, 0, 0));

		int id = Convert.ToInt32(Request.QueryString["id"]);
		if (id < 1)
		{
			Response.WriteFile(basefile);
			return;
		}
		LoadImageFile(context, id, basefile);
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
	static object _Lock = new object();
	void DellOldFiles(string dir, string validprefix)
	{
		var files = Directory.EnumerateFiles(dir);
		foreach (var file in files)
		{
			string filename = Path.GetFileName(file);
			if (!filename.StartsWith(validprefix))
			{
				File.Delete(file);
			}
		}
	}
	void LoadImageFile(HttpContext context, int id, string basefile)
	{
		string dir = context.Server.MapPath($"images/cache/dispo/");
		string prefix = ReferenceVersion.REFSVERSION();
		string filename = $"{dir}{prefix}_{id}.png";
		if (!File.Exists(filename))
		{
			lock (_Lock)
			{
				if (!File.Exists(filename))
				{
					DellOldFiles(dir, prefix);
					Color color = ColorTranslator.FromHtml(DefectDispo.GetDispColor(id));
					using (Bitmap bitmap = new Bitmap(basefile))
					{
						for (int x = 0; x < bitmap.Width; x++)
						{
							for (int y = 0; y < bitmap.Height; y++)
							{
								Color gotColor = bitmap.GetPixel(x, y);
								if (gotColor.A == 0)
								{
									bitmap.SetPixel(x, y, color);
								}
							}
						}
						bitmap.Save(filename);
					}
				}
			}
		}
		context.Response.AddHeader("Content-Length", (new FileInfo(filename)).Length.ToString());
		context.Response.WriteFile(filename);
		context.Response.Flush();
		context.Response.Close();
		context.Response.End();
	}
}