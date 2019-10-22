using System;
using System.Collections.Generic;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]

public class PlanService : WebService
{
    public class DefectPlan
    {
        public DefectPlan() { }
        public DefectPlan(DefectBase db)
        {
            this.ID = db.ID;
            this.SUMMARY = db.SUMMARY;
            this.SMODTRID = db.SMODTRID;
            this.SMODIFIER = db.SMODIFIER;
            this.DISPO = int.Parse(db.DISPO);
            this.SPENT = db.SPENT;
            this.IDREC = db.IDREC;
            this.ESTIM = db.ESTIM;
            this.FIRE = db.FIRE;
        }
        public bool FIRE { get; set; }
        public int ESTIM { get; set; }
        public int IDREC { get; set; }
        public int SPENT { get; set; }
        public int SMODTRID { get; set; }
        public string SMODIFIER { get; set; }
        public int DISPO { get; set; }
        public int ID { get; set; }
        public string SUMMARY { get; set; }
    }

    public PlanService() { }
    List<DefectPlan> Convert2Plan(List<DefectBase> ls)
    {
        List<DefectPlan> lsout = new List<DefectPlan>();
        foreach (var def in ls)
        {
            lsout.Add(new DefectPlan(def));
        }
        return lsout;
    }
    [WebMethod(EnableSession = true)]
    public List<DefectPlan> getplanned(string userid)
    {
        if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
            return null;

        DefectBase d = new DefectBase();
        return Convert2Plan(d.EnumPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid)));
    }
    [WebMethod(EnableSession = true)]
    public List<DefectPlan> getplannedShort(string userid)
    {
        if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
            return null;

        DefectBase d = new DefectBase();
        return Convert2Plan(d.EnumPlanShort(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid)));
    }
    [WebMethod(EnableSession = true)]
    public List<DefectPlan> getunplanned(string userid)
    {
        if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
            return null;

        DefectBase d = new DefectBase();
        return Convert2Plan(d.EnumUnPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid)));
    }
}