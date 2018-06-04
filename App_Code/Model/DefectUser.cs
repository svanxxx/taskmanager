using System;
using System.Collections.Generic;

public class DefectUser : IdBasedObject
{
	private const string _ID = "idRecord";
	private const string _Firn = "FirstName";
	private const string _Lasn = "LastName";
	private const string _Emai = "EMailAddr";
	private const string _Tabl = "[TT_RES].[DBO].[USERS]";
	public string ID
	{
		get { return this[_ID].ToString(); }
		set { this[_ID] = value; }
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
		: base(_Tabl, new string[] { _ID }, 1.ToString(), _ID, false)
	{
	}
	public DefectUser(int id)
		: base(	_Tabl, new string[] {_ID, _Firn, _Lasn, _Emai }, id.ToString(), _ID)
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
}