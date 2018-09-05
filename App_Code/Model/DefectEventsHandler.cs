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
		int id;
		if (!int.TryParse(d.AUSER, out id))
			return;
		DefectUser u = new DefectUser(id);
		_infoMap[u.TRID] = DateTime.Now;
	}
	public static DateTime GetUserUpdateTime(int id)
	{
		if (!_infoMap.ContainsKey(id))
		{
			_infoMap[id] = DateTime.Now;
		}
		return _infoMap[id];
	}
}