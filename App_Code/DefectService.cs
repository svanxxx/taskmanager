using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class DefectService : WebService
{
	public DefectService(){}
	[WebMethod(EnableSession = true)]
	public string settask(Defect d)
	{
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
}