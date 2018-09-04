using System;
using System.Collections.Concurrent;

public class DefectEventsHandler
{
	static ConcurrentDictionary<int, DateTime> _infoMap = new ConcurrentDictionary<int, DateTime>();
	public DefectEventsHandler()
	{
	}
	public static void DefectChanged(DefectBase d)
	{
		int id = int.Parse(d.AUSER);
		_infoMap[id] = DateTime.Now;
	}
	public static DateTime GetUserUpdateTime(int id)
	{
		return _infoMap[id];
	}
}