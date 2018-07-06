using System;
using System.Web;

public static class CurrentContext
{
	static string _id = "userid";
	static string _us = "currentuser";
	public static bool Valid
	{
		get
		{
			return User != null;
		}
	}
	public static MPSUser User
	{
		set
		{
			if (value == null)
			{
				HttpContext.Current.Session.Remove(_id);
				HttpContext.Current.Session.Remove(_us);
				return;
			}
			HttpContext.Current.Session[_id] = value.ID;
			HttpContext.Current.Session[_us] = value;
		}
		get
		{
			//old version compatibility (stored by id only):
			object o = HttpContext.Current.Session[_id];
			if (o == null)
				return null;

			int userid = Convert.ToInt32(o);
			o = HttpContext.Current.Session[_us];
			if (o == null)
			{
				o = new MPSUser(userid);
				HttpContext.Current.Session[_us] = o;
			}
			return (o as MPSUser);
		}
	}
}