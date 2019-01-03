using System;
using System.Collections.Generic;
using System.Management.Automation;
using Telegram.Bot;
using GitHelper;
using System.Text.RegularExpressions;

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
		Git git = new Git(path);
		List<string> res = new List<string>();

		git.SetCredentials(CurrentContext.User.PERSON_NAME, CurrentContext.User.EMAIL);
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
			Git git = new Git(path);
			res.AddRange(Git.DiffFriendOutput(git.Diff()));
			return string.Join("<br/>", res.ToArray());
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
		Git git = new Git(path);

		List<string> res = new List<string>();
		res.AddRange(git.PushOrigin());
		res.AddRange(git.Checkout("Release"));
		res.AddRange(git.Rebase("master"));
		res.AddRange(git.PushOrigin());
		res.AddRange(git.PushTags());
		return string.Join(Environment.NewLine, res.ToArray()).Replace(Environment.NewLine, "<br/>");
	}
	public static void SendAlarm(string message)
	{
		TelegramBotClient client = new TelegramBotClient(Settings.CurrentSettings.TELEGRAMBUILDTOKEN);
		client.GetMeAsync().Wait();
		client.SendTextMessageAsync(Settings.CurrentSettings.TELEGRAMBUILDCHANNEL, message, Telegram.Bot.Types.Enums.ParseMode.Html, true).Wait();
	}
	public static void SendVersionAlarm()
	{
		string details = "";
		string version = "";
		Git git = new Git(Settings.CurrentSettings.TEMPGIT);
		foreach (var f in git.GetTopCommit().EnumFiles())
		{
			if (f.Name.ToLower().Contains("changelog.txt"))
			{
				foreach (var d in f.Diff)
				{
					if (d.StartsWith("+"))
					{
						string line = d.Substring(1).Trim();
						if (line.StartsWith("=="))
						{
							version = line.Replace("=", "").Trim();
						}
						else
						{
							if (line.StartsWith("TT"))
							{
								Match m = Regex.Match(line, "TT[0-9]+");
								if (m.Success)
								{
									string ttid = m.Value.Replace("TT", "");
									line = string.Format("<a href='{0}{1}{2}'>{3}</a>", Settings.CurrentSettings.GLOBALSITEURL, StaticSettings.DefectUrl, ttid, line.Substring(0, Math.Min(line.Length, 60)));
								}
							}
							details += line + Environment.NewLine;
						}
					}
				}
			}
		}
		details = details.Trim();
		SendAlarm(string.Format("<a href='{3}versionchanges.aspx'>{0}</a> has been setup.{1}List of changes:{1}{2}{1}The build will be started as soon as possible.", version, Environment.NewLine, details, Settings.CurrentSettings.GLOBALSITEURL));
	}
}