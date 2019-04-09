using System;
using System.Collections.Generic;

public class DefectHistory : IdBasedObject
{
	private const string _ID = "idRecord";
	private const string _Dat = "dateLog";
	private const string _idUser = "idUser";
	private const string _Notes = "Notes";
	private const string _ParentID = "ParentID";
	private const string _Proj = "PROJECTID";
	private const string _Tabl = "[TT_RES].[DBO].[DEFLOG]";
	private static string[] _allcols = new string[] { _ID, _Dat, _idUser, _Notes, _ParentID };

	public string NOTES
	{
		get { return this[_Notes].ToString(); }
		set { this[_Notes] = value; }
	}
	public string IDUSER
	{
		get { return this[_idUser].ToString(); }
		set { this[_idUser] = value; }
	}
	public string DATE
	{
		get { return Convert.ToDateTime(this[_Dat]).ToLocalTime().ToString(); }
		set { this[_Dat] = value; }
	}

	public DefectHistory()
		: base(_Tabl, _allcols, "0", _ID, false)
	{
	}
	public DefectHistory(int id)
		: base(	_Tabl, _allcols, id.ToString(), _ID)
	{
	}
	public static List<DefectHistory> GetHisotoryByTask(int ttid)
	{
		List<DefectHistory> res = new List<DefectHistory>();
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _ParentID }, new object[] { Defect.GetIDbyTT(ttid) }))
		{
			res.Add(new DefectHistory(i));
		}
		res.Sort(
				 delegate (DefectHistory h1, DefectHistory h2)
				 {
					 return Convert.ToDateTime(h1[_Dat]).CompareTo(Convert.ToDateTime(h2[_Dat]));
				 }
			);
		return res;
	}
	public static void AddHisotoryByTask(int id, string notes, string ttuserid = "")
	{
		notes = notes.Replace("'", "''");
		string sql = string.Format(@"
		INSERT INTO {0}
			({1}, {2}, {3}, {4}, {5}, {6})
		VALUES
		(
		(SELECT MAX(DLI.{1}) + 1 FROM {0} DLI WHERE DLI.{1} < 3000000)
						,1
						,{7}
						, GETUTCDATE()
						,'{8}'
						, {9}
		)", _Tabl, 
			_ID, _Proj, _idUser, _Dat, _Notes, _ParentID, string.IsNullOrEmpty(ttuserid) ? CurrentContext.User.TTUSERID.ToString() : ttuserid, notes, id);
		SQLExecute(sql);
	}
	public static void DelHisotoryByTask(int parentid)
	{
		string sql = string.Format(@"DELETE FROM {0} WHERE {1} = {2}", _Tabl, _ParentID, parentid);
		SQLExecute(sql);
	}
}