<%@ WebHandler Language="C#" Class="getTaskAttachment" %>

using System;
using System.Web;
using System.IO;

public class getTaskAttachment : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
	static string ReturnExtension(string fileExt)
	{
		switch (fileExt.ToLower())
		{
			case ".txt":
			case ".las": return "text/plain";
			case ".pdf": return "application/pdf";
			case ".swf": return "application/x-shockwave-flash";
			case ".gif": return "image/gif";
			case ".jpeg": return "image/jpg";
			case ".jpg": return "image/jpg";
			case ".png": return "image/png";
			case ".mp4": return "video/mp4";
			case ".mpeg": return "video/mpeg";
			case ".mov": return "video/quicktime";
			case ".wmv":
			case ".avi": return "video/x-ms-wmv";
			default: return "application/octet-stream";
		}
	}

	public void ProcessRequest(HttpContext context)
	{
		HttpRequest Request = context.Request;
		HttpResponse Response = context.Response;

		if (!CurrentContext.Validate(Response))
		{
			return;
		}

		string sid = Request.QueryString["idrecord"];
		int id;
		if (!int.TryParse(sid, out id) || id < 1)
		{
			Response.ContentType = "text/plain";
			Response.Write($"Invalid attachemment id format: '{sid}'");
			return;
		}
		if (!DefectAttach.Exists(id))
		{
			Response.ContentType = "text/plain";
			Response.Write($"Requested attachemment was not found. ID: '{sid}'");
			return;
		}
		DefectAttach d = new DefectAttach(id);
		string ext = Path.GetExtension(d.FILENAME);
		Response.ClearContent();
		Response.ClearHeaders();

		Response.ContentType = ReturnExtension(ext);
		Response.AddHeader("Content-Length", d.SIZE.ToString());
		Response.AddHeader("Content-Disposition", string.Format("filename=\"{0}\"", d.FILENAME));

		if (d.IsFileOnDisk)
		{
			Response.WriteFile(d.FileOnDisk);
			return;
		}
		else
		{
			byte[] bytes = d.FileBinary();
			Response.OutputStream.Write(bytes, 0, bytes.Length);
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