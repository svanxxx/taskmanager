using System;
using System.Collections.Generic;
using System.Globalization;

public class Vacation
{
	public Vacation() { }
	public int UserId { get; set;	}
	public int TTUserId { get; set; }
	public bool Sick { get; set; }
}
public static class Vacations
{
	static object _lock = new object();
	static DateTime _loadDT = DateTime.Now.AddMinutes(-6);
	static List<Vacation> _TodayVacs;
	public static List<Vacation> TodayVacations()
	{
		lock(_lock)
		{
			if (_TodayVacs == null || (DateTime.Now - _loadDT).TotalMinutes > 5)
			{
				_TodayVacs = new List<Vacation>();
				var tasks = EnumCloseVacations(DateTime.Now.ToString(IdBasedObject.defDateFormat, CultureInfo.InvariantCulture), 0);
				foreach (var t in tasks)
				{
					DefectUser u = new DefectUser(t.AUSER);
					_TodayVacs.Add(new Vacation() { UserId = u.TRID, Sick = t.SICK, TTUserId = u.ID });
				}
			}
			return new List<Vacation>(_TodayVacs);
		}
	}
	public static List<DefectBase> EnumCloseVacations(DateTime startdate, int days = 31)
	{
		return EnumCloseVacations(startdate.ToString(IdBasedObject.defDateFormat, CultureInfo.InvariantCulture), days);
	}
	public static List<DefectBase> EnumCloseVacations(string startdate, int days = 31)
	{
		DefectsFilter f = new DefectsFilter();
		f.components = new List<int>(DefectComp.GetVacationRec());
		f.dispositions = DefectDispo.EnumCanStartIDs();
		f.startDateEnter = startdate;
		f.endDateEnter = DateTime.ParseExact(startdate, IdBasedObject.defDateFormat, CultureInfo.InvariantCulture).AddDays(days).ToString(IdBasedObject.defDateFormat);//two weeks adnvance
		return (new DefectBase()).Enum(f, 2000);
	}
}