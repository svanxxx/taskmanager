using System.Collections.Generic;
using System.Data;
using System.Web;

public class Settings
{
	static string settKey = "current_settings";
	static object _lockobject = new object();
	public static Settings CurrentSettings
	{
		set
		{
			lock (_lockobject)
			{
				if (value == null)
				{
					HttpContext.Current.Application.Remove(settKey);
					return;
				}
				HttpContext.Current.Application[settKey] = value;
			}
		}
		get
		{
			lock (_lockobject)
			{
				if (HttpContext.Current.Application == null)
				{
					return null;
				}
				object outs = HttpContext.Current.Application[settKey];
				if (outs == null)
				{
					outs = new Settings(true);
					HttpContext.Current.Application[settKey] = outs;
				}
				return new Settings(outs as Settings);
			}
		}
	}

	static readonly string _Tabl = "[tt_res].[dbo].[SETTINGS]";
	string GetVal(string key)
	{
		string val = values.ContainsKey(key) ? values[key] : "";
		if (val.ToUpper().StartsWith("HTTPS://") && !HttpContext.Current.Request.IsSecureConnection)
		{
			return "http" + val.Substring(5);
		}
		return val;

	}
	string CheckSSLPath(string path)
	{
		return path;
	}

	public string SMTPHOST
	{
		get { return GetVal("smtp.Host"); }
		set { values["smtp.Host"] = value; }
	}
	public string SMTPPORT
	{
		get { return GetVal("smtp.Port"); }
		set { values["smtp.Port"] = value; }
	}
	public string SMTPENABLESSL
	{
		get { return GetVal("smtp.EnableSsl"); }
		set { values["smtp.EnableSsl"] = value; }
	}
	public string SMTPTIMEOUT
	{
		get { return GetVal("smtp.Timeout"); }
		set { values["smtp.Timeout"] = value; }
	}
	public string CREDENTIALS1
	{
		get { return GetVal("Credentials1"); }
		set { values["Credentials1"] = value; }
	}
	public string CREDENTIALS2
	{
		get { return GetVal("Credentials2"); }
		set { values["Credentials2"] = value; }
	}
	public string DEFLISTENERS
	{
		get { return GetVal("defListeners"); }
		set { values["defListeners"] = value; }
	}
	public string ANGULARCDN
	{
		get { return GetVal("ANGULARCDN"); }
		set { values["ANGULARCDN"] = value; }
	}
	public string JQUERYCDN
	{
		get { return GetVal("JQUERYCDN"); }
		set { values["JQUERYCDN"] = value; }
	}
	public string BOOTCSSCDN
	{
		get { return GetVal("BOOTCSSCDN"); }
		set { values["BOOTCSSCDN"] = value; }
	}
	public string BOOTSTRAPCDN
	{
		get { return GetVal("BOOTSTRAPCDN"); }
		set { values["BOOTSTRAPCDN"] = value; }
	}
	public string MPSCDN
	{
		get { return GetVal("MPSCDN"); }
		set { values["MPSCDN"] = value; }
	}
	public string COLRESIZABLECDN
	{
		get { return GetVal("COLRESIZABLECDN"); }
		set { values["COLRESIZABLECDN"] = value; }
	}
	public string CHARTSJSCDN
	{
		get { return GetVal("CHARTSJSCDN"); }
		set { values["CHARTSJSCDN"] = value; }
	}
	public string BSTSITE
	{
		get { return GetVal("BSTSITE"); }
		set { values["BSTSITE"] = value; }
	}
	public string WIKISITE
	{
		get { return GetVal("WIKISITE"); }
		set { values["WIKISITE"] = value; }
	}
	public string MPSWIKISITE
	{
		get { return GetVal("MPSWIKISITE"); }
		set { values["MPSWIKISITE"] = value; }
	}
	public string METASITE
	{
		get { return GetVal("METASITE"); }
		set { values["METASITE"] = value; }
	}
	public string COMPANYSITE
	{
		get { return GetVal("COMPANYSITE"); }
		set { values["COMPANYSITE"] = value; }
	}
	public string COMPANYNAME
	{
		get { return GetVal("COMPANYNAME"); }
		set { values["COMPANYNAME"] = value; }
	}
	public string BUILDLOGSDIR
	{
		get { return GetVal("BUILDLOGSDIR"); }
		set { values["BUILDLOGSDIR"] = value; }
	}

	Dictionary<string, string> values = new Dictionary<string, string>();
	void LoadData()
	{
		foreach (DataRow dr in DBHelper.GetRows(string.Format("SELECT * FROM {0}", _Tabl)))
		{
			values[dr["NAME"].ToString()] = dr["VALUE"].ToString();
		}
	}
	public Settings(bool loaddata)
	{
		if (loaddata)
		{
			LoadData();
		}
	}
	public Settings()
	{
	}
	public Settings (Settings o)
	{
		Settings s = new Settings();
		values = new Dictionary<string, string>(o.values);
	}
	public void Store()
	{
		foreach (string key in values.Keys)
		{
			DBHelper.SQLExecute(string.Format("INSERT INTO {0} ([NAME], [VALUE]) SELECT '{1}', '{2}' WHERE NOT EXISTS (SELECT * FROM {0} WHERE NAME = '{1}')", _Tabl, key, values[key]));
			DBHelper.SQLExecute(string.Format("UPDATE {0} SET [VALUE]='{2}' WHERE [NAME] = '{1}'", _Tabl, key, values[key]));
		}
		CurrentSettings = new Settings(this);
	}
}