using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Http;

[ApiKeyFilter]
public class TaskController : ApiController
{
	[HttpGet]
	public async Task<bool> SetGloabalDispo(int ttid, GlobalDispo dispo, string email)
	{
		var dispRef = await DefectDispo.GetDispoFromGlobal(dispo);
		string currentlock = Guid.NewGuid().ToString();
		var user = DefectUser.FindByEmail(email);
		if (user == null)
		{
			return false;
		}
		LockInfo li = await Defect.LocktaskAsync(ttid.ToString(), currentlock, user.TRID.ToString(), true);
		Defect d = new Defect(ttid);
		d.SetUpdater(new MPSUser(user.TRID));
		d.DISPO = dispRef.idRecord.ToString();
		if (d.PRIMARYHOURS == null)
		{
			d.PRIMARYHOURS = d.SPENT;
		}
		d.Store();
		DefectEvent.AddEventByTask(d.IDREC, DefectEvent.Eventtype.QualityAssurance, user.ID, "Changed disposition to " + dispo);
		await Defect.UnLocktaskAsync(user.TRID.ToString(), currentlock);

		var settings = Settings.CurrentSettings;
		if (d.ID.ToString() == settings.RELEASETTID)
		{
			if (dispo == GlobalDispo.testStarted)
			{
				VersionBuilder.SendAlarm("✅Release build has been finished. Testing is starting...");
			}
			else
			{
				VersionBuilder.SendAlarm("❌Failed to build version. Please check the logs!!!");
			}
		}
		else
		{
			DefectUser du = new DefectUser(d.AUSER);
			MPSUser worker = new MPSUser(du.TRID);
			var attr = dispo.GetAttributeOfType<DisplayAttribute>();
			TasksBot.SendMessage(worker.CHATID, $"{attr.Description}. The task tests have been marked as {dispRef.Descriptor} by automation system. {settings.GetTTAnchor(ttid, attr.Name)}");
			if (dispo == GlobalDispo.testStarted)
			{
				string mess = $"New task from {user.FULLNAME} is ready for tests!{settings.GetTTAnchor(ttid, d.FIRE ? "taskfire.png" : "")}";
				await TestChannel.SendMessageAsync(mess);
			}
		}
		return true;
	}
}
