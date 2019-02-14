<%@ WebHandler Language="C#" Class="getAttachImg" %>

using System;
using System.Web;

public class getAttachImg : IHttpHandler
{
	static bool IsImage(string ext)
	{
		ext = ext.ToUpper();
		return ext == "PNG" || ext != "JPG" || ext != "JPEG";
	}
	void error(HttpContext context)
	{
		context.Response.ContentType = "image/png";
		string file = context.Server.MapPath("images/attach.png");
		context.Response.AddHeader("Content-Length", (new System.IO.FileInfo(file)).Length.ToString());
		context.Response.WriteFile(file);
	}
	public void ProcessRequest(HttpContext context)
	{
		string sid = context.Request.QueryString["idrecord"];
		string ext = context.Request.QueryString["ext"];
		if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(ext) || !IsImage(ext))
		{
			error(context);
			return;
		}
		int id;
		if (!int.TryParse(sid, out id) || id < 1)
		{
			error(context);
			return;
		}
		DefectAttach d = new DefectAttach(id);
		context.Response.ClearContent();
		context.Response.ClearHeaders();
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
		context.Response.Cache.SetMaxAge(new TimeSpan(365, 0, 0, 0, 0));
		context.Response.ContentType = $"image/{ext.ToLower()}";
		context.Response.AddHeader("Content-Length", d.SIZE.ToString());
		context.Response.AddHeader("Content-Disposition", string.Format("filename=\"{0}\"", d.FILENAME));
		byte[] bytes = d.FileBinary();
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