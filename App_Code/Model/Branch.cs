using System;
using System.Collections.Generic;

public class Branch
{
	public string DATE { get; set; }
	public string NAME { get; set; }
	public string AUTHOR { get; set; }
	public string AUTHOREML { get; set; }

	public Branch()
	{
	}
	public static List<Branch> Enum()
	{
		List<Branch> ls = new List<Branch>();
		foreach (string line in GitHelper.RunCommand(@"for-each-ref --format=""%(committerdate) %09 %(authorname) %09 %(refname) %09 %(authoremail)"" --sort=-committerdate"))
		{
			string[] sep = new string[] { "\t" };
			string[] pars = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
			if (pars.Length == 4)
			{
				ls.Add(new Branch() { DATE = pars[0].Trim(), AUTHOR = pars[1].Trim(), NAME = pars[2].Trim().Split('/')[2], AUTHOREML = pars[3].Trim() });
			}
		}
		return ls;
	}
	public static void Delete(string branch)
	{
		GitHelper.RunCommand(string.Format("branch -D {0}", branch));
	}
	public static List<Commit> EnumCommits(string branch)
	{
		List<Commit> ls = new List<Commit>();
		Commit com = null;
		string command = branch == "master" ? "log -100" : string.Format(@"log master..{0}", branch);
		foreach (string line in GitHelper.RunCommand(command + @" --date=format:""%Y.%m.%d %H:%M"""))
		{
			if (line.StartsWith("commit"))
			{
				if (com != null)
				{
					ls.Add(com);
				}
				com = new Commit();
				com.COMMIT = line.Remove(0, 7);
				com.NOTES = "";
			}
			else if (line.StartsWith("Author: "))
			{
				if (com != null)
				{
					com.AUTHOR = line.Remove(0, 8);
				}
			}
			else if (line.StartsWith("Date:   "))
			{
				if (com != null)
				{
					com.DATE = line.Remove(0, 8);
				}
			}
			else if (!string.IsNullOrEmpty(line))
			{
				if (com != null)
				{
					com.NOTES = string.IsNullOrEmpty(com.NOTES) ? line : (com.NOTES + Environment.NewLine + line);
				}
			}
		}
		if (com != null)
		{
			ls.Add(com);
		}
		return ls;
	}
}