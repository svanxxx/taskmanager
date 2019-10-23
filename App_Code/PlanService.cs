using System;
using System.Collections.Generic;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]

public class PlanService : WebService
{
    public PlanService() { }
    [WebMethod(EnableSession = true)]
    public List<DefectPlan> getplanned(string userid)
    {
        if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
            return null;

        DefectBase d = new DefectBase();
        return DefectPlan.Convert2Plan(d.EnumPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid)));
    }
    [WebMethod(EnableSession = true)]
    public List<DefectPlan> getplannedShort(string userid)
    {
        if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
            return null;

        DefectBase d = new DefectBase();
        return DefectPlan.Convert2Plan(d.EnumPlanShort(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid)));
    }
    [WebMethod(EnableSession = true)]
    public List<DefectPlan> getunplanned(string userid)
    {
        if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
            return null;

        DefectBase d = new DefectBase();
        return DefectPlan.Convert2Plan(d.EnumUnPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid)));
    }
}