using System;
using System.Collections.Generic;

public class DefectUser : IdBasedObject
{
	static string _ID = "idRecord";
	static string _Firn = "FirstName";
	static string _Lasn = "LastName";
	static string _Emai = "EMailAddr";
	static string _Atci = "Active";

	static string[] _Allcols = new string[] { _ID, _Firn, _Lasn, _Emai, _Atci };
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
}