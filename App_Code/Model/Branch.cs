using System;
using System.Collections.Generic;
using System.Management.Automation;

public class Branch
{
	public string NAME { get; set; }

	public Branch()
	{
	}
	public Branch(string name)
	{
		NAME = name;
	}
	public static List<Branch> Enum()
	{
		List<Branch> ls = new List<Branch>();
		foreach (string line in GitHelper.RunCommand("branch --sort=-committerdate"))
		{
			ls.Add(new Branch(line));
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
		foreach (string line in GitHelper.RunCommand(string.Format(@"log master..{0}", branch)))
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
			if (com != null)
			{
				ls.Add(com);
			}
		}
		return ls;
	}
}