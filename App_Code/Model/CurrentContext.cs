using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Web;
public static class CurrentContext
{
	public static string ucook = "userid";
	public static string retiredURL = "retired";
	static string _id = "userid";
	static string _us = "currentuser";
	static string _authErr = "Unauthorized access.";
	public static bool Validate(HttpResponse Response = null)
	{
		if (!Valid)
		{
			if (Response != null)
			{
				Response.ContentType = "text/plain";
				Response.Write(_authErr);
				return false;
			}
			throw new Exception(_authErr);
		}
		return true;
	}
	public static void ValidateAdmin()
	{
		Validate();
		if (!Admin)
		{
			throw new Exception("Insufficient Rights");
		}
	}
	public static bool Valid
	{
		get
		{
			return User != null && !User.RETIRED;
		}
	}
	public static bool Admin
	{
		get
		{
			return User != null && User.ISADMIN;
		}
	}
	public static bool Client
	{
		get
		{
			return User != null && User.ISCLIENT;
		}
	}
	public static string TestFilter
	{
		get
		{
			return User == null ? "" : User.TESTFILTER;
		}
	}
	public static int UserID
	{
		get
		{
			return User != null ? User.ID : -1;
		}
	}
	public static int TTUSERID
	{
		get
		{
			return User != null ? User.TTUSERID : -1;
		}
	}
	public static string UserName()
	{
		if (!Valid)
		{
			return "";
		}
		return User.PERSON_NAME;
	}
	public static string UserLogin()
	{
		if (!Valid)
		{
			return "";
		}
		return User.LOGIN;
	}
	static object _lockobject = new object();
	static ConcurrentDictionary<int, MPSUser> _threadContext = new ConcurrentDictionary<int, MPSUser>();
	static void ThreadLogin(MPSUser u)
	{
		_threadContext[Thread.CurrentThread.ManagedThreadId] = u;
	}
	static void ThreadLogout()
	{
		int i = Thread.CurrentThread.ManagedThreadId;
		if (_threadContext.ContainsKey(i))
		{
			MPSUser u;
			_threadContext.TryRemove(i, out u);
		}
	}
	static MPSUser ThreadUser()
	{
		return _threadContext[Thread.CurrentThread.ManagedThreadId];
	}
	public static MPSUser User
	{
		set
		{
			lock (_lockobject)
			{
				if (value == null)
				{
					if (HttpContext.Current.Session != null)
					{
						HttpContext.Current.Session.Remove(_id);
						HttpContext.Current.Session.Remove(_us);
					} else
					{
						ThreadLogout();
					}
					return;
				}
				if (HttpContext.Current.Session != null)
				{
					HttpContext.Current.Session[_id] = value.ID;
					HttpContext.Current.Session[_us] = value;
				} 
				else
				{
					ThreadLogin(value);
				}
			}
		}
		get
		{
			if (HttpContext.Current.Session == null)
			{
				return ThreadUser();
			}
			object ous = HttpContext.Current.Session[_us];
			if (ous == null)
			{
				HttpRequest r = HttpContext.Current.Request;
				if (r != null && r.Params["susername"] != null && r.Params["suserpass"] != null)
				{
					ous = MPSUser.FindUser(r.Params["susername"].ToString(), r.Params["suserpass"].ToString());
				}
				else
				{
					HttpCookie cookie = r.Cookies.Get(ucook);
					if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
					{
						int id = -1;
						if (int.TryParse(cookie.Value, out id))
						{
							ous = MPSUser.FindUserbyID(id);
						}
					}
				}
				HttpContext.Current.Session[_us] = ous;
			}
			return (ous as MPSUser);
		}
	}
}