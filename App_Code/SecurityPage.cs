using System;
using System.Linq;
using System.Web;

public class SecurityPage : System.Web.UI.Page
{
	public static string returl = "ReturnUrl";
	public static string loginpage = "login.aspx";


	void CheckRetired()
	{
		if (CurrentContext.Valid && CurrentContext.User.RETIRED)
		{
			Response.Redirect(string.Format("{0}?{1}=1", loginpage, CurrentContext.retiredURL), false);
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
			CheckRetired();
			return;
		}
		else
		{
			Response.Redirect(loginpage + "?" + returl + "=" + Request.Url.PathAndQuery, false);
			Context.ApplicationInstance.CompleteRequest();
		}
		CheckRetired();
		return;
	}
}