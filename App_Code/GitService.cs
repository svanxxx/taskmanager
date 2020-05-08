using GitHelper;
using System;
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
	static object _lockTodayCommits = new object();
	static List<Commit> _TodayCommits = null;
	static DateTime _dtOut = DateTime.Now.AddDays(-1);
	[WebMethod(EnableSession = true, CacheDuration = 60)]
	public List<Commit> TodayCommits()
	{
		CurrentContext.Validate();

		lock (_lockTodayCommits)
		{
			DateTime now = DateTime.Now;
			if ((now - _dtOut).TotalSeconds > 60)
			{
				Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
				Branch b = new Branch(git);
				_TodayCommits = b.TodayCommits();
				foreach (var c in _TodayCommits)
				{
					c.TTSUMMARY = Defect.GetTaskDispName(c.TTID);
				}
				_dtOut = now;
			}
		}

		return _TodayCommits;
	}
	[WebMethod(EnableSession = true, CacheDuration = 300)]
	public GitTag LastTag()
	{
		CurrentContext.Validate();

		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.LastTag();
	}
}