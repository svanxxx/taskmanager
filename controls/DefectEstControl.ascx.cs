public partial class DefectEstControl : System.Web.UI.UserControl
{
	public string Member()
	{
		string st = Attributes["member"];
		if (string.IsNullOrEmpty(st))
		{
			return "d";
		}
		return st;
	}
}