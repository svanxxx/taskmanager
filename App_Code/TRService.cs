﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Xml.Serialization;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class TRService : WebService
{
	static public string defDateFormat = "MM-dd-yyyy";
	public TRService()
	{
		//Uncomment the following line if using designed components 
		//InitializeComponent(); 
	}
	//========================references==============================================================
	//================================================================================================
	void StoreObjects(IdBasedObject[] data)
	{
		foreach (var d in data)
		{
			var dstore = Activator.CreateInstance(d.GetType(), d.GetID()) as Reference;
			dstore.FromAnotherObject(d);
			if (dstore.IsModified())
			{
				dstore.Store();
			}
		}
	}
	[WebMethod(EnableSession = true)]
	public List<DefectComp> gettaskcomps()
	{
		return DefectComp.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settaskcomps(List<DefectComp> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectDispo> gettaskdispos()
	{
		return DefectDispo.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void setdispos(List<DefectDispo> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectSeverity> gettasksevers()
	{
		return DefectSeverity.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void setsevers(List<DefectSeverity> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectProduct> gettaskproducts()
	{
		return DefectProduct.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settaskproducts(List<DefectProduct> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectType> gettasktypes()
	{
		return DefectType.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settasktypes(List<DefectType> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectPriority> gettaskpriorities()
	{
		return DefectPriority.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settaskpriorities(List<DefectPriority> data)
	{
		StoreObjects(data.ToArray());
	}
	//================================================================================================
	//================================================================================================
	[WebMethod(EnableSession = true)]
	public int newTask4MeNow(string summary)
	{
		if (string.IsNullOrEmpty(summary))
			return -1;
		DefectBase d = new DefectBase(Defect.New(summary));
		d.AUSER = CurrentContext.TTUSERID.ToString();
		d.ESTIM = 1;
		d.DISPO = DefectDispo.GetWorkingRec().ToString();
		d.ORDER = 1;
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public int planTask(string summary, int ttuserid)
	{
		if (string.IsNullOrEmpty(summary))
			return -1;
		DefectBase d = new DefectBase(Defect.New(summary));
		d.AUSER = ttuserid == -1 ? CurrentContext.TTUSERID.ToString() : ttuserid.ToString();
		d.ESTIM = 1;
		List<int> disp = DefectDispo.EnumCannotStartIDs();
		if (disp.Count > 0)
		{
			d.DISPO = disp[0].ToString();
		}
		d.ORDER = 1;
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public void addVacation(string summary, int ttuserid, int num)
	{
		if (string.IsNullOrEmpty(summary) || ttuserid < 1 || num < 1 || num > 100)
			return;
		for (int i = 0; i < num; i++)
		{
			DefectBase d = new DefectBase(Defect.New(summary + " #" + (i + 1).ToString()));
			d.AUSER = ttuserid.ToString();
			d.ESTIM = 8;
			d.COMP = DefectComp.GetVacationRec()[0].ToString();
			List<int> disp = DefectDispo.EnumCannotStartIDs();
			if (disp.Count > 0)
			{
				d.DISPO = disp[0].ToString();
			}
			d.DATE = new DateTime(DateTime.Now.Year, 12, 31).ToString(defDateFormat);
			d.Store();
		}
		MPSUser mpu = new MPSUser(new DefectUser(ttuserid).TRID);
		TasksBot.SendMessage(mpu.CHATID, $"{num} vacation tasks have been created for you by {CurrentContext.UserName()}");
	}
	[WebMethod(EnableSession = true)]
	public void addSickness(string details, int ttuserid)
	{
		if (!CurrentContext.Valid)
			return;

		if (string.IsNullOrEmpty(details) || ttuserid < 1)
			return;
		DateTime dt = DateTime.Today;
		Defect d = new Defect(Defect.New("SICKNESS DAY " + dt.Year));
		d.DESCR = $"{CurrentContext.User.PERSON_NAME}: {details}";
		d.AUSER = ttuserid.ToString();
		d.DISPO = DefectDispo.GetWorkingRec().ToString();
		d.ESTIM = 8;
		d.COMP = DefectComp.GetVacationRec()[0].ToString();
		d.DATE = dt.ToString(defDateFormat);
		d.Store();
		TasksBot.SendMessage(Settings.CurrentSettings.TELEGRAMCOMPANYCHANNEL, $"🌡{details}");
	}
	[WebMethod(EnableSession = true)]
	public int newTask(string summary)
	{
		if (string.IsNullOrEmpty(summary))
			return -1;
		DefectBase d = new DefectBase(Defect.New(summary));
		d.ESTIM = 1;
		var disp = DefectDispo.EnumCannotStartIDs();
		if (disp.Any())
		{
			d.DISPO = disp.First().ToString();
		}
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public int copyTask(int ttid)
	{
		Defect old = new Defect(ttid);
		Defect d = new Defect(Defect.New(old.SUMMARY));
		d.From(old);
		d.AUSER = "";
		d.ESTIM = 0;
		d.ORDER = -1;
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public void copyTasks(string ttids)
	{
		string[] ids = ttids.Split(',');
		foreach (string id in ids)
		{
			Defect old = new Defect(int.Parse(id));
			Defect d = new Defect(Defect.New(old.SUMMARY));
			d.From(old);
			d.DISPO = old.DISPO;
			d.Store();
		}
	}
	[WebMethod(EnableSession = true)]
	public StoredDefectsFilter saveFilter(string name, bool personal, DefectsFilter filter)
	{
		CurrentContext.Validate();
		return StoredDefectsFilter.NewFilter(name, personal, filter, CurrentContext.TTUSERID);
	}
	[WebMethod(EnableSession = true)]
	public StoredDefectsFilter udpateFilter(int id, DefectsFilter filter)
	{
		CurrentContext.Validate();
		StoredDefectsFilter stf = new StoredDefectsFilter(id);
		stf.SetFilter(filter);
		stf.Store();
		return stf;
	}
	[WebMethod(EnableSession = true)]
	public void deleteFilter(int id)
	{
		StoredDefectsFilter.Delete(id);
	}
	[WebMethod(EnableSession = true)]
	public DefectsFilter savedFilterData(int id)
	{
		return (new StoredDefectsFilter(id)).GetFilter();
	}
	[WebMethod(EnableSession = true)]
	public List<StoredDefectsFilter> getFilters()
	{
		return StoredDefectsFilter.Enum(CurrentContext.TTUSERID);
	}
	[WebMethod(EnableSession = true)]
	public Defect gettask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		Defect d = new Defect(Convert.ToInt32(ttid));
		if (!d.IsLoaded())
			return null;
		return d;
	}
	[WebMethod(EnableSession = true)]
	public void settaskBase(List<DefectBase> defects)
	{
		foreach (var d in defects)
		{
			Defect dstore = new Defect(d.ID);
			dstore.FromAnotherObject(d);
			if (!string.IsNullOrEmpty(d.add_details))
			{
				dstore.DESCR += $"{Environment.NewLine}<task-message userid='{CurrentContext.UserID}' user='{CurrentContext.UserLogin()}' time='{DateTime.Now.ToString()}'>{d.add_details}</task-message>";
			}
			if (dstore.IsModified())
			{
				dstore.Store();
			}
		}
	}
	[WebMethod(EnableSession = true)]
	public List<DefectHistory> gettaskhistory(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectHistory.GetHisotoryByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectAttach> getattachsbytask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectAttach.GetAttachsByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public void cancelBuildByTask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return;
		DefectBuild.CancelRequestByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectUser> gettaskusers()
	{
		return DefectUser.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void newfileupload(string ttid, string filename, string data)
	{
		try
		{
			DefectAttach.AddAttachByTask(Convert.ToInt32(ttid), filename, data.Remove(0, data.IndexOf("base64,") + 7));
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
	[WebMethod(EnableSession = true)]
	public void delfileupload(string ttid, string id)
	{
		int iid = Convert.ToInt32(id);
		if (iid < 1)
		{
			return;
		}
		DefectAttach.DeleteAttach(ttid, iid);
	}
	[WebMethod(EnableSession = true)]
	public LockInfo locktask(string ttid, string lockid)
	{
		if (!CurrentContext.Valid || string.IsNullOrEmpty(ttid))
		{
			return null;
		}
		return Defect.Locktask(ttid, lockid, CurrentContext.UserID.ToString());
	}
	[WebMethod(EnableSession = true)]
	public void unlocktask(string ttid, string lockid)
	{
		Defect.UnLocktask(ttid, lockid);
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> gettasks(DefectsFilter f)
	{
		DefectBase enm = new DefectBase();
		return enm.Enum(f);
	}
	[WebMethod(EnableSession = true)]
	public TRRec gettrrec(string date)
	{
		if (!CurrentContext.Valid)
		{
			return null;
		}
		DateTime d = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		return r;
	}
	[WebMethod(EnableSession = true)]
	public void settrrec(TRRec rec)
	{
		CurrentContext.Validate();
		try
		{
			TRRec store = new TRRec(rec.ID);
			store.FromAnotherObject(rec);
			if (store.IsModified())
			{
				store.Store();
			}
		}
		catch { }
	}
	[WebMethod(EnableSession = true)]
	public void deltrrec(int id)
	{
		TRRec.DelRec(id);
	}
	[WebMethod(EnableSession = true)]
	public void todayrrec(string lastday)
	{
		if (!CurrentContext.Valid)
		{
			return;
		}
		DateTime d = DateTime.Today;
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		if (r == null)
		{
			TRRec.NewRec(d, CurrentContext.User.ID, lastday == "True");
		}
	}
	[WebMethod(EnableSession = true)]
	public void addrec(string date, string lastday)
	{
		DateTime d = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		if (r == null)
		{
			TRRec.NewRec(d, CurrentContext.User.ID, lastday == "True");
		}
	}
	[WebMethod(EnableSession = true)]
	public DefectBase scheduletask(string ttid, string date)
	{
		if (Defect.Locked(ttid))
			return null;
		Defect d = new Defect(Convert.ToInt32(ttid));
		d.DISPO = DefectDispo.GetWorkingRec().ToString();
		d.DATE = date;
		d.Store();
		return new DefectBase(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<MPSUser> getActiveMPSusers()
	{
		return getMPSUsers(true);
	}
	[WebMethod(EnableSession = true)]
	public List<MPSUser> getMPSUsers(bool active)
	{
		return MPSUser.EnumAllUsers(active);
	}
	[WebMethod(EnableSession = true)]
	public string setusers(List<MPSUser> users)
	{
		CurrentContext.ValidateAdmin();
		foreach (MPSUser u in users)
		{
			MPSUser ustore = new MPSUser(u.ID);
			ustore.FromAnotherObject(u);
			if (ustore.IsModified())
			{
				ustore.Store();
			}
		}
		return "OK";
	}
	public class TTBackOrder
	{
		public int ttid { get; set; }
		public int backorder { get; set; }
		public bool moved { get; set; }
	}
	[WebMethod(EnableSession = true)]
	public void setschedule(List<TTBackOrder> ttids, List<string> unschedule)
	{
		if (!CurrentContext.Valid)
		{
			return;
		}

		foreach (var ttid in unschedule)
		{
			Defect d = new Defect(Convert.ToInt32(ttid));
			d.ORDER = -1;
			d.Store();
		}

		foreach (var ttid in ttids)
		{
			DefectBase d;
			if (ttid.moved)
			{
				d = new Defect(ttid.ttid); //will add history record about moving
			}
			else
			{
				d = new DefectBase(ttid.ttid);
			}
			if (d.BACKORDER != ttid.backorder)
			{
				d.BACKORDER = ttid.backorder;
				d.Store();
			}
		}
	}
	[WebMethod(EnableSession = true)]
	public List<TRRec> getreports(List<string> dates)
	{
		if (!CurrentContext.Valid || dates.Count < 1)
		{
			return new List<TRRec>();
		}
		List<DateTime> datetimes = new List<DateTime>();
		foreach (var d in dates)
		{
			datetimes.Add(DateTime.ParseExact(d, defDateFormat, CultureInfo.InvariantCulture));
		}
		return TRRec.Enum(datetimes.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> enumCloseVacations(string start, int days)
	{
		return Vacations.EnumCloseVacations(start, days);
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> enumUnusedVacations()
	{
		return DefectBase.EnumUnusedVacations();
	}
	[WebMethod(EnableSession = true)]
	public void pageLoadedComplete(int id)
	{
		PageLoadNofify.RemoveLoad(id);
	}
	[WebMethod(EnableSession = true)]
	public List<TRRecSignal> enumTRSignal(string from, string to)
	{
		if (!CurrentContext.Valid)
		{
			return new List<TRRecSignal>();
		}
		return TRRecSignal.Enum(DateTime.ParseExact(from, defDateFormat, CultureInfo.InvariantCulture), DateTime.ParseExact(to, defDateFormat, CultureInfo.InvariantCulture));
	}
	[WebMethod(EnableSession = true)]
	public List<Statistic> getStatistics(string start, string days)
	{
		return Defect.EnumStatistics(DateTime.ParseExact(start, defDateFormat, CultureInfo.InvariantCulture), Convert.ToInt32(days));
	}
	[WebMethod(EnableSession = true)]
	public List<TRStatistic> getTRStatistic(string start, string days)
	{
		return TRRec.EnumTRStatistics(DateTime.ParseExact(start, defDateFormat, CultureInfo.InvariantCulture), Convert.ToInt32(days));
	}
	[WebMethod(EnableSession = true)]
	public List<TRRec> getreports4Person(int personid, string start, int days, string text)
	{
		if (!CurrentContext.Valid)
		{
			return new List<TRRec>();
		}
		return TRRec.EnumPersonal(personid, DateTime.ParseExact(start, defDateFormat, CultureInfo.InvariantCulture), days, text);
	}
	[WebMethod(EnableSession = true)]
	public RawSettings getSettings()
	{
		return RawSettings.CurrentRawSettings;
	}
	[WebMethod(EnableSession = true)]
	public void setSettings(RawSettings s)
	{
		if (!CurrentContext.Valid || !CurrentContext.Admin)
		{
			return;
		}
		s.Store();
	}
	[WebMethod(EnableSession = true)]
	public DefectDefaults getDefaults()
	{
		return DefectDefaults.CurrentDefaults;
	}
	[WebMethod(EnableSession = true)]
	public void setDefaults(DefectDefaults d)
	{
		if (!CurrentContext.Valid || !CurrentContext.Admin)
		{
			return;
		}
		DefectDefaults.CurrentDefaults = d;
	}
	[WebMethod]
	public BuildRequest getBuildRequest(string machine)
	{
		DefectBuild b = DefectBuild.GetTask2Build(machine, DefectBuild.BuildType.testbuild);
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
	[WebMethod]
	public bool hasBuildRequest()
	{
		return DefectBuild.hasBuildRequest();
	}
	[WebMethod]
	public void CommentBuild(int id, string comment)
	{
		DefectBuild b = new DefectBuild(id)
		{
			STATUSTXT = comment
		};
		b.Store();
	}
	[WebMethod]
	public void VersionBuildAlarm(string message)
	{
		VersionBuilder.SendAlarm($"{message}");
	}
	[WebMethod]
	public void FinishBuild(int id, string requestguid)
	{
		try
		{
			DefectBuild b = new DefectBuild(id) { STATUS = DefectBuild.BuildStatus.finishedok.ToString(), TESTGUID = requestguid };
			b.Store();

			if (Settings.CurrentSettings.RELEASETTID == b.TTID.ToString() && b.TYPE == (int)DefectBuild.BuildType.releasebuild)
			{
				//release builder sends its own notifications
				return;
			}

			Defect d = new Defect(b.TTID);
			DefectUser u = new DefectUser(b.TTUSERID);
			d.SetUpdater(new MPSUser(u.TRID));
			List<DefectDispo> dsps = DefectDispo.EnumTestsStarted();
			if (dsps.Count > 0)
			{
				string currentlock = Guid.NewGuid().ToString();
				LockInfo li = Defect.Locktask(b.TTID.ToString(), currentlock, u.TRID.ToString(), true);
				d.DISPO = dsps[0].ID.ToString();
				if (d.PRIMARYHOURS == null)
				{
					d.PRIMARYHOURS = d.SPENT;
				}
				d.Store();
				DefectEvent.AddEventByTask(id, DefectEvent.Eventtype.QualityAssurance, b.TTUSERID, "Sent for QA Automation");
				Defect.UnLocktask(u.TRID.ToString(), currentlock);
			}

			if (Settings.CurrentSettings.RELEASETTID == b.TTID.ToString())
			{
				VersionBuilder.SendAlarm("✅New internal release build has been finished. Testing is starting...");
			}
			else
			{
				try
				{
					string mess = $"New task from {u.FULLNAME} is ready for tests!{Settings.CurrentSettings.GetTTAnchor(b.TTID, d.FIRE ? "taskfire.png" : "")}";
					TestChannel.SendMessage(mess);
				}
				catch (Exception e)
				{
					Logger.Log(e);
				}
			}

			string bst_b = d.BSTBATCHES.Trim();
			string bst_c = d.BSTCOMMANDS.Trim();
			if (!string.IsNullOrEmpty(bst_b) || !string.IsNullOrEmpty(bst_c))
			{
				string batches = string.Join(",", bst_b.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
				string commands = string.Join(",", bst_c.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
				using (var wcClient = new WebClient())
				{
					var reqparm = new NameValueCollection();
					reqparm.Add("guid", requestguid);
					reqparm.Add("commaseparatedbatches", batches);
					reqparm.Add("commaseparatedcommands", commands);
					reqparm.Add("priority", d.TESTPRIORITY);
					//reqparm.Add("branch", d.BRANCHBST);
					wcClient.UploadValues(Settings.CurrentSettings.BSTSITESERVICE + "/StartTest", reqparm);
				}
			}
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
	[WebMethod]
	public void SetTaskTestStatus(string ttid, string failed, string userphone)
	{
		try
		{
			MPSUser bsu = MPSUser.FindUserbyPhone(userphone);
			if (bsu == null)
			{
				Logger.Log($"Cannot update task {ttid} by testing system. User was not found by phone number: {userphone}");
				return;
			}

			Defect d = new Defect(ttid);
			d.SetUpdater(bsu);
			string lockguid = Guid.NewGuid().ToString();
			var lt = Defect.Locktask(ttid.ToString(), lockguid, bsu.ID.ToString());
			bool locked = lt.globallock != lockguid;
			bool testFail;
			bool testcancel = false;
			if (!bool.TryParse(failed, out testFail))
			{
				testcancel = true;
			}
			if (locked)
			{
				MPSUser lu = new MPSUser(lt.lockedby);
				TasksBot.SendMessage(lu.CHATID, $"You was disconnected from the task by the testing system to update task status!{Settings.CurrentSettings.GetTTAnchor(int.Parse(ttid), "disconnect.png")}");
				NotifyHub.lockTaskForceUpdatePages(int.Parse(ttid), lockguid, bsu.ID);
				lt = Defect.Locktask(ttid.ToString(), lockguid, bsu.ID.ToString());
			}
			List<DefectDispo> disp = (testcancel || testFail) ? DefectDispo.EnumTestsFailed() : DefectDispo.EnumTestsPassed();
			if (disp.Count > 0)
			{
				if (!testcancel)
				{
					d.DISPO = disp[0].ID.ToString();
				}
				else
				{
					d.AddMessage("Test request have been ignored", bsu.ID);
				}
				d.Store();
				Defect.UnLocktask(ttid, lt.globallock);

				if (!testcancel)
				{
					DefectUser du = new DefectUser(d.AUSER);
					if (du.TRID > -1)
					{
						MPSUser worker = new MPSUser(du.TRID);
						string result = "Succeeded!";
						string img = "taskokay.png";
						if (testcancel)
						{
							result = "Cancelled!";
							img = "bin.png";
						}
						else if (testFail)
						{
							result = "Failed!";
							img = "taskfail.png";
						}
						TasksBot.SendMessage(worker.CHATID, $"The task tests have been marked as BST {result} by {bsu.PERSON_NAME}{Settings.CurrentSettings.GetTTAnchor(int.Parse(ttid), img)}");
					}
				}
			}
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
	[WebMethod(EnableSession = true)]
	public void NotifyDefect(string ttid, string message, string img, bool alsoteam)
	{
		try
		{
			if (!CurrentContext.Valid)
			{
				throw new Exception("NotifyDefect called without loggin in!");
			}
			Defect d = new Defect(ttid);
			DefectUser du = new DefectUser(d.AUSER);
			if (du.TRID > -1)
			{
				MPSUser worker = new MPSUser(du.TRID);
				string mess2send = $"TT{ttid} update from {CurrentContext.UserName()}:  {message.Replace("<", "&#60;").Replace(">", "&#62;")}{Settings.CurrentSettings.GetTTAnchor(int.Parse(ttid), img)}";
				TasksBot.SendMessage(worker.CHATID, mess2send);
				if (alsoteam)
				{
					TestChannel.SendMessage(mess2send);
				}
			}
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
	[WebMethod]
	public void NotifyDefectWorker(string ttid, string message, string userphone)
	{
		try
		{
			MPSUser bsu = MPSUser.FindUserbyPhone(userphone);
			if (bsu == null)
			{
				Logger.Log($"Cannot update task {ttid} by testing system. User was not found by phone number: {userphone}");
				return;
			}

			Defect d = new Defect(ttid);
			DefectUser du = new DefectUser(d.AUSER);
			if (du.TRID > -1)
			{
				MPSUser worker = new MPSUser(du.TRID);
				TasksBot.SendMessage(worker.CHATID, message);
			}
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
	[WebMethod]
	public bool IsBuildCancelled(int id)
	{
		DefectBuild b = new DefectBuild(id);
		return b.CANCELLED;
	}
	[WebMethod]
	public string geBuildLogDir()
	{
		return Settings.CurrentSettings.BUILDLOGSDIR;
	}
	[WebMethod]
	public void FailBuild(int id)
	{
		DefectBuild b = new DefectBuild(id)
		{
			STATUS = DefectBuild.BuildStatus.failed.ToString()
		};
		b.Store();
		if (Settings.CurrentSettings.RELEASETTID == b.TTID.ToString())
		{
			VersionBuilder.SendAlarm("❌Failed to build version. Please check the logs!!!");
		}
	}
	[WebMethod(EnableSession = true)]
	public string alarmEmail(int ttid, string addresses)
	{
		if (!CurrentContext.Valid)
			return "Please login";

		Defect d = new Defect(ttid);
		MailMessage mail = new MailMessage();

		foreach (string addr in addresses.Split(','))
		{
			mail.To.Add(new MailAddress(addr.Trim()));
		}
		mail.From = new MailAddress(CurrentContext.User.EMAIL.Trim());
		mail.Subject = string.Format("TT{0} {1}", d.ID, d.SUMMARY);
		mail.IsBodyHtml = true;

		string descr = d.DESCR.Replace(Environment.NewLine, "<br/>");
		descr = descr.Replace("\n", "<br/>");

		descr = BodyProcessor.ResolveLinks(descr);
		descr = Regex.Replace(descr, "----+", "<hr>");
		descr = Regex.Replace(descr, "====+", "<hr>");

		string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
		body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
		body += "</HEAD><BODY'>" + descr + " </BODY></HTML>";

		System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
		AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
		mail.AlternateViews.Add(alternate);

		SmtpClient smtp = new SmtpClient();
		Settings sett = Settings.CurrentSettings;
		smtp.Host = sett.SMTPHOST;
		smtp.Port = Convert.ToInt32(sett.SMTPPORT);
		smtp.EnableSsl = Convert.ToBoolean(sett.SMTPENABLESSL); ;
		smtp.Timeout = Convert.ToInt32(sett.SMTPTIMEOUT); ;
		smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
		smtp.UseDefaultCredentials = false;
		smtp.Credentials = new NetworkCredential(sett.CREDENTIALS1, sett.CREDENTIALS2);

		string strError = "The email was sent!";
		long counter = 0;
		while (true)
		{
			try
			{
				counter++;
				smtp.Send(mail);
				break;
			}
			catch (Exception e)
			{
				strError = e.Message;
				if (!strError.Contains("The operation has timed out.") || counter > 10)
				{
					break;
				}
			}
		}
		return strError;
	}
	[WebMethod(EnableSession = true)]
	public List<string> getBSTBatches()
	{
		Uri _uri = new Uri(Settings.CurrentSettings.BSTSITESERVICE + "/EnumBatches");
		using (var wcClient = new WebClient())
		{
			string res = wcClient.UploadString(_uri, "POST", "");
			XmlSerializer ser = new XmlSerializer(typeof(string[]), new XmlRootAttribute("ArrayOfString") { Namespace = "http://tempuri.org/" });
			string[] arrres = (string[])ser.Deserialize(new StringReader(res));
			return new List<string>(arrres);
		}
	}
	[WebMethod(EnableSession = true)]
	public List<string> getBSTBatchData(string batch)
	{
		using (var wcClient = new WebClient())
		{
			var reqparm = new NameValueCollection();
			reqparm.Add("name", batch);
			byte[] result = wcClient.UploadValues(Settings.CurrentSettings.BSTSITESERVICE + "/getBatchData", reqparm);
			string res = Encoding.ASCII.GetString(result);
			XmlSerializer ser = new XmlSerializer(typeof(string[]), new XmlRootAttribute("ArrayOfString") { Namespace = "http://tempuri.org/" });
			string[] arrres = (string[])ser.Deserialize(new StringReader(res));
			return new List<string>(arrres);
		}
	}
	[WebMethod(EnableSession = true)]
	public int getTestID(string requestGUID)
	{
		using (var wcClient = new WebClient())
		{
			var reqparm = new NameValueCollection();
			reqparm.Add("guid", requestGUID);
			byte[] result = wcClient.UploadValues(Settings.CurrentSettings.BSTSITESERVICE + "/GetTestID", reqparm);
			string sres = Encoding.ASCII.GetString(result);
			XmlSerializer ser = new XmlSerializer(typeof(int), new XmlRootAttribute("int") { Namespace = "http://tempuri.org/" });
			return (int)ser.Deserialize(new StringReader(sres));
		}
	}
	[WebMethod(EnableSession = true)]
	public string addRef(string type, string desc)
	{
		if (!CurrentContext.Valid)
		{
			return "FAILED";
		}
		if (!CurrentContext.Admin)
		{
			return "FAILED - not admin.";
		}
		RefType rtype;
		if (!Enum.TryParse(type, out rtype))
		{
			return "Invalid argument.";
		}
		switch (rtype)
		{
			case RefType.disposition:
				return DefectDispo.New(desc).ToString();
			case RefType.severity:
				return DefectSeverity.New(desc).ToString();
			case RefType.component:
				return DefectComp.New(desc).ToString();
		}
		return "Unsupported";
	}
	[WebMethod(EnableSession = true)]
	public string getLog(int from, int to)
	{
		return Logger.GetLog(from, to);
	}
	[WebMethod(EnableSession = true)]
	public string clearLog()
	{
		return Logger.ClearLog();
	}
}