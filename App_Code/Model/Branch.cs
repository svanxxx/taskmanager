using System;
using System.Collections.Generic;

public class Branch
{
	public string DATE { get; set; }
	public string NAME { get; set; }
	public string AUTHOR { get; set; }
	public string AUTHOREML { get; set; }
	public string COLOR
	{
		set { }
		get
		{
			return NAME == "master" ? "#ff00004a" : (NAME == "Release" ? "#0000ff3d" : "white");
		}
	}
	public int TTID
	{
		set { }
		get
		{
			int ttid;
			if (NAME.StartsWith("TT") && int.TryParse(NAME.Substring(2), out ttid))
			{
				return ttid;
			}
			return -1;
		}
	}
	public Branch()
	{
	}
	static object _lock = new object();
	static DateTime _loadtime = DateTime.Now;
	static List<Branch> _branches = new List<Branch>();
	public static List<Branch> Enum(string user)
	{
		lock (_lock)
		{
			if (_branches.Count < 1 || (DateTime.Now - _loadtime).TotalSeconds > 20) //cached by 20 seconds span to reduce disk load and response time
			{
				_branches.Clear();
				_loadtime = DateTime.Now;
				foreach (string line in (new GitHelper()).RunCommand(@"for-each-ref --format=""%(committerdate) %09 %(authorname) %09 %(refname) %09 %(authoremail)"" --sort=-committerdate"))
				{
					string[] sep = new string[] { "\t" };
					string[] pars = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
					if (pars.Length == 4)
					{
						_branches.Add(new Branch() { DATE = pars[0].Trim(), AUTHOR = pars[1].Trim(), NAME = pars[2].Trim().Split('/')[2], AUTHOREML = pars[3].Trim('>', '<', ' ', '\t') });
					}
				}
			}
			if (string.IsNullOrEmpty(user))
			{
				return new List<Branch>(_branches);
			}
			return new List<Branch>(_branches.FindAll(s => s.AUTHOR == user));
		}
	}
	public static List<Branch> Enum(int from, int to, string user)
	{
		List<Branch> ls = Enum(user);
		int count = to - from + 1;
		if (ls.Count < 1 || from < 1 || from > ls.Count || count < 1)
		{
			return new List<Branch>();
		}
		count = Math.Min(count, ls.Count - from + 1);
		return ls.GetRange(from - 1, count);
	}
	public static void Delete(string branch)
	{
		(new GitHelper()).RunCommand(string.Format("branch -D {0}", branch));
	}
	public static List<Commit> EnumCommits(string branch, int from, int to)
	{
		List<Commit> ls = new List<Commit>();
		Commit com = null;
		string command = branch == "master" ? "log -100" : string.Format(@"log master..{0}", branch);
		foreach (string line in (new GitHelper()).RunCommand(command + @" --date=format:""%Y.%m.%d %H:%M"""))
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

		if (from > ls.Count)
		{
			return new List<Commit>();
		}

		int ifrom = Math.Min(ls.Count - 1, from - 1);
		int ito = Math.Min(ls.Count - ifrom, to - from + 1);
		return ls.GetRange(ifrom, ito);
	}
	public static List<ChangedFile> EnumFiles(string branch)
	{
		List<ChangedFile> ls = new List<ChangedFile>();
		string command = "diff --name-status {0} master";
		foreach (string line in (new GitHelper()).RunCommand(command))
		{
			ls.Add(new ChangedFile(line.Trim()));
		}
		return ls;
	}
}