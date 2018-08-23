using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

public class Statistic
{
	public Statistic() { }
	public Statistic(DataRow dr)
	{
		TTUSER = dr["TTUSER"] == DBNull.Value ? -1 : Convert.ToInt32(dr["TTUSER"]);
		HOURS = dr["HOURS"] == DBNull.Value ? -1 : Convert.ToInt32(dr["HOURS"]);
		CNT = dr["CNT"] == DBNull.Value ? -1 : Convert.ToInt32(dr["CNT"]);
		FLAG = Convert.ToInt32(dr["FLAG"]);//1 - created, 2 - finished
	}
	public int TTUSER { get; set; }
	public int HOURS { get; set; }
	public int CNT { get; set; }
	public int FLAG { get; set; }//1 - created, 2 - finished, 3 - vacations unused, 4 - sick, 5 - vacations used
}
partial class Defect
{
	public static List<Statistic> EnumStatistics(DateTime start, int days)
	{
		List<Statistic> ls = new List<Statistic>();

		DateTime end = new DateTime(start.Year, start.Month, start.Day);
		end = end.AddDays(days);

		string vacs = string.Join(",", DefectComp.GetVacationRec());
		string vacsfree = string.Join(",", DefectDispo.EnumCannotStart());

		string sql = string.Format(@"
			SELECT D.{0} TTUSER, SUM(D.{2}) HOURS, COUNT(*) CNT, 1 FLAG FROM {1} D 
			WHERE D.{3} >= '{4}' AND D.{3} <= '{6}'
					AND D.IDCOMPON NOT IN ({7})
			GROUP BY {0}

			UNION ALL

			SELECT D.{0} TTUSER, COUNT(*)*8 HOURS, COUNT(*) CNT, 3 FLAG FROM {1} D 
			WHERE D.IDCOMPON IN ({7}) AND D.IDDISPOSIT IN ({9})
			GROUP BY {0}

			UNION ALL

			SELECT D.{0} TTUSER, COUNT(*)*8 HOURS, COUNT(*) CNT, 4 FLAG FROM {1} D 
			WHERE D.IDCOMPON IN ({7}) 
					AND D.IDDISPOSIT NOT IN ({9}) 
					AND UPPER(D.Summary) like '%SICK%'
					AND D.{5} >= '{4}' AND D.{5} <= '{6}'
			GROUP BY {0}

			UNION ALL

			SELECT D.{0} TTUSER, COUNT(*)*8 HOURS, COUNT(*) CNT, 5 FLAG FROM {1} D 
			WHERE D.IDCOMPON IN ({7}) 
					AND D.IDDISPOSIT NOT IN ({9}) 
					AND UPPER(D.Summary) NOT LIKE '%SICK%'
					AND D.{5} >= '{4}' AND D.{5} <= '{6}'
			GROUP BY {0}

			UNION ALL

			SELECT {0} TTUSER, SUM(D.{2}) HOURS, COUNT(*) CNT, 2 FLAG FROM {1} D 
			WHERE D.{5} >= '{4}' AND D.{5} <= '{6}'
					AND D.IDDISPOSIT IN (SELECT DI.IDRECORD FROM {8} DI WHERE DI.CANNOTSTART = 0 AND DI.REQUIREWORK = 0)
					AND D.IDCOMPON NOT IN ({7})
			GROUP BY {0}
		",
		_AsUser, _Tabl, _Est, _Created, start.ToString(defDateFormat, CultureInfo.InvariantCulture), _Date, end.ToString(defDateFormat, CultureInfo.InvariantCulture),
		vacs,
		DefectDispo._Tabl, vacsfree);

		foreach (DataRow d in DBHelper.GetRows(sql))
		{
			ls.Add(new Statistic(d));
		}
		return ls;
	}
}