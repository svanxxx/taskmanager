using System;
using System.Web.UI;

public partial class Logout : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		CurrentContext.User = null;
		Response.Redirect(SiteSettings.GetAuthProvider(Request) + "/home/logout?returnURL=" + Uri.EscapeDataString(Request.Url.OriginalString.Replace("/logout.aspx", "")), false);
	}
}
