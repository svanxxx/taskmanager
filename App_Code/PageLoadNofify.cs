using System;
using System.Collections.Concurrent;

public class PageLoadNofify
{
	private static int _counter = 1;
	private static ConcurrentDictionary<int, bool> _loads = new ConcurrentDictionary<int, bool>();
	public static string NewLoad()
	{
		_counter++;
		AddLoad(_counter);
		return _counter.ToString();
	}
	public static void AddLoad(int id)
	{
		_loads.TryAdd(id, true);
	}
	public static void RemoveLoad(int id)
	{
		bool bout;
		_loads.TryRemove(id, out bout);
	}
	public static bool IsLoading(int id)
	{
		return _loads.Keys.Contains(id);
	}
}