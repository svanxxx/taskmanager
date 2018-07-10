using System;
using System.Linq;
using System.Web;

public class SecurityPage : System.Web.UI.Page
{
	public static string returl = "ReturnUrl";
	public static string loginpage = "login.aspx";

	static string _ucook = "userid";

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

		if (!CurrentContext.Valid)
		{
			if (Request.Params["susername"] != null && Request.Params["suserpass"] != null)
			{
				CurrentContext.User = MPSUser.FindUser(Request.Params["susername"].ToString(), Request.Params["suserpass"].ToString());
				if (CurrentContext.Valid)
				{
					CheckRetired();
					return;
				}
			}
			HttpCookie cookie = Request.Cookies.Get(_ucook);
			if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
			{
				int id = -1;
				if (int.TryParse(cookie.Value, out id))
				{
					CurrentContext.User = MPSUser.FindUserbyID(id);
					if (CurrentContext.Valid)
					{
						CheckRetired();
						return;
					}
				}
			}
		}
		if (!CurrentContext.Valid)
		{
			Response.Redirect(loginpage + "?" + returl + "=" + Request.Url.PathAndQuery, false);
			Context.ApplicationInstance.CompleteRequest();
		}
		CheckRetired();
		return;
	}
}