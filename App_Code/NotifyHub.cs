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
	public void RequestRoomUsers()
	{
		Clients.Caller.OnRoomChanged(Roommate.Enum());
	}
}