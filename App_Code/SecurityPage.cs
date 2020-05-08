using System;
using System.Linq;
using System.Web;

public class SecurityPage : System.Web.UI.Page
{
	public static string returl = "ReturnUrl";
	public static string loginpage = "login.aspx";
	public static readonly string[] _clientpages = { "tracker.aspx", "versionchanges.aspx" };

	void CheckPermissions()
	{
		if (CurrentContext.Valid && CurrentContext.User.RETIRED)
		{
			Response.Redirect(string.Format("{0}?{1}=1", loginpage, CurrentContext.retiredURL), false);
			Context.ApplicationInstance.CompleteRequest();
		}
		if (CurrentContext.Valid && CurrentContext.User.ISCLIENT && Array.FindIndex(_clientpages, x=>x.ToUpper()==Request.Url.Segments.Last().ToUpper()) < 0)
		{
			Response.Redirect(_clientpages[0], false);
			Context.ApplicationInstance.CompleteRequest();
		}
	}
	protected void Page_PreInit(object sender, EventArgs e)
	{
		if (Request.Url.Segments.Last().ToUpper() == loginpage.ToUpper())
		{
			return;
		}
		if (CurrentContext.Valid)
		{
			CheckPermissions();
			return;
		}
		else
		{
			Response.Redirect(loginpage + "?" + returl + "=" + Request.Url.PathAndQuery, false);
			Context.ApplicationInstance.CompleteRequest();
		}
		CheckPermissions();
		return;
	}
}