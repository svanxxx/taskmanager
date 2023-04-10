using System;
using System.Linq;
using System.Security.Claims;
using System.Web;

public static class ContextExtensions
{
	public static string UserName(this HttpContext httpContext)
	{
		return ((ClaimsPrincipal)httpContext.User).UserName();
	}
	public static string UserName(this ClaimsPrincipal principal)
	{
		var DispName = principal.Claims.Where(x => x.Type == ClaimTypes.GivenName).Select(x => x.Value).FirstOrDefault();
		if (string.IsNullOrEmpty(DispName))
		{
			return principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).FirstOrDefault();
		}
		return DispName;
	}
	public static string User(this ClaimsPrincipal principal)
	{
		return principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).FirstOrDefault();
	}
	public static int UserId(this HttpContext httpContext)
	{
		return ((ClaimsPrincipal)httpContext.User).UserId();
	}
	public static int UserId(this ClaimsPrincipal principal)
	{
		var id = principal.Claims.Where(x => x.Type == ClaimTypes.Sid).Select(x => x.Value).FirstOrDefault();
		return Convert.ToInt32(id);
	}
	public static bool UserAdmin(this HttpContext httpContext)
	{
		return ((ClaimsPrincipal)httpContext.User).UserAdmin();
	}
	public static bool UserAdmin(this ClaimsPrincipal principal)
	{
		return principal.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value.ToUpper() == "ADMINISTRATORS");
	}
	public static bool UserClient(this HttpContext httpContext)
	{
		return ((ClaimsPrincipal)httpContext.User).UserClient();
	}
	public static bool UserClient(this ClaimsPrincipal principal)
	{
		return principal.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value.ToUpper() == "CLIENTS");
	}
	public static string UserPhone(this HttpContext httpContext)
	{
		return ((ClaimsPrincipal)httpContext.User).UserPhone();
	}
	public static string UserPhone(this ClaimsPrincipal principal)
	{
		var phoneVal = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone);
		if (phoneVal != null)
		{
			return phoneVal.Value;
		}
		return string.Empty;
	}
}
