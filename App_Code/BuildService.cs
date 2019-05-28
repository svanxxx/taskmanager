using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class BuildService : WebService
{
	public BuildService(){}
	[WebMethod(EnableSession = true)]
	public string getUpdateWorkGit()
	{
		if (!CurrentContext.Valid)
		{
			return "FAILED";
		}
		if (!CurrentContext.Admin)
		{
			return "FAILED - not admin.";
		}
		return VersionBuilder.PrepareGit();
	}
	[WebMethod(EnableSession = true)]
	public string versionIncrement()
	{
		if (!CurrentContext.Valid)
		{
			return "FAILED";
		}
		if (!CurrentContext.Admin)
		{
			return "FAILED - not admin.";
		}
		return VersionBuilder.VersionIncrement();
	}
	[WebMethod(EnableSession = true)]
	public string push2Master()
	{
		if (!CurrentContext.Valid)
		{
			return "FAILED";
		}
		if (!CurrentContext.Admin)
		{
			return "FAILED - not admin.";
		}
		string res = VersionBuilder.PushRelease();
		VersionBuilder.SendVersionAlarm();
		return res + "<br/>Finished!";
	}
	[WebMethod(EnableSession = true)]
	public int? GetBuilderID()
	{
		return VersionBuilder.GetLock();
	}
}