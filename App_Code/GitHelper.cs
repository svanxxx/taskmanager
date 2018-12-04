using System;
using System.Collections.Generic;
using System.Management.Automation;

public class GitHelper
{
	string _path = Settings.CurrentSettings.WORKGITLOCATION;
	public GitHelper(string path = "")
	{
		if (!string.IsNullOrEmpty(path))
		{
			_path = path;
		}
	}
	public List<string> ResetHard(string tobranch = "")
	{
		return RunCommand("reset --hard " + tobranch);
	}
	public List<string> Status()
	{
		return RunCommand("status");
	}
	public List<string> CleanaupLocalTags()
	{
		return RunCommand("fetch--prune origin \"+refs/tags/*:refs/tags/*\"");
	}
	public List<string> PullOrigin()
	{
		return RunCommand("pull origin");
	}
	public List<string> PushOrigin()
	{
		return RunCommand("push origin HEAD");
	}
	public List<string> PushTags()
	{
		return RunCommand("push --tags");
	}
	public List<string> Rebase(string barnch)
	{
		return RunCommand("rebase " + barnch);
	}
	public List<string> Checkout(string barnch)
	{
		return RunCommand("checkout " + barnch);
	}
	public List<string> RunCommand(string command)
	{
		using (PowerShell ps = PowerShell.Create())
		{
			ps.AddScript("cd " + _path);
			ps.AddScript(@"git.exe " + command);
			List<string> ls = new List<string>();
			foreach (var line in ps.Invoke())
			{
				ls.Add(line.ToString().Trim());
			}
			return ls;
		}
	}
	internal List<string> Diff()
	{
		return RunCommand("diff HEAD^ HEAD");
	}
}