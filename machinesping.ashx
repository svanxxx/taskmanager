<%@ WebHandler Language="C#" Class="machinesping" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

public class machinesping : IHttpHandler
{
	object _lockobject = new object();
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/event-stream";
		context.Response.CacheControl = "no-cache";
		context.Response.Flush();
		List<Machine> mls = Machine.Enum();
		while (context.Response.IsClientConnected)
		{
			Parallel.ForEach(mls, (machine) =>
			{
				Ping ping = new Ping();
				IPStatus status = IPStatus.Unknown;
				try
				{
					status = ping.Send(machine.NAME).Status;
				}
				catch (Exception /*e*/)
				{

				}
				lock (_lockobject)
				{
					context.Response.Write(string.Format("event: machine\ndata: {0}\n\n", machine.NAME + "-" + status.ToString()));
					context.Response.Flush();
				}
			});
			System.Threading.Thread.Sleep(10000);
		}
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

}