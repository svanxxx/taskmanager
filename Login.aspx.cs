using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Collections.Specialized;
using GTOHELPER;

public partial class Login : GTOHelper
{
	protected System.Int32 m_lUserID = -1;
	private bool SiteLevelCustomAuthenticationMethod(string strUserName, string strPassword)
	{
		m_lUserID = FindUser(strUserName, strPassword);
		if (m_lUserID == -1)
			return false;
		return true;
	}
	protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
	{
		bool Authenticated = false;
		Authenticated = SiteLevelCustomAuthenticationMethod(Login1.UserName, Login1.Password);
		e.Authenticated = Authenticated;
		if (Authenticated == true)
		{
			HttpCookie cookie = new HttpCookie("userid", m_lUserID.ToString());
			cookie.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Add(cookie);
			FormsAuthentication.RedirectFromLoginPage(Login1.UserName, true);
		}
	}
}
