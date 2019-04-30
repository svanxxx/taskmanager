using System;
using System.Collections.Generic;
using System.IO;

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
		byte[] filedata4Disk = filedata;
		int filesize = filedata.Length;
		if (filedata.Length > 1024 * 1024 * 5)
		{
			filedata = new byte[0];
		}
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
		int attkey = DefectAttach.AddObject(
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
													filesize,
													string.Format("{0}.DAT", key),
													0,
													1
												},
												"IDRECORD"
												);

		if (filedata.Length < 1)
		{
			if (!Directory.Exists(Settings.CurrentSettings.DEFECTATTACHDIR))
			{
				Directory.CreateDirectory(Settings.CurrentSettings.DEFECTATTACHDIR);
			}
			DefectAttach da = new DefectAttach(attkey);
			File.WriteAllBytes(da.FileOnDisk, filedata4Disk);
		}
		DefectHistory.AddHisotoryByTask(taksid, string.Format("Added attachment: {0}", filename));
	}
	public static void DeleteAttach(string ttid, int id)
	{
		DefectAttach a = new DefectAttach(id);
		int taksid = Defect.GetIDbyTT(Convert.ToInt32(ttid));
		DefectHistory.AddHisotoryByTask(taksid, string.Format("Deleted attachment: {0}", a.FILENAME));
		string fname = a.FileOnDisk;
		if (File.Exists(fname))
		{
			File.Delete(fname);
		}
		DeleteObject(_Tabl, id.ToString(), _ID);
	}
	public string FileOnDisk
	{
		get
		{
			return Settings.CurrentSettings.DEFECTATTACHDIR + ARCHIVE;
		}
	}
	public bool IsFileOnDisk
	{
		get
		{
			return File.Exists(FileOnDisk);
		}
	}

	public byte[] FileBinary()
	{
		byte[] bytes = (byte[])GetRecdata(_FileTabl, "filedata", _File, string.Format("'{0}'", ARCHIVE));
		if (bytes == null || bytes.Length < 1) //stored not id db - go to folder
		{
			if (File.Exists(FileOnDisk))
			{
				bytes = File.ReadAllBytes(FileOnDisk);
			}
		}
		return bytes;
	}
}