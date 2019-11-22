using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.DirectoryServices.AccountManagement;
using System.Linq;

public class MPSUser : IdBasedObject
{
	public const string _pid = "PERSON_ID";
	const string _pname = "PERSON_NAME";
	public const string _email = "WORK_EMAIL";
	public const string _work = "IN_WORK";
	const string _ttuser = "ttuserid";
	const string _addr = "ADDRESS";
	const string _login = "PERSON_LOGIN";
	const string _pass = "USER_PASSWORD";
	const string _isAdm = "IS_ADMIN";
	const string _phone = "PERSON_PHONE";
	const string _ret = "RETIRED";
	const string _img = "IMAGE";
	const string _lvl = "LEVEL_ID";
	const string _imgTransfer = "IMAGETRANSFER";
	const string _birth = "PERSON_BIRTHDAY";
	const string _chat = "CHATID";
	const string _schat = "SUPPCHATID";
	const string _sclientchat = "SUPPCLIENTCHATID";
	const string _cli = "CLIENT";

	static string[] _allcols = new string[] { _pid, _pname, _email, _ttuser, _addr, _login, _pass, _isAdm, _phone, _work, _ret, _imgTransfer, _birth, _lvl, _chat, _schat, _sclientchat, _cli };
	public static string _Tabl = "[PERSONS]";

	public string CHATID
	{
		get { return this[_chat].ToString(); }
		set { this[_chat] = value; }
	}
	public string SUPCHATID
	{
		get { return this[_schat].ToString(); }
		set { this[_schat] = value; }
	}
	public string SUPCHATCLIENTID
	{
		get { return this[_sclientchat].ToString(); }
		set { this[_sclientchat] = value; }
	}
	public string PHONE
	{
		get { return this[_phone].ToString(); }
		set { this[_phone] = value; }
	}
	public bool ISCLIENT
	{
		get { return GetAsBool(_cli, false); }
		set { this[_cli] = value; }
	}
	public bool ISADMIN
	{
		get { return Convert.ToBoolean(this[_isAdm]); }
		set { this[_isAdm] = value; }
	}
	public bool RETIRED
	{
		get { return GetAsBool(_ret); }
		set { this[_ret] = value; }
	}
	public string LOGIN
	{
		get { return this[_login].ToString(); }
		set { this[_login] = value; }
	}
	public string PASSWORD
	{
		get { return this[_pass].ToString(); }
		set { this[_pass] = value; }
	}
	public string ADDRESS
	{
		get { return this[_addr].ToString(); }
		set { this[_addr] = value; }
	}
	public bool INWORK
	{
		get
		{
			if (this[_work] == DBNull.Value)
				return false;

			return Convert.ToBoolean(this[_work]);
		}
		set { this[_work] = value; }
	}
	public int ID
	{
		get { return Convert.ToInt32(this[_fldid]); }
		set { this[_fldid] = value; }
	}
	public string EMAIL
	{
		get { return this[_email].ToString(); }
		set { this[_email] = value; }
	}
	public string PERSON_NAME
	{
		get { return this[_pname].ToString(); }
		set { this[_pname] = value; }
	}
	public string BIRTHDAY
	{
		get { return GetAsDate(_birth); }
		set { SetAsDate(_birth, value); }
	}
	public int TTUSERID
	{
		get
		{
			if (this[_ttuser] == DBNull.Value)
				return -1;

			return Convert.ToInt32(this[_ttuser]);
		}
		set { this[_ttuser] = value; }
	}
	public string IMGTRANSFER
	{
		get { return this[_imgTransfer].ToString(); }
		set { this[_imgTransfer] = value; }
	}

	protected override string OnTransformCol(string col)
	{
		if (col == _ttuser)
		{
			return string.Format("(SELECT TOP 1 IDRECORD FROM TT_RES.DBO.USERS WHERE UPPER(EMAILADDR) = (SELECT UPPER(WORK_EMAIL) FROM PERSONS WHERE PERSON_ID = {0})) {1}", _id, _ttuser);
		}
		if (col == _imgTransfer)
		{
			return "'' " + _imgTransfer;
		}
		return base.OnTransformCol(col);
	}
	protected override void OnProcessComplexColumn(string col, object val)
	{
		if (col == _imgTransfer)
		{
			string sval = val.ToString();
			if (!string.IsNullOrEmpty(sval))
			{
				sval = sval.Remove(0, sval.IndexOf("base64,") + 7);
				byte[] filedata = Convert.FromBase64String(sval);
				OleDbParameter p = new OleDbParameter("@" + _img, OleDbType.VarBinary);
				p.Value = filedata;
				List<OleDbParameter> pars = new List<OleDbParameter>();
				pars.Add(p);
				DBHelper.SQLExecute(string.Format("UPDATE {0} SET [{1}] = ? WHERE {2} = {3}", _Tabl, _img, _pid, ID), pars.ToArray());
			}
			return;
		}
		else if (col == _ttuser)
		{
			return;//nothing to do: readonly data
		}
		base.OnProcessComplexColumn(col, val);
	}
	protected override void PostStore()
	{
		ReferenceVersion.Updatekey();
	}
	protected override void OnChangeColumn(string col, string val)
	{
		if ((col == _pname || col == _cli) && TTUSERID > -1)
		{
			DefectUser du = new DefectUser(TTUSERID);
			if (col == _pname)
			{
				string[] vals = val.Replace("\'", "").Split(' ');
				if (vals.Length == 2)
				{
					du.FIRSTNAME = vals[0];
					du.LASTNAME = vals[1];
				}
				else
				{
					du.FIRSTNAME = val;
				}
			}
			else if (col == _cli)
			{
				du.CUSTOMER = (val != "0");
			}
			du.Store();
		}
		else if (col == _ret && TTUSERID > -1)
		{
			DefectUser du = new DefectUser(TTUSERID);
			du.ACTIVE = !RETIRED;
			du.Store();
		}
		base.OnChangeColumn(col, val);
	}
	public byte[] GetImage()
	{
		return DBHelper.GetValue(string.Format("SELECT [{0}] FROM {1} WHERE {2} = {3}", _img, _Tabl, _pid, ID)) as byte[];
	}
	public MPSUser(int id)
	  : base(_Tabl, _allcols, id.ToString(), _pid)
	{
	}
	public MPSUser(string id)
	  : base(_Tabl, _allcols, id, _pid)
	{
	}
	public MPSUser()
	  : base(_Tabl, _allcols, 0.ToString(), _pid, false)
	{
	}
	public static List<MPSUser> EnumAllUsers(bool active)
	{
		List<MPSUser> ls = new List<MPSUser>();
		List<string> fields = new List<string>();
		List<object> values = new List<object>();
		if (active)
		{
			fields.Add(_work);
			values.Add(true);
		}
		foreach (int i in EnumRecords(_Tabl, _pid, fields.ToArray(), values.ToArray()))
		{
			ls.Add(new MPSUser(i));
		}
		return ls;
	}
	public static List<MPSUser> EnumAllSupporters()
	{
		return new List<MPSUser>(EnumAllUsers(true).Where(item => !string.IsNullOrEmpty(item.SUPCHATID)));
	}
	public static MPSUser FindUser(string name, string pass)
	{
		bool domain = name.Contains("@");
		if (domain)
		{
			bool valid = false;
			string dispUserName = name;
			using (PrincipalContext context = new PrincipalContext(ContextType.Domain, Settings.CurrentSettings.COMPANYDOMAIN))
			{
				valid = context.ValidateCredentials(name, pass);
				if (valid)
				{
					var usr = UserPrincipal.FindByIdentity(context, name);
					if (usr != null)
						dispUserName = usr.GivenName + " " + usr.Surname;
				}
			}
			if (!valid)
			{
				return null;
			}
			foreach (int i in EnumRecords(_Tabl, _pid, new string[] { _login }, new object[] { name }))
			{
				return new MPSUser(i);
			}
			AddObject(_Tabl, new string[] { _login, _pname, _pass, _isAdm, _ret, _birth, _lvl, _email }, new object[] { name, dispUserName, "", 0, 0, DateTime.Now, 3, name }, "");
			DefectUser.NewUser(name, "", name);
			ReferenceVersion.Updatekey();
			foreach (int i in EnumRecords(_Tabl, _pid, new string[] { _login }, new object[] { name }))
			{
				return new MPSUser(i);
			}
			return null;
		}
		foreach (int i in EnumRecords(_Tabl, _pid, new string[] { _login, _pass }, new object[] { name, pass }))
		{
			return new MPSUser(i);
		}
		return null;
	}
	public static MPSUser FindUserbyID(int id)
	{
		foreach (int i in EnumRecords(_Tabl, _pid, new string[] { _pid }, new object[] { id }))
		{
			return new MPSUser(i);
		}
		return null;
	}
	public static MPSUser FindUserbyPhone(string num)
	{
		foreach (int i in EnumRecords(_Tabl, _pid, new string[] { _phone }, new object[] { num }))
		{
			return new MPSUser(i);
		}
		return null;
	}
}
public class Roommate
{
	public enum userstatus
	{
		online = 1,
		offline = 2,
		sick = 3,
		vacation = 4
	}
	public Roommate() { }
	public Roommate(DataRow r)
	{
		ID = Convert.ToInt32(r[0]);
		STATUS = r[1] == DBNull.Value ? (int)userstatus.offline : (int)userstatus.online;
	}
	public int ID { get; set; }
	public int STATUS { get; set; }
	static string _sql = string.Format(@"
			SELECT
			P.{0}
			,R.{4}
			FROM
			{2} P
			LEFT JOIN (SELECT RIN.{5}, RIN.{4} FROM {3} RIN WHERE RIN.{4} = CAST(GETDATE() AS DATE)) R ON R.{0} = P.{0}
			WHERE
			P.{1} = 1
		", MPSUser._pid, MPSUser._work, MPSUser._Tabl, TRRecSignal._Tabl, TRRecSignal._dat, TRRecSignal._perid);
	public static List<Roommate> Enum()
	{
		List<Roommate> ls = new List<Roommate>();
		foreach (DataRow r in DBHelper.GetRows(_sql))
		{
			Roommate d = new Roommate(r);
			ls.Add(d);
		}
		return ls;
	}

}