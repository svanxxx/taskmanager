using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

public class TRStatistic
{
	public TRStatistic(DataRow dr)
	{
		IDUSER = dr["IDUSER"] == DBNull.Value ? -1 : Convert.ToInt32( dr["IDUSER"]);
		HOURS = dr["HOURS"] == DBNull.Value ? 0 : Convert.ToDouble(dr["HOURS"]);
	}
	public int IDUSER { get; set; }
	public double HOURS { get; set; }
}
public partial class TRRec
{
	public static List<TRStatistic> EnumTRStatistics(DateTime start, int days)
	{
		List<TRStatistic> ls = new List<TRStatistic>();

		DateTime end = new DateTime(start.Year, start.Month, start.Day);
		end = end.AddDays(days);
		string sql = string.Format(@"
			SELECT {0} IDUSER, SUM(DATEDIFF(MINUTE, {1}, {2}) / 60.0) HOURS FROM {3}
			WHERE {4} >= '{5}' AND {4} <= '{6}'
			GROUP BY {0}",
		_perid, _start, _end, _Tabl, _dat, start.ToString(defDateFormat, CultureInfo.InvariantCulture), end.ToString(defDateFormat, CultureInfo.InvariantCulture));
		
		foreach (DataRow d in DBHelper.GetRows(sql))
		{
			ls.Add(new TRStatistic(d));
		}
		return ls;
	}
}