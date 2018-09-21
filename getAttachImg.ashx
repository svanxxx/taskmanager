<%@ WebHandler Language="C#" Class="getAttachImg" %>

using System;
using System.Web;

public class getAttachImg : IHttpHandler
{
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
		if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(ext) || ext.ToUpper() != "PNG")
		{
			error(context);
			return;
		}
		int id = Convert.ToInt32(sid);
		if (id < 1)
		{
			error(context);
			return;
		}
		DefectAttach d = new DefectAttach(id);
		context.Response.ClearContent();
		context.Response.ClearHeaders();
		context.Response.ContentType = "image/png";
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