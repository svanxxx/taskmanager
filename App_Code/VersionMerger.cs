using System;
using System.Collections.Generic;
using System.Management.Automation;
using Telegram.Bot;
using GitHelper;
using System.Text.RegularExpressions;

public class VersionMerger
{
	static int? _UserID = null;
	static DateTime? _lockTime = null;
	static object _lock = new object();
	static bool TimedOut()
	{
		if (_lockTime != null)
		{
			TimeSpan ts = DateTime.Now - _lockTime.Value;
			if (ts.TotalMinutes > 1)
			{
				return true;
			}
		}
		return false;
	}
	public static int? GetLock()
	{
		lock (_lock)
		{
			if (_lockTime != null)
			{
				if (TimedOut())
				{
					return null;
				}
			}
			return _UserID;
		}
	}
	static bool Lock()
	{
		lock (_lock)
		{
			if (_UserID == null)
			{
				_lockTime = DateTime.Now;
				_UserID = CurrentContext.UserID;
				return true;
			}
			if (_UserID == CurrentContext.UserID)
			{
				_lockTime = DateTime.Now;
				return true;
			}
			if (TimedOut())
			{
				_lockTime = DateTime.Now;
				_UserID = CurrentContext.UserID;
				return true;
			}
			return false;
		}
	}
	static Git getGit()
	{
		string path = Settings.CurrentSettings.MERGEGIT;
		if (string.IsNullOrEmpty(path))
		{
			throw new Exception("Path to git is not specified!");
		}
		Git g = new Git(path);
		g.SetCredentials(CurrentContext.User.PERSON_NAME, CurrentContext.User.EMAIL);
		return g;
	}
	static public string PrepareGit(string branch)
	{
		CurrentContext.ValidateAdmin();
		if (!Lock())
		{
			return "Locked by another user!";
		}
		Git git = getGit();
		List<string> res = new List<string>();

		git.RunCommand("rebase --abort");
		res.AddRange(git.Checkout("master"));
		git.DeleteBranch(branch);
		res.AddRange(git.ResetHard("origin/master"));
		res.AddRange(git.PullOrigin());
		res.AddRange(git.Status());
		res.AddRange(git.Checkout(branch));
		res.AddRange(git.PullOrigin());
		return string.Join(Environment.NewLine, res.ToArray()).Replace(Environment.NewLine, "<br/>");
	}
	static public string MergeCode(string branch)
	{
		CurrentContext.ValidateAdmin();
		if (!Lock())
		{
			return "Locked by another user!";
		}
		Git git = getGit();
		List<string> res = new List<string>();

		res.AddRange(git.Rebase("master"));
		res.AddRange(git.Checkout("master"));
		res.AddRange(git.Rebase(branch));
		res.AddRange(git.Status());
		return string.Join(Environment.NewLine, res.ToArray()).Replace(Environment.NewLine, "<br/>");
	}
	static public string Push(string ttid)
	{
		CurrentContext.ValidateAdmin();
		if (!Lock())
		{
			return "Locked by another user!";
		}
		Git git = getGit();
		List<string> res = new List<string>();

		DefectBase d = new DefectBase(ttid);

		res.AddRange(git.PushOrigin());
		git.DeleteBranch(d.BRANCH);
		res.AddRange(git.Status());
		
		DefectUser ttu = new DefectUser(d.AUSER);
		MPSUser mpu = new MPSUser(ttu.TRID);
		string mess = $"Task commit has been pushed to master by {CurrentContext.UserName()}{Settings.CurrentSettings.GetTTAnchor(d.ID, "git-pull-request.png")}";
		TasksBot.SendMessage(mpu.CHATID, mess);
		return string.Join(Environment.NewLine, res.ToArray()).Replace(Environment.NewLine, "<br/>");
	}
}