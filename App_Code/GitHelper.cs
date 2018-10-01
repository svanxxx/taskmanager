using System.Collections.Generic;
using System.Management.Automation;

public class GitHelper
{
	public GitHelper()
	{
	}
	public static List<string> RunCommand(string command)
	{
		using (PowerShell ps = PowerShell.Create())
		{
			ps.AddScript(@"cd \\192.168.0.1\git\v8");
			ps.AddScript(@"git " + command);
			List<string> ls = new List<string>();
			foreach (var line in ps.Invoke())
			{
				ls.Add(line.ToString().Trim());
			}
			return ls;
		}
	}
}