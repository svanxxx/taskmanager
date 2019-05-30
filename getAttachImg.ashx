<%@ WebHandler Language="C#" Class="getAttachImg" %>

using System;
using System.Web;
using System.IO;
using System.Drawing;

public class getAttachImg : IHttpHandler
{
	static bool IsImage(string ext)
	{
		ext = ext.ToUpper();
		return ext == "PNG" || ext == "JPG" || ext == "JPEG";
	}
	static bool IsStandardImg(HttpContext context, string ext)
	{
		string file = context.Server.MapPath($"images/ftypes/{ext}.png");
		if (File.Exists(file))
		{
			context.Response.ContentType = "image/png";
			context.Response.AddHeader("Content-Length", (new FileInfo(file)).Length.ToString());
			context.Response.WriteFile(file);
			return true;
		}
		return false;
	}
	void error(HttpContext context)
	{
		context.Response.ContentType = "image/png";
		string file = context.Server.MapPath("images/attach.png");
		context.Response.AddHeader("Content-Length", (new FileInfo(file)).Length.ToString());
		context.Response.WriteFile(file);
	}
	public void ProcessRequest(HttpContext context)
	{
		string sid = context.Request.QueryString["idrecord"];
		string ext = context.Request.QueryString["ext"];
		if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(ext))
		{
			error(context);
			return;
		}
		if (IsStandardImg(context, ext))
		{
			return;
		}
		Icon ico = null;
		if (!IsImage(ext))
		{
			string file = Path.GetTempPath() + "img." + ext;
			if (!File.Exists(file))
			{
				File.WriteAllText(file, string.Empty);
			}
			ico = Icon.ExtractAssociatedIcon(file);
		}

		int id;
		if (!int.TryParse(sid, out id) || id < 1)
		{
			error(context);
			return;
		}
		context.Response.ClearContent();
		context.Response.ClearHeaders();
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
		context.Response.Cache.SetMaxAge(new TimeSpan(365, 0, 0, 0, 0));
		context.Response.ContentType = $"image/{ext.ToLower()}";
		DefectAttach d = new DefectAttach(id);
		context.Response.AddHeader("Content-Disposition", string.Format("filename=\"{0}\"", d.FILENAME));
		byte[] bytes;
		if (ico != null)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				ico.Save(ms);
				bytes = ms.ToArray();
			}
			ico.Dispose();
		}
		else
		{
			bytes = d.FileBinary();
		}
		context.Response.AddHeader("Content-Length", bytes.Length.ToString());
		context.Response.OutputStream.Write(bytes, 0, bytes.Length);
		context.Response.Flush();
		context.ApplicationInstance.CompleteRequest();
	}
	public bool IsReusable
	{
		get
		{
			return true;
		}
	}
}