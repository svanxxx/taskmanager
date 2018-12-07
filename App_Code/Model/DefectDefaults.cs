using System.Collections.Generic;
using System.Data;

public class DefectDefaults
{
	static readonly int _undef = -1;
	Dictionary<string, int> _ivalues = new Dictionary<string, int>();
	public DefectDefaults() { }
	public DefectDefaults(DefectDefaults copy)
	{
		foreach (string key in copy._ivalues.Keys)
		{
			_ivalues.Add(key, copy._ivalues[key]);
		}
	}
	static readonly string _Tabl = "[tt_res].[dbo].[FLDDFNTN]";
	static object _lockobject = new object();
	static DefectDefaults _DefectDefaults = null;
	public static DefectDefaults CurrentDefaults
	{
		set
		{
			lock (_lockobject)
			{
				_DefectDefaults = value;
			}
		}
		get
		{
			lock (_lockobject)
			{
				if (_DefectDefaults == null)
				{
					_DefectDefaults = LoadValues();
				}
				return new DefectDefaults(_DefectDefaults);
			}
		}
	}
	static DefectDefaults LoadValues()
	{
		DefectDefaults v = new DefectDefaults();
		foreach (var p in v.GetType().GetProperties())
		{
			if (p.CanWrite && p.PropertyType == typeof(int))
			{
				p.SetValue(v, _undef);
			}
		}
		string fields = "";
		foreach (var k in v._ivalues.Keys)
		{
			fields += string.Format(", '{0}'", k);
		}
		fields = fields.Remove(0, 1);
		string sql = string.Format("select FieldCode, DefaultVal from {0} where FieldCode in ({1})", _Tabl, fields);
		foreach (DataRow row in DBHelper.GetRows(sql))
		{
			v._ivalues[row[0].ToString()] = int.Parse(row[1].ToString());
		}
		return v;
	}
	public int PRODUCT
	{
		get { return _ivalues["PROD"]; }
		set { _ivalues["PROD"] = value; }
	}
}