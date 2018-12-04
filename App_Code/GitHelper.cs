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
	public void SetCredentials(string user, string email)
	{
		RunCommand(string.Format("config --local user.name \"{0}\"", user));
		RunCommand(string.Format("config --local user.email \"{0}\"", email));
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
	public List<string> Diff()
	{
		return RunCommand("diff HEAD^ HEAD");
	}
	public List<string> DiffFriendOutput()
	{
		List<string> res = Diff();
		List<string> processed = new List<string>();
		for (int i = 0; i < res.Count; i++)
		{
			string s = res[i];
			string color = "white";
			string pre = "";
			string pos = "";
			if (s.StartsWith("+"))
			{
				color = "#00800047";
			}
			else if (s.StartsWith("-"))
			{
				color = "#ff000036";
			}
			else if (s.StartsWith("fatal"))
			{
				color = "red";
			}
			else if (s.StartsWith("+++ b"))
			{
				continue;
			}
			else if (s.StartsWith("--- a"))
			{
				continue;
			}
			else if (s.StartsWith("diff --git a"))
			{
				s = "<hr>" + s.Replace("diff --git a", "").Split(new string[] { "b/" }, StringSplitOptions.RemoveEmptyEntries)[0];
				pre = "<b>";
				pos = "</b>";
			}
			processed.Add(string.Format("<div style='background-color:{0};'>{2}{1}{3}</div>", color, s, pre, pos));
		}
		return processed;
	}
}