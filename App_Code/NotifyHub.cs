using Microsoft.AspNet.SignalR;

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
	public void RequestRoomUsers()
	{
		Clients.Caller.OnRoomChanged(Roommate.Enum());
	}
}