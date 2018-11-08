using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;

public class NotifyHub : Hub
{
	public static void NotifyPlanChange(int userid)
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnPlanChanged(userid);
	}
	public static void NotifyRoomChange()
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnRoomChanged(Roommate.Enum());
	}
	public static void NotifyBuildChange(int id, int ttid, int userid, string message)
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnBuildChanged(id, ttid, userid, message);
	}
	public static void NotifyBuildStatusChange(int id, int ttid, int userid, string message)
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnBuildStatusChanged(id, ttid, userid, message);
	}
	public void RequestRoomUsers()
	{
		Clients.Caller.OnRoomChanged(Roommate.Enum());
	}
	static ConcurrentDictionary<string, string> _registry = new ConcurrentDictionary<string, string>();
	public void RegisterMessenger(string userid)
	{
		_registry[userid] = Context.ConnectionId;
	}
	public void SendMessage(int fromID, int toID, string message)
	{
		if (!_registry.ContainsKey(toID.ToString()))
		{
			return;
		}
		string id = _registry[toID.ToString()];
		if (Clients.Client(id) != null)
		{
			Clients.Client(id).OnMessage(fromID, message);
		}
	}
	public void LockTask(int ttid, string currentlock, int userid)
	{
		LockInfo li = Defect.Locktask(ttid.ToString(), currentlock, userid.ToString());
		Clients.Caller.OnLockTask(li);
	}
	public void UnLockTask(int ttid, string currentlock)
	{
		Defect.UnLocktask(ttid.ToString(), currentlock);
	}
}