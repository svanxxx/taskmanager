using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class DefectService : WebService
{
	public DefectService() { }
	[WebMethod(EnableSession = true)]
	public string settask(Defect d)
	{
		CurrentContext.Validate();
		Defect dstore = new Defect(d.ID);
		if (d.ORDER != dstore.ORDER)
		{
			//copy object specifics for multiple savings from same page: only order change should be processed.
			d.BACKORDER = dstore.BACKORDER;
		}
		dstore.FromAnotherObject(d);
		dstore.REQUESTRESET = d.REQUESTRESET;
		if (dstore.IsModified())
		{
			dstore.Store();
		}
		return "OK";
	}
	private DefectBase ChangeDispo(int ttid, string disp)
	{
		Defect d = new Defect(ttid);

		if (d.DISPO == disp)//already set
		{
			return new DefectBase(ttid);
		}

		if (Defect.Locked(ttid.ToString()))
		{
			return null;
		}

		d.DISPO = disp;
		if (Convert.ToInt32(d.DISPO) == DefectDispo.GetWorkingRec())
		{
			if (d.ORDER < 1)
			{
				d.ORDER = 1;
			}
		}
		d.Store();
		return new DefectBase(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public DefectBase setTaskDispo(int ttid, string disp)
	{
		CurrentContext.Validate();
		return ChangeDispo(ttid, disp);
	}
	[WebMethod(EnableSession = true)]
	public List<DefectEvent> gettaskevents(string ttid)
	{
		CurrentContext.Validate();
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectEvent.GetEventsByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectEventDefect> getDayEvents(string date)
	{
		CurrentContext.Validate();
		List<DefectEventDefect> res = new List<DefectEventDefect>();
		if (string.IsNullOrEmpty(date))
			return res;
		DateTime dt = DateTime.ParseExact(date, IdBasedObject.defDateFormat, CultureInfo.InvariantCulture);
		foreach (var i in DefectEvent.GetEventsByDay(dt, CurrentContext.TTUSERID))
		{
			res.Add(new DefectEventDefect(i));
		}
		return res;
	}
	[WebMethod(EnableSession = true)]
	public void delEvent(int id)
	{
		CurrentContext.Validate();
		DefectEvent.Delete(id);
		NotifyHub.NotifyPlanChange(CurrentContext.UserID);
	}
	[WebMethod(EnableSession = true)]
	public void spendEvent(int id, int hrs)
	{
		CurrentContext.Validate();
		DefectEvent de = new DefectEvent(id);
		de.TIME = hrs.ToString();
		de.Store();
		NotifyHub.NotifyPlanChange(CurrentContext.UserID);
	}
	[WebMethod(EnableSession = true)]
	public DefectBase startEvent(int ttid, string disp, int hrs, string date)
	{
		CurrentContext.Validate();
		DateTime dt = DateTime.ParseExact(date, IdBasedObject.defDateFormat, CultureInfo.InvariantCulture);
		int id = Defect.GetIDbyTT(ttid);
		DefectEvent.AddEventByTask(id, DefectEvent.Eventtype.worked, CurrentContext.TTUSERID, "I have worked on this task", hrs, -1, dt);
		NotifyHub.NotifyDefectChange(id);
		return ChangeDispo(ttid, disp);
	}
	[WebMethod]
	public DefectBase versionChange(int ttid, string version)
	{
		int id = Defect.GetIDbyTT(ttid);
		DefectEvent.AddEventByTask(id, DefectEvent.Eventtype.versionIncluded, CurrentContext.TTUSERID, version, -1, -1, null);
		return new DefectBase(ttid);
	}
	[WebMethod(EnableSession = true)]
	public int estimateDefect(int ttid, int estim)
	{
		CurrentContext.Validate();
		Defect d = new Defect(ttid);
		if (Defect.Locked(ttid.ToString()))
		{
			return d.ESTIM;
		}
		d.ESTIM = estim;
		d.Store();
		return d.ESTIM;
	}
}