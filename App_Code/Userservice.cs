using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Userservice : WebService
{
	public Userservice() { }
	[WebMethod(EnableSession = true)]
	public MPSUser newUser()
	{
		CurrentContext.ValidateAdmin();
		return MPSUser.NewUser();
	}
	[WebMethod(EnableSession = true)]
	public DisplayUser getUser(int id)
	{
		CurrentContext.Validate();
		if (id < 1)
		{
			return new DisplayUser();
		}
		DefectUser u = new DefectUser(id);
		return new DisplayUser() { FULLNAME = u.FULLNAME, TRID = u.TRID };
	}
}