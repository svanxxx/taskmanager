public partial class DefectOrdControl : System.Web.UI.UserControl
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
	public string Class()
	{
		string st = Attributes["class"];
		if (string.IsNullOrEmpty(st))
		{
			return "";
		}
		return st;
	}
	public string Attrs()
	{
		string outstr = "";
		foreach (var k in Attributes.Keys)
		{
			if (k.ToString() == "ng-show")
			{
				outstr += $" {k.ToString()}='{Attributes[k.ToString()]}' ";
			}
		}
		return outstr;
	}
}