using System;
using System.Linq;
using System.Web;

public class SiteSettings
{
	public SiteSettings()
	{
	}
	public const string UserCookie = "4B11738612F7427990AAAD3FCD793872";
	public static string GetAuthProvider(HttpRequest Request)
	{
		var requestPage = Request.Url.OriginalString;
		var myUri = new Uri(requestPage);
		return "https://auth." + string.Join(".", myUri.Host.Split('.').Reverse().Take(2).Reverse());
	}
	public static string GetjwtKey()
	{
		return "6b18891351c343aa8e7cd06d3ec05117e2ea15958e51460a9c40de865b4bfb741faadebdfd6b45a5ac4fb5af1f43fa1e";
	}
}