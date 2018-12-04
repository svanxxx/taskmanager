using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;
using Telegram.Bot;

public class VersionBuilder
{
	static public string PrepareGit()
	{
		if (!CurrentContext.Admin)
		{
			throw new Exception("Not admin!");
		}
		string path = Settings.CurrentSettings.TEMPGIT;
		if (string.IsNullOrEmpty(path))
		{
			throw new Exception("Path to git is not specified!");
		}
		GitHelper git = new GitHelper(path);
		List<string> res = new List<string>();
		git.ResetHard();
		res.AddRange(git.Checkout("master"));
		res.AddRange(git.ResetHard("origin/master"));
		res.AddRange(git.CleanaupLocalTags());
		res.AddRange(git.PullOrigin());
		res.AddRange(git.Status());
		return string.Join(Environment.NewLine, res.ToArray()).Replace(Environment.NewLine, "<br/>");
	}
	static public string VersionIncrement()
	{
		if (!CurrentContext.Admin)
		{
			throw new Exception("Not admin!");
		}
		string path = Settings.CurrentSettings.TEMPGIT;
		if (string.IsNullOrEmpty(path))
		{
			throw new Exception("Path to git is not specified!");
		}

		using (PowerShell ps = PowerShell.Create())
		{
			string script = Settings.CurrentSettings.TEMPGIT + "Projects.32\\version.ps1 1";
			ps.AddScript("& " + script);
			List<string> res = new List<string>();
			foreach (var line in ps.Invoke())
			{
				res.Add(line.ToString().Trim());
			}
			if (ps.HadErrors)
			{
				foreach(var e in ps.Streams.Error)
				{
					res.Add(e.ToString());
				}
			}
			GitHelper git = new GitHelper(path);
			res.AddRange(git.Diff());
			List<string> processed = new List<string>();
			for(int i = 0; i < res.Count; i++)
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
			return string.Join("", processed.ToArray());
		}
	}
	static public string PushRelease()
	{
		if (!CurrentContext.Admin)
		{
			throw new Exception("Not admin!");
		}
		string path = Settings.CurrentSettings.TEMPGIT;
		if (string.IsNullOrEmpty(path))
		{
			throw new Exception("Path to git is not specified!");
		}
		GitHelper git = new GitHelper(path);

		List<string> res = new List<string>();
		res.AddRange(git.PushOrigin());
		res.AddRange(git.Checkout("release"));
		res.AddRange(git.Rebase("master"));
		res.AddRange(git.PushOrigin());
		res.AddRange(git.PushTags());
		return string.Join(Environment.NewLine, res.ToArray()).Replace(Environment.NewLine, "<br/>");
	}
	public static void SendAlarm(string message)
	{
		TelegramBotClient client = new TelegramBotClient(Settings.CurrentSettings.TELEGRAMBUILDTOKEN);
		client.GetMeAsync().Wait();
		client.SendTextMessageAsync("@" + Settings.CurrentSettings.TELEGRAMBUILDCHANNEL, message).Wait();
	}
}