using System;
using System.Collections.Generic;

public class DefectAttach : IdBasedObject
{
	private static string _ID = "idRecord";
	private static string _Proj = "ProjectID";
	private static string _Entit = "EntityType";
	private static string _AttType = "AttType";
	private static string _Rid = "EntityRID";
	private static string _Name = "FileName";
	private static string _Mac = "MacType";
	private static string _MacCr = "MacCreator";
	private static string _DateC = "dateCreate";
	private static string _DateM = "dateModify";
	private static string _Size = "FileSize";
	private static string _File = "ArchvFile";
	private static string _Compr = "Compressed";
	private static string _InDB = "AttachInDB";
	private static string _BinaryTransfer = "BINDATA";

	private static string[] _allCols = new string[] { _ID, _Proj, _Entit, _AttType, _Rid, _Name, _Mac, _MacCr, _DateC, _DateM, _Size, _File, _Compr, _InDB };
	private static string _Tabl = "[tt_res].[dbo].[ATTACHMT]";
	private static string _FileTabl = "[tt_res].[dbo].[ATTARCHIVE]";

	public int ID
	{
		get { return Convert.ToInt32(this[_ID]); }
		set { this[_ID] = value; }
	}
	public string FILENAME
	{
		get { return this[_Name].ToString(); }
		set { this[_Name] = value; }
	}
	public string ARCHIVE
	{
		get { return this[_File].ToString(); }
		set { this[_File] = value; }
	}
	public string DATE
	{
		get { return Convert.ToDateTime(this[_DateC]).ToLocalTime().ToString(); }
		set { this[_DateC] = value; }
	}
	public int SIZE
	{
		get { return Convert.ToInt32(this[_Size]); }
		set { this[_Size] = value; }
	}
	public string BINARYTRANSFER
	{
		get;
		set;
	}

	public DefectAttach()
		: base(_Tabl, _allCols, 0.ToString(), _ID, false)
	{
	}
	public DefectAttach(int id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public static List<DefectAttach> GetAttachsByTask(int ttid)
	{
		List<DefectAttach> res = new List<DefectAttach>();
		int taksid = Defect.GetIDbyTT(ttid);
		int repid = Defect.GetRepRecByTTID(taksid);
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { _Rid }, new object[] { repid }))
		{
			res.Add(new DefectAttach(i));
		}
		return res;
	}
	public static void AddAttachByTask(int ttid, string filename, string data)
	{
		int taksid = Defect.GetIDbyTT(ttid);
		int repid = Defect.GetRepRecByTTID(taksid);
		byte[] filedata = Convert.FromBase64String(data);
		int key = DefectAttach.AddObject(
												_FileTabl,
												new string[] { "idRecord", "ProjectID", "ArchvFile", "FileData" },
												new object[] {
													new Expression(string.Format("(SELECT MAX(IDRECORD) + 1 FROM {0} where IDRECORD < 3000000)", _FileTabl)),
													1,
													new Expression(string.Format("(SELECT CONVERT(VARCHAR(10), MAX(IDRECORD) + 1) + '.DAT' FROM {0}  where IDRECORD < 3000000)", _FileTabl)),
													filedata
												},
												"IDRECORD"
												);
		DefectAttach.AddObject(
												_Tabl,
												new string[] { _ID, _Proj, _Entit, _AttType, _Rid, _Name, _Mac, _MacCr, _DateC, _Size, _File, _Compr, _InDB },
												new object[] {
													new Expression(string.Format("(SELECT MAX(IDRECORD) + 1 FROM {0} where IDRECORD < 3000000)", _Tabl)),
													1,
													1684431732,
													1,
													repid,
													filename,
													1061109567,
													1061109567,
													new Expression("GETUTCDATE()"),
													filedata.Length,
													string.Format("{0}.DAT", key),
													0,
													1
												},
												"IDRECORD"
												);
		DefectHistory.AddHisotoryByTask(taksid, string.Format("Added attachment: {0}", filename));
	}
	public static void DeleteAttach(string ttid, int id)
	{
		DefectAttach a = new DefectAttach(id);
		int taksid = Defect.GetIDbyTT(Convert.ToInt32(ttid));
		DefectHistory.AddHisotoryByTask(taksid, string.Format("Deleted attachment: {0}", a.FILENAME));
		DeleteObject(_Tabl, id.ToString(), _ID);
	}
	public byte[] FileBinary()
	{
		return (byte[])GetRecdata(_FileTabl, "filedata", _File, string.Format("'{0}'", ARCHIVE));
	}
}