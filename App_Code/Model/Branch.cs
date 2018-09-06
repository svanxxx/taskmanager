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
		using (PowerShell ps = PowerShell.Create())
		{
			ps.AddScript(@"cd \\192.168.0.1\git\v8");
			ps.AddScript(@"git branch");
			foreach (var cm in ps.Invoke())
			{
				ls.Add(new Branch(cm.ToString().Trim()));
			}
		}
		return ls;
	}
	public static List<Commit> EnumCommits(string branch)
	{
		List<Commit> ls = new List<Commit>();
		using (PowerShell ps = PowerShell.Create())
		{
			ps.AddScript(@"cd \\192.168.0.1\git\v8");
			ps.AddScript(string.Format(@"git log master..{0}", branch));
			Commit com = null;
			foreach (var cm in ps.Invoke())
			{
				string st = cm.ToString().Trim();
				if (st.StartsWith("commit"))
				{
					if (com != null)
					{
						ls.Add(com);
					}
					com = new Commit();
					com.COMMIT = st.Remove(0, 7);
					com.NOTES = "";
				}
				else if (st.StartsWith("Author: "))
				{
					if (com != null)
					{
						com.AUTHOR = st.Remove(0, 8);
					}
				}
				else if (st.StartsWith("Date:   "))
				{
					if (com != null)
					{
						com.DATE = st.Remove(0, 8);
					}
				}
				else if (!string.IsNullOrEmpty(st))
				{
					com.NOTES = string.IsNullOrEmpty(com.NOTES) ? st : (com.NOTES + Environment.NewLine + st);
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