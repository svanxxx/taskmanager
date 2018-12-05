using System.Collections.Generic;
using System.Data;
using System.Web;

public class RawSettings
{
	static object _lockobject = new object();
	static RawSettings _CurrentSettings = null;
	public static Settings CurrentSettings
	{
		get { return new Settings(CurrentRawSettings); }
		set { CurrentRawSettings = null; }
	}
	public static RawSettings CurrentRawSettings
	{
		set
		{
			lock (_lockobject)
			{
				_CurrentSettings = value;
			}
		}
		get
		{
			lock (_lockobject)
			{
				if (_CurrentSettings == null)
				{
					_CurrentSettings = new RawSettings(true);
				}
				return new RawSettings(_CurrentSettings);
			}
		}
	}

	static readonly string _Tabl = "[tt_res].[dbo].[SETTINGS]";
	protected virtual string ProcessValue(string val)
	{
		return val;
	}
	string GetVal(string key)
	{
		string val = values.ContainsKey(key) ? values[key] : "";
		return ProcessValue(val);
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
	public string BSTSITESERVICE
	{
		get { return GetVal("BSTSITESERVICE"); }
		set { values["BSTSITESERVICE"] = value; }
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
	public string INSTALLSFOLDER
	{
		get { return GetVal("INSTALLSFOLDER"); }
		set { values["INSTALLSFOLDER"] = value; }
	}
	public string FIELDPROCLIENT
	{
		get { return GetVal("FIELDPROCLIENT"); }
		set { values["FIELDPROCLIENT"] = value; }
	}
	public string FLEXLMSERVER
	{
		get { return GetVal("FLEXLMSERVER"); }
		set { values["FLEXLMSERVER"] = value; }
	}
	public string BUILDTIME
	{
		get { return GetVal("BUILDTIME"); }
		set { values["BUILDTIME"] = value; }
	}
	public string RELEASETTID
	{
		get { return GetVal("RELEASETTID"); }
		set { values["RELEASETTID"] = value; }
	}
	public string WORKGITLOCATION
	{
		get { return GetVal("WORKGITLOCATION"); }
		set { values["WORKGITLOCATION"] = value; }
	}
	public string TESTREQUESTLINK
	{
		get { return GetVal("TESTREQUESTLINK"); }
		set { values["TESTREQUESTLINK"] = value; }
	}
	public string TEMPGIT
	{
		get { return GetVal("TEMPGIT"); }
		set { values["TEMPGIT"] = value; }
	}
	public string TELEGRAMTESTTOKEN
	{
		get { return GetVal("TELEGRAMTESTTOKEN"); }
		set { values["TELEGRAMTESTTOKEN"] = value; }
	}
	public string TELEGRAMTESTCHANNEL
	{
		get { return GetVal("TELEGRAMTESTCHANNEL"); }
		set { values["TELEGRAMTESTCHANNEL"] = value; }
	}
	public string TELEGRAMBUILDTOKEN
	{
		get { return GetVal("TELEGRAMBUILDTOKEN"); }
		set { values["TELEGRAMBUILDTOKEN"] = value; }
	}
	public string TELEGRAMBUILDCHANNEL
	{
		get { return GetVal("TELEGRAMBUILDCHANNEL"); }
		set { values["TELEGRAMBUILDCHANNEL"] = value; }
	}
	public string TESTSGIT
	{
		get { return GetVal("TESTSGIT"); }
		set { values["TESTSGIT"] = value; }
	}
	public string BACKGROUNDIMG
	{
		get { return GetVal("BACKGROUNDIMG"); }
		set { values["BACKGROUNDIMG"] = value; }
	}
	Dictionary<string, string> values = new Dictionary<string, string>();
	void LoadData()
	{
		foreach (DataRow dr in DBHelper.GetRows(string.Format("SELECT * FROM {0}", _Tabl)))
		{
			values[dr["NAME"].ToString()] = dr["VALUE"].ToString();
		}
	}
	public RawSettings(bool loaddata)
	{
		if (loaddata)
		{
			LoadData();
		}
	}
	public RawSettings()
	{
	}
	public RawSettings(RawSettings o)
	{
		values = new Dictionary<string, string>(o.values);
	}
	public void Store()
	{
		foreach (string key in values.Keys)
		{
			DBHelper.SQLExecute(string.Format("INSERT INTO {0} ([NAME], [VALUE]) SELECT '{1}', '{2}' WHERE NOT EXISTS (SELECT * FROM {0} WHERE NAME = '{1}')", _Tabl, key, values[key]));
			DBHelper.SQLExecute(string.Format("UPDATE {0} SET [VALUE]='{2}' WHERE [NAME] = '{1}'", _Tabl, key, values[key]));
		}
		CurrentRawSettings = null;
	}
}
public class Settings : RawSettings
{
	public Settings(bool loaddata) : base(loaddata)
	{
	}
	public Settings() : base()
	{
	}
	public Settings(RawSettings o) : base(o)
	{
	}
	protected override string ProcessValue(string val)
	{
		if (val.ToUpper().StartsWith("HTTPS://") && !HttpContext.Current.Request.IsSecureConnection)
		{
			return "http" + val.Substring(5);
		}
		return val;
	}
}