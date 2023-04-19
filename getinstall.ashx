<%@ WebHandler Language="C#" Class="getinstall" %>

using System.Net;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

public class getinstall : IHttpHandler
{
	const string _devZip = "devfip";
	const string _devMSI = "devfipMSI";
	const string _mxZip = "devmx";
	const string _mxMSI = "devmxMSI";
	static List<string> _devItems = new List<string>(new string[] { _devZip, _mxZip, _devMSI, _mxMSI });

	public void ProcessRequest(HttpContext context)
	{
		HttpRequest Request = context.Request;
		HttpResponse Response = context.Response;
		object ot = Request.QueryString["type"];
		object ov = Request.QueryString["version"];
		if (ot == null || ov == null)
		{
			Response.StatusCode = (int)HttpStatusCode.BadRequest;
			Response.StatusDescription = "Query parameters are invalid";
			Response.End();
		}

		string t = ot.ToString();
		string v = ov.ToString();
		if (string.IsNullOrEmpty(v) || string.IsNullOrEmpty(t))
		{
			Response.StatusCode = (int)HttpStatusCode.BadRequest;
			Response.StatusDescription = "Query parameters should not be empty";
			Response.End();
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
		if (numbers.Length != 3 && !_devItems.Contains(t))
		{
			Response.StatusCode = (int)HttpStatusCode.NotFound;
			Response.StatusDescription = "Requested installation file was not found";
			Response.End();
		}

		verfolder = verfolder.Remove(verfolder.Length - 1);

		folder += verfolder + "\\";
		if (t == "efip" || t == "cx" || t == "onsite" || t == "demo" || t == "client" || t == "flex" || _devItems.Contains(t))
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
			else if (t == _devZip)
			{
				download = string.Format("{0}{1}\\Release.zip", Settings.CurrentSettings.DEVINSTALLSFOLDER, v);
			}
			else if (t == _mxZip)
			{
				download = string.Format("{0}{1}\\Modules.zip", Settings.CurrentSettings.DEVINSTALLSFOLDER, v);
			}
			else if (t == _devMSI)
			{
				download = string.Format("{0}{1}\\FIELDPRO_SERVER.msi", Settings.CurrentSettings.DEVINSTALLSFOLDER, v);
			}
			else if (t == _mxMSI)
			{
				download = string.Format("{0}{1}\\FIELDPRO_MODELS_ONSITE_REAL_TIME.msi", Settings.CurrentSettings.DEVINSTALLSFOLDER, v);
			}
			else
			{
				string prefix = "";
				string postfix = "";
				if (t == "efip")
				{
					prefix = "FIELDPRO_SERVER_8";
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
					//postfix = "_BELACT";
				}
				download = string.Format("{0}{1}{2}_{3}_{4}_{5}{6}.msi", folder, prefix, lett, nums[0], nums[1], nums[2], postfix);
			}

			if (!System.IO.File.Exists(download))
			{
				Response.StatusCode = (int)HttpStatusCode.NotFound;
				Response.StatusDescription = $"Requested installation file was not found: {download}";
				Response.End();
			}
			Response.ContentType = "application/octet-stream";
			Response.AddHeader("Content-Length", (new System.IO.FileInfo(download)).Length.ToString());
			Response.AddHeader("Content-Disposition", "filename=" + '"' + System.IO.Path.GetFileName(download) + '"');
			Response.TransmitFile(download);
			return;
		}
		Response.StatusCode = (int)HttpStatusCode.BadRequest;
		Response.StatusDescription = "Unsupported file type requested";
		Response.End();
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}