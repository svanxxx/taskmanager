using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OwinStartup))]

public class OwinStartup
{
	public void Configuration(IAppBuilder app)
	{
		app.MapSignalR();
	}
}
