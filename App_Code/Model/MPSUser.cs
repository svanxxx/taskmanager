using System;
using System.Web;

public class MPSUser : IdBasedObject
{
	private const string _pid = "PERSON_ID";
	private const string _pname = "PERSON_NAME";
	private const string _email = "WORK_EMAIL";
	private const string _ttuser = "ttuserid";
	private static string[] _allcols = new string[] { _pid, _pname, _email, _ttuser };
	private static string _Tabl = "[PERSONS]";

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
		get { return Convert.ToInt32(this[_ttuser]); }
		set { this[_ttuser] = value; }
	}

	protected override string OnTransformCol(string col)
	{
		if (col == _ttuser)
		{
			return string.Format("(SELECT IDRECORD FROM TT_RES.DBO.USERS WHERE UPPER(EMAILADDR) = (SELECT UPPER(WORK_EMAIL) FROM PERSONS WHERE PERSON_ID = {0})) {1}", _id, _ttuser);
		}
		return base.OnTransformCol(col);
	}
	public MPSUser(int id)
	  : base(_Tabl, _allcols, id.ToString(), _pid)
	{
	}
}