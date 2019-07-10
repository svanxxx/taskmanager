using System.Collections.Generic;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class BuildService : WebService
{
	public BuildService() { }
	[WebMethod(EnableSession = true)]
	public string getUpdateWorkGit()
	{
		CurrentContext.ValidateAdmin();
		return VersionBuilder.PrepareGit();
	}
	[WebMethod(EnableSession = true)]
	public string getUpdateMergeGit(string branch)
	{
		CurrentContext.ValidateAdmin();
		return VersionMerger.PrepareGit(branch);
	}
	[WebMethod(EnableSession = true)]
	public string rebaseBranch(string branch)
	{
		CurrentContext.ValidateAdmin();
		return VersionMerger.MergeCode(branch);
	}
	[WebMethod(EnableSession = true)]
	public string versionIncrement()
	{
		CurrentContext.ValidateAdmin();
		return VersionBuilder.VersionIncrement();
	}
	[WebMethod(EnableSession = true)]
	public string push2Master()
	{
		CurrentContext.ValidateAdmin();
		string res = VersionBuilder.PushRelease();
		VersionBuilder.SendVersionAlarm();
		return res + "<br/>Finished!";
	}
	[WebMethod(EnableSession = true)]
	public string pushMerger2Master(string ttid)
	{
		CurrentContext.ValidateAdmin();
		string res = VersionMerger.Push(ttid);
		return res + "<br/>Finished!";
	}
	[WebMethod(EnableSession = true)]
	public int? GetBuilderID()
	{
		CurrentContext.ValidateAdmin();
		return VersionBuilder.GetLock();
	}
	[WebMethod(EnableSession = true)]
	public int? GetMergerID()
	{
		CurrentContext.ValidateAdmin();
		return VersionMerger.GetLock();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBuild> getBuildsByTask(int from, int to, string ttid)
	{
		CurrentContext.Validate();
		if (string.IsNullOrEmpty(ttid))
			return new List<DefectBuild>();
		return DefectBuild.EnumData(from, to, int.Parse(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBuild> getBuildRequests(int from, int to)
	{
		CurrentContext.Validate();
		return DefectBuild.EnumData(from, to);
	}
	[WebMethod(EnableSession = true)]
	public void alertVersion()
	{
		CurrentContext.ValidateAdmin();
		VersionBuilder.SendVersionAlarm();
	}
}