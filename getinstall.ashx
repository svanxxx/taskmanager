<%@ WebHandler Language="C#" Class="getinstall" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

public class getinstall : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		string t = context.Request.QueryString["type"].ToString();
		string v = context.Request.QueryString["version"].ToString();
		if (string.IsNullOrEmpty(v) || string.IsNullOrEmpty(t))
		{
			context.Response.ContentType = "text/plain";
			context.Response.Write(string.Format("invalid parameters:{0} {1}", t.ToString(), v.ToString()));
			return;
		}
		string lett = Regex.Replace(v, @"[\d-]", string.Empty).Replace(".", string.Empty);
		string folder = string.Format(@"{0}FIELDPRO_V8{1}\", Settings.CurrentSettings.INSTALLSFOLDER, lett);

		string ver = Regex.Replace(v, "[^0-9.]", string.Empty);
		string[] numbers = ver.Split('.');
		string verfolder = "FIELDPRO V8" + lett + " ";
		List<int> nums = new List<int>();
		foreach (string n in numbers)
		{
			int num;
			if (int.TryParse(n, out num))
			{
				nums.Add(num);
				verfolder += num.ToString() + "."; //replace 01 to 1
			}
		}
		verfolder = verfolder.Remove(verfolder.Length - 1);

		folder += verfolder + "\\";
		if (t == "efip" || t == "cx" || t == "onsite" || t == "demo" || t == "client" || t == "flex" || t == "devmx" || t == "devfip")
		{
			string download = "";
			if (t == "client")
			{
				download = Settings.CurrentSettings.INSTALLSFOLDER + Settings.CurrentSettings.FIELDPROCLIENT;
			}
			else if (t == "flex")
			{
				download = Settings.CurrentSettings.INSTALLSFOLDER + Settings.CurrentSettings.FLEXLMSERVER;
			}
			else if (t == "devfip")
			{
				download = string.Format("{0}{1}\\Release.zip", Settings.CurrentSettings.DEVINSTALLSFOLDER, v);
			}
			else if (t == "devmx")
			{
				download = string.Format("{0}{1}\\Modules.zip", Settings.CurrentSettings.DEVINSTALLSFOLDER, v);
			}
			else
			{
				string prefix = "";
				string postfix = "ACT";
				if (t == "efip")
				{
					prefix = "FIELDPRO_8";
				}
				else if (t == "cx")
				{
					prefix = "FIELDPRO_MODELS_ONSITE_REAL_TIME_8";
				}
				else if (t == "onsite")
				{
					prefix = "FIELDPRO_ONSITE_8";
				}
				else if (t == "demo")
				{
					prefix = "FIELDPRO_DEMO_DB_MSSQL_8";
					postfix = "BELACT";
				}
				download = string.Format("{0}{1}{2}_{3}_{4}_{5}_{6}.msi", folder, prefix, lett, nums[0], nums[1], nums[2], postfix);
			}

			if (!System.IO.File.Exists(download))
			{
				context.Response.ContentType = "text/plain";
				context.Response.Write("File not found:" + download);
				return;
			}
			context.Response.ContentType = "application/octet-stream";
			context.Response.AddHeader("Content-Length", (new System.IO.FileInfo(download)).Length.ToString());
			context.Response.AddHeader("Content-Disposition", "filename=" + '"' + System.IO.Path.GetFileName(download) + '"');
			context.Response.BinaryWrite(System.IO.File.ReadAllBytes(download));
			context.Response.Flush();
			context.Response.Close();
			context.Response.End();
			return;
		}
		context.Response.ContentType = "text/plain";
		context.Response.Write("Unsupported file type.");
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}