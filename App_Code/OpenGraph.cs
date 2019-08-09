﻿using System.Web;

public class OpenGraph : System.Web.UI.Page
{
	void getParam(string param, out string val)
	{
		if (HttpContext.Current == null || HttpContext.Current.Request == null)
		{
			val = "";
			return;
		}
		object o = HttpContext.Current.Request.QueryString[SecurityPage.returl];
		if (o == null)
		{
			val = "";
			return;
		}
		string findstr = $"{param}=";
		string s = o.ToString();
		int ind = s.IndexOf(findstr);
		if (ind < 0)
		{
			val = "";
			return;
		}
		val = s.Substring(ind + findstr.Length);
	}
	string _ttid = null;
	string GetPageTTID()
	{
		if (_ttid != null)
		{
			return _ttid;
		}
		getParam("ttid", out _ttid);
		return _ttid;
	}
	string _version = null;
	string GetPageVerson()
	{
		if (_version != null)
		{
			return _version;
		}
		getParam("version", out _version);
		return _version;
	}
	DefectBase defectedURl = null;
	DefectBase getDefect()
	{
		if (defectedURl == null)
		{
			string ttid = GetPageTTID();
			if (!string.IsNullOrEmpty(ttid))
			{
				defectedURl = new DefectBase(ttid);
			}
		}
		return defectedURl;
	}
	public string GetPageOgName()
	{
		if (!string.IsNullOrEmpty(GetPageVerson()))
		{
			return "Version: " + GetPageVerson();
		}
		string ttid = GetPageTTID();
		if (!string.IsNullOrEmpty(ttid))
		{
			return "TT" + ttid;
		}
		return "";
	}
	public string GetPageOgImage()
	{
		string basepath = HttpContext.Current.Request.Url.Scheme +
			"://" +
			HttpContext.Current.Request.Url.Host + ":" +
			HttpContext.Current.Request.Url.Port.ToString() +
			HttpContext.Current.Request.ApplicationPath;

		if (!string.IsNullOrEmpty(GetPageVerson()))
		{
			return basepath + $"/images/vlog.png";
		}
		string userid = GetPageUserID();
		if (userid != "")
		{
			return basepath + "/getUserImg.ashx?id=" + userid;
		}
		string img = GetPageCustomImg();
		if (!string.IsNullOrEmpty(img))
			return basepath + $"/images/{img}";

		DefectBase d = getDefect();
		int dispoid = -1;
		if (d != null)
		{
			dispoid = int.Parse(d.DISPO);
		}
		return $"{basepath}/getTaskImg.ashx?id={dispoid}";
	}
	string GetPageCustomImg()
	{
		if (HttpContext.Current == null || HttpContext.Current.Request == null)
		{
			return "";
		}
		object o = HttpContext.Current.Request.QueryString[StaticSettings.ogImg];
		if (o == null)
		{
			DefectBase d = getDefect();
			if (d != null && d.FIRE)
			{
				return "taskfire.png";
			}
			return "";
		}
		return o.ToString();
	}
	static string GetPageUserID()
	{
		if (HttpContext.Current == null || HttpContext.Current.Request == null)
		{
			return "";
		}
		object o = HttpContext.Current.Request.QueryString[SecurityPage.returl];
		if (o == null)
		{
			return "";
		}
		string findstr = "userid=";
		string s = o.ToString();
		int ind = s.IndexOf(findstr);
		if (ind < 0)
		{
			return "";
		}
		return s.Substring(ind + findstr.Length);
	}
	public string GetPageOgTitle()
	{
		string ttid = GetPageTTID();
		string userid = GetPageUserID();
		if (!string.IsNullOrEmpty(ttid))
		{
			DefectBase d = getDefect();
			if (d != null)
			{
				return d.SUMMARY.Replace("\"", "&quot;").Replace("\'", "&apos;");
			}
		}
		else if (!string.IsNullOrEmpty(userid))
		{
			return (new MPSUser(userid)).LOGIN + "'s Live Plan";
		}
		else if (!string.IsNullOrEmpty(GetPageVerson()))
		{
			return "Change Log";
		}
		return "";
	}
	public string GetPageOgDesc()
	{
		string ttid = GetPageTTID();
		string userid = GetPageUserID();
		if (!string.IsNullOrEmpty(ttid))
		{
			DefectBase d = getDefect();
			if (d != null)
			{
				return $"{d.GetTaskUserName()} ({d.ESTIM} hrs) - {d.GetTaskDispoName()}";
			}
		}
		else if (!string.IsNullOrEmpty(userid))
		{
			return (new MPSUser(userid)).PERSON_NAME;
		}
		return "Click to see details.";
	}
}