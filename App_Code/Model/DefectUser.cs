using System;
using System.Collections.Generic;

public class DefectUser : IdBasedObject
{
	static string _ID = "idRecord";
	static string _Firn = "FirstName";
	static string _Lasn = "LastName";
	static string _Emai = "EMailAddr";
	static string _Atci = "Active";
	static string _trID = "TRID";

	static string[] _Allcols = new string[] { _ID, _Firn, _Lasn, _Emai, _Atci, _trID };
	static string _Tabl = "[TT_RES].[DBO].[USERS]";

	public int ID
	{
		get { return Convert.ToInt32(this[_ID]); }
		set { this[_ID] = value; }
	}
	public bool ACTIVE
	{
		get { return Convert.ToBoolean(this[_Atci]); }
		set { this[_Atci] = value; }
	}
	public string EMAIL
	{
		get { return this[_Emai].ToString(); }
		set { this[_Emai] = value; }
	}
	public string FIRSTNAME
	{
		get { return this[_Firn].ToString(); }
		set { this[_Firn] = value; }
	}
	public string LASTNAME
	{
		get { return this[_Lasn].ToString(); }
		set { this[_Lasn] = value; }
	}
	public string FULLNAME
	{
		get { return FIRSTNAME + " " + LASTNAME; }
		set { }
	}
	public int TRID
	{
		get { return (this[_trID] == DBNull.Value) ? -1 : Convert.ToInt32(this[_trID]); }
		set { this[_trID] = value; }
	}

	protected override string OnTransformCol(string col)
	{
		if (col == _trID)
		{
			return string.Format("(SELECT P.{0} FROM {1} P WHERE UPPER(P.{2}) = UPPER({3})) {4}", MPSUser._pid, MPSUser._Tabl, MPSUser._email, _Emai, _trID);
		}
		return base.OnTransformCol(col);
	}
	protected override void OnProcessComplexColumn(string col, object val)
	{
		if (col == _trID)
		{
			return;//nothing to do: readonly data
		}
		base.OnProcessComplexColumn(col, val);
	}

	public DefectUser()
		: base(_Tabl, _Allcols, 0.ToString(), _ID, false)
	{
	}
	public DefectUser(int id)
		: base(_Tabl, _Allcols, id.ToString(), _ID)
	{
	}
	public static List<DefectUser> Enum()
	{
		List<DefectUser> res = new List<DefectUser>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectUser(i));
		}
		return res;
	}
	public static DefectUser FindByEmail(string em)
	{
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _Emai }, new object[] { em.ToLower() }))
		{
			return new DefectUser(i);
		}
		return null;
	}
	public static void NewUser(string fname, string lname, string eml)
	{
		string sql = 
		$"insert into {_Tabl} (idRecord, ProjectID, Active, {_Firn}, idUsrGroup, {_Lasn}, EMailType, IsGlobal, IsCustomer, EMailAddr, AllowSSO, BetaSite, LoginName) " +
		$"values ((select max({_ID}) + 1 from {_Tabl}), 1, 1, '{fname}', 3, '{lname}', 1, 1, 0, '{eml}', 0, 0, 'default')";
		SQLExecute(sql);
	}
}