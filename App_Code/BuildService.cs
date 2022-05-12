using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class BuildService : WebService
{
	static string btboth = "both";
	public BuildService() { }
	[WebMethod(EnableSession = true)]
	public string getUpdateWorkGit()
	{
		CurrentContext.ValidateAdmin();
		return VersionBuilder.PrepareGit();
	}
	[WebMethod(EnableSession = true)]
	public string getUpdateMergeGit(string branch)
	{
		CurrentContext.ValidateAdmin();
		return VersionMerger.PrepareGit(branch);
	}
	[WebMethod(EnableSession = true)]
	public string rebaseBranch(string branch)
	{
		CurrentContext.ValidateAdmin();
		return VersionMerger.MergeCode(branch);
	}
	[WebMethod(EnableSession = true)]
	public string versionIncrement()
	{
		CurrentContext.ValidateAdmin();
		return VersionBuilder.VersionIncrement();
	}
	[WebMethod(EnableSession = true)]
	public string push2Master()
	{
		CurrentContext.ValidateAdmin();
		string res = VersionBuilder.PushRelease();
		VersionBuilder.SendVersionAlarm();
		return res + "<br/>Finished!";
	}
	[WebMethod(EnableSession = true)]
	public string pushMerger2Master(string ttid)
	{
		CurrentContext.ValidateAdmin();
		string res = VersionMerger.Push(ttid);
		return res + "<br/>Finished!";
	}
	[WebMethod(EnableSession = true)]
	public int? GetBuilderID()
	{
		CurrentContext.ValidateAdmin();
		return VersionBuilder.GetLock();
	}
	[WebMethod(EnableSession = true)]
	public int? GetMergerID()
	{
		CurrentContext.ValidateAdmin();
		return VersionMerger.GetLock();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBuild> getBuildsByTask(int from, int to, string ttid)
	{
		CurrentContext.Validate();
		if (string.IsNullOrEmpty(ttid))
			return new List<DefectBuild>();
		return DefectBuild.EnumData(from, to, int.Parse(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBuild> getBuildRequests(int from, int to)
	{
		CurrentContext.Validate();
		return DefectBuild.EnumData(from, to);
	}
	[WebMethod(EnableSession = true)]
	public void alertVersion()
	{
		CurrentContext.ValidateAdmin();
		VersionBuilder.SendVersionAlarm();
	}
	[WebMethod]
	public string scheduledBuild()
	{
		if (HttpContext.Current.Request.Url.Host.ToUpper() != "LOCALHOST")
		{
			return "Rejected: Schedule can be run only locally.";
		}
		MPSUser u = MPSUser.FindUserbyPhone(Settings.CurrentSettings.AUTOBOTPHONE);
		if (u == null || !u.ISADMIN)
		{
			return "No auto bot users found with admin rights.";
		}
		CurrentContext.User = u;
		VersionBuilder.PrepareGit();
		VersionBuilder.VersionIncrement();
		VersionBuilder.PushRelease();
		VersionBuilder.SendVersionAlarm();
		addBuildByTask(Settings.CurrentSettings.RELEASETTID, "Automated Build", btboth);
		return "OK";
	}
	static string _tname = "TaskManagerBuilder"; 
	static WeeklyTrigger getDefTrigger()
	{
		WeeklyTrigger wt = new WeeklyTrigger();
		wt.StartBoundary = DateTime.Today.Date;
		wt.StartBoundary = wt.StartBoundary.AddHours(20);
		wt.WeeksInterval = 1;
		wt.DaysOfWeek = DaysOfTheWeek.Monday | DaysOfTheWeek.Tuesday | DaysOfTheWeek.Wednesday | DaysOfTheWeek.Thursday | DaysOfTheWeek.Friday;
		return wt;
	}
	static ExecAction getDefAction()
	{
		return new ExecAction("powershell.exe", HttpContext.Current.Server.MapPath("scripts/builder.ps1"), null);
	}
	static Task getSchedTask()
	{
		using (TaskService ts = new TaskService())
		{
			Task task = ts.FindTask(_tname);
			if (task != null)
			{
				return task;
			}
		}
		return CreateNewTask(getDefTrigger());
	}
	static Task CreateNewTask(WeeklyTrigger wt, bool enabled = false)
	{
		using (TaskService ts = new TaskService())
		{
			TaskDefinition td = ts.NewTask();
			td.RegistrationInfo.Description = _tname;
			td.Triggers.Add(wt);
			td.Actions.Add(getDefAction());
			ts.RootFolder.RegisterTaskDefinition(_tname, td);
			Task t = ts.FindTask(_tname);
			t.Enabled = enabled;
			t.Dispose();
			return ts.FindTask(_tname);
		}
	}
	public class Day
	{
		public Day() { }
		public DaysOfTheWeek DAY { get; set; }
		public bool USE { get; set; }
		public string DAYNAME
		{
			get
			{
				return DAY.ToString();
			}
		}
	}
	public class ScheduledBuild
	{
		public ScheduledBuild()
		{
			this.DAYS = new List<Day>();
		}
		public void Load()
		{
			using (Task t = getSchedTask())
			{
				this.ENABLED = t.Enabled;
				WeeklyTrigger wt = (WeeklyTrigger)t.Definition.Triggers[0];
				this.DAYS.Add(new Day() { DAY = DaysOfTheWeek.Monday, USE = (wt.DaysOfWeek & DaysOfTheWeek.Monday) == DaysOfTheWeek.Monday });
				this.DAYS.Add(new Day() { DAY = DaysOfTheWeek.Tuesday, USE = (wt.DaysOfWeek & DaysOfTheWeek.Tuesday) == DaysOfTheWeek.Tuesday });
				this.DAYS.Add(new Day() { DAY = DaysOfTheWeek.Wednesday, USE = (wt.DaysOfWeek & DaysOfTheWeek.Wednesday) == DaysOfTheWeek.Wednesday });
				this.DAYS.Add(new Day() { DAY = DaysOfTheWeek.Thursday, USE = (wt.DaysOfWeek & DaysOfTheWeek.Thursday) == DaysOfTheWeek.Thursday });
				this.DAYS.Add(new Day() { DAY = DaysOfTheWeek.Friday, USE = (wt.DaysOfWeek & DaysOfTheWeek.Friday) == DaysOfTheWeek.Friday });
				this.DAYS.Add(new Day() { DAY = DaysOfTheWeek.Saturday, USE = (wt.DaysOfWeek & DaysOfTheWeek.Saturday) == DaysOfTheWeek.Saturday });
				this.DAYS.Add(new Day() { DAY = DaysOfTheWeek.Sunday, USE = (wt.DaysOfWeek & DaysOfTheWeek.Sunday) == DaysOfTheWeek.Sunday });
				TIME = wt.StartBoundary.Hour * 60 * 60 * 1000 + wt.StartBoundary.Minute * 60 * 1000;
			}
		}
		public void Store()
		{
			using (TaskService ts = new TaskService())
			{
				ts.RootFolder.DeleteTask(_tname);
			}
			WeeklyTrigger wt = getDefTrigger();
			TimeSpan t = TimeSpan.FromMilliseconds(this.TIME);
			wt.StartBoundary = DateTime.Today + t;
			foreach (var d in DAYS)
			{
				if (d.USE)
				{
					wt.DaysOfWeek |= d.DAY;
				}
				else
				{
					wt.DaysOfWeek &= ~d.DAY;
				}
			}
			CreateNewTask(wt, this.ENABLED).Dispose();
		}
		public bool ENABLED { get; set; }
		public int TIME { get; set; }
		public List<Day> DAYS;
	}
	[WebMethod(EnableSession = true)]
	public ScheduledBuild getSchedule()
	{
		CurrentContext.ValidateAdmin();
		ScheduledBuild sb = new ScheduledBuild();
		sb.Load();
		return sb;
	}
	[WebMethod(EnableSession = true)]
	public ScheduledBuild setSchedule(ScheduledBuild sb)
	{
		CurrentContext.ValidateAdmin();
		sb.Store();
		return sb;
	}
	[WebMethod(EnableSession = true)]
	public void addBuildByTask(string ttid, string notes, string btype)
	{
		if (string.IsNullOrEmpty(ttid))
		{
			return;
		}
		DefectBuild.AddRequestByTask(Convert.ToInt32(ttid), "Public Release", DefectBuild.BuildType.releasebuild);
	}
	[WebMethod]
	public BuildRequest getInstallRequest(string machine)
	{
		DefectBuild b = DefectBuild.GetTask2Build(machine, DefectBuild.BuildType.releasebuild);
		BuildRequest r = new BuildRequest();
		if (b != null)
		{
			DefectBase def = new DefectBase(Defect.GetTTbyID(b.DEFID));
			DefectUser user = new DefectUser(int.Parse(def.AUSER));
			r.ID = b.ID;
			r.TTID = def.ID;
			r.COMM = b.NOTES;
			string em = user.EMAIL.Trim();
			if (string.IsNullOrEmpty(em))
			{
				r.USER = "ADMIN";
			}
			else
			{
				r.USER = em.Substring(0, em.IndexOf("@")).ToUpper();
			}
			r.SUMMARY = def.SUMMARY;
			r.BRANCH = def.BRANCH;
		}
		return r;
	}
}