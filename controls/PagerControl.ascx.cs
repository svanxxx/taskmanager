public partial class PagerControl : System.Web.UI.UserControl
{
	public string StateName()
	{
		string st = Attributes["statename"];
		if (string.IsNullOrEmpty(st))
		{
			return "state";
		}
		return st;
	}
}