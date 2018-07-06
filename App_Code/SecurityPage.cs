using System;
using System.Linq;
using System.Web;

public class SecurityPage : System.Web.UI.Page
{
	public static string returl = "ReturnUrl";
	static string _ucook = "userid";
	static string _login = "login.aspx";

	protected void Page_PreInit(object sender, EventArgs e)
	{
		if (Request.Url.Segments.Last().ToUpper() == _login.ToUpper())
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
					return;
				}
			}
			HttpCookie cookie = Request.Cookies.Get(_ucook);
			if (cookie != null || !string.IsNullOrEmpty(cookie.Value))
			{
				int id = -1;
				if (int.TryParse(cookie.Value, out id))
				{
					CurrentContext.User = MPSUser.FindUserbyID(id);
					if (CurrentContext.Valid)
					{
						return;
					}
				}
			}
		}
		if (!CurrentContext.Valid)
		{
			Response.Redirect(_login + "?" + returl + "=" + Request.Url.PathAndQuery, false);
			Context.ApplicationInstance.CompleteRequest();
		}
		return;
	}
}