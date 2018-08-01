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
		string folder = string.Format(@"\\192.168.0.1\FiP Installations\FIELDPRO_V8{0}\", lett);

		string ver = Regex.Replace(v, "[^0-9.]", string.Empty);
		string[] numbers = ver.Split('.');
		string verfolder = "FIELDPRO V8" + lett + " ";
		List<int> nums = new List<int>();
		foreach (string n in numbers)
		{
			var num = Convert.ToInt32(n);
			nums.Add(num);
			verfolder += num.ToString() + "."; //replace 01 to 1
		}
		verfolder = verfolder.Remove(verfolder.Length - 1);

		folder += verfolder + "\\";
		if (t == "efip" || t == "cx" || t == "onsite")
		{
			string download = "";
			if (t == "efip")
			{
				download = folder + string.Format("eFIELDPRO_8{0}_{1}_{2}_{3}_ACT.msi", lett, nums[0], nums[1], nums[2]);
			}
			else if (t == "cx")
			{
				download = folder + string.Format("FIELDPRO_ONSITE_MODELS_REAL_TIME_8{0}_{1}_{2}_{3}_ACT.msi", lett, nums[0], nums[1], nums[2]);
			}
			else if (t == "onsite")
			{
				download = folder + string.Format("FIELDPRO_ONSITE_8{0}_{1}_{2}_{3}_ACT.msi", lett, nums[0], nums[1], nums[2]);
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