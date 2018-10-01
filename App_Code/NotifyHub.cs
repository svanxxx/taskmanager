using Microsoft.AspNet.SignalR;

public class NotifyHub : Hub
{
	public static void NotifyPlanChange(int userid)
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnPlanChanged(userid);
	}
	public void OnPlanChanged(int userid)
	{
		Clients.All.PlanChanged(userid);
	}
}