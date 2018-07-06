using System;
using System.Collections.Generic;
using System.Web;

public class MPSUser : IdBasedObject
{
	const string _pid = "PERSON_ID";
	const string _pname = "PERSON_NAME";
	const string _email = "WORK_EMAIL";
	const string _work = "IN_WORK";
	const string _ttuser = "ttuserid";
	const string _addr = "ADDRESS";
	const string _login = "PERSON_LOGIN";
	const string _pass = "USER_PASSWORD";
	const string _isAdm = "IS_ADMIN";
	const string _phone = "PERSON_PHONE";

	static string[] _allcols = new string[] { _pid, _pname, _email, _ttuser, _addr, _login, _pass, _isAdm, _phone, _work };
	static string _Tabl = "[PERSONS]";

	public string PHONE
	{
		get { return this[_phone].ToString(); }
		set { this[_phone] = value; }
	}
	public bool ISADMIN
	{
		get { return Convert.ToBoolean(this[_isAdm]); }
		set { this[_isAdm] = value; }
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

	protected override string OnTransformCol(string col)
	{
		if (col == _ttuser)
		{
			return string.Format("(SELECT TOP 1 IDRECORD FROM TT_RES.DBO.USERS WHERE UPPER(EMAILADDR) = (SELECT UPPER(WORK_EMAIL) FROM PERSONS WHERE PERSON_ID = {0})) {1}", _id, _ttuser);
		}
		return base.OnTransformCol(col);
	}
	public MPSUser(int id)
	  : base(_Tabl, _allcols, id.ToString(), _pid)
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
	public static MPSUser FindUser(string name, string pass)
	{
		foreach (int i in EnumRecords(_Tabl, _pid, new string[]{ _login, _pass}, new object[] { name, pass }))
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
}