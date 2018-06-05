using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class CurrentContext
{
	public static bool Valid
	{
		get
		{
			return User != null;
		}
	}
	public static MPSUser User
	{
		get
		{
			object o = HttpContext.Current.Session["userid"];
			if (o == null)
				return null;

			int userid = Convert.ToInt32(o);
			o = HttpContext.Current.Session["currentuser"];
			if (o == null)
			{
				o = new MPSUser(userid);
				HttpContext.Current.Session["currentuser"] = o;
			}
			return (o as MPSUser);
		}
	}
}