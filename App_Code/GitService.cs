using GitHelper;
using System.Collections.Generic;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class GitService : WebService
{
	public GitService() { }
	[WebMethod(EnableSession = true)]
	public List<Commit> EnumCommits(string branch, int from, int to)
	{
		CurrentContext.Validate();
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.GetBranch(branch).EnumCommits(from, to);
	}
	[WebMethod(EnableSession = true)]
	public string BranchHash(string branch)
	{
		CurrentContext.Validate();
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.GetBranch(branch).TopCommit();
	}
	[WebMethod(EnableSession = true)]
	public List<string> getCommitDiff(string commit)
	{
		CurrentContext.Validate();
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		Commit c = new Commit(git);
		c.COMMIT = commit;
		return Git.DiffFriendOutput(c.Diff());
	}
	[WebMethod(EnableSession = true)]
	public void deleteBranch(string branch)
	{
		CurrentContext.Validate();
		if (string.IsNullOrEmpty(branch) || branch.ToUpper() == "MASTER" || branch.ToUpper() == "RELEASE")
			return;
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		git.DeleteBranch(branch);
	}
	[WebMethod(EnableSession = true)]
	public List<Branch> enumbranches(int from, int to, string user)
	{
		CurrentContext.Validate();
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.EnumBranches(from, to, user);
	}
	[WebMethod(EnableSession = true)]
	public List<string> getVersionLog()
	{
		CurrentContext.Validate();
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.RunCommand(@"show HEAD:""Projects.32/ChangeLog.txt""");
	}
	[WebMethod(EnableSession = true)]
	public List<Commit> QueryCommits(string pattern)
	{
		CurrentContext.Validate();
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		Branch b = new Branch(git);
		return b.QueryCommits(pattern);
	}
}