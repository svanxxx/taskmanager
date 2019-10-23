using System;
using System.Linq;
using System.Web;

public class SecurityPage : System.Web.UI.Page
{
	public static string returl = "ReturnUrl";
	public static string loginpage = "login.aspx";
	public static string clientpage = "tracker.aspx";

	void CheckPermissions()
	{
		if (CurrentContext.Valid && CurrentContext.User.RETIRED)
		{
			Response.Redirect(string.Format("{0}?{1}=1", loginpage, CurrentContext.retiredURL), false);
			Context.ApplicationInstance.CompleteRequest();
		}
		if (CurrentContext.Valid && CurrentContext.User.ISCLIENT && Request.Url.Segments.Last().ToUpper() != clientpage.ToUpper())
		{
			Response.Redirect(clientpage, false);
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