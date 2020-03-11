public class DefectControl : System.Web.UI.UserControl
{
	public string ProxiAttribute(string att)
	{
		return Attributes[att];
	}
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
			string key = k.ToString();
			if (key == "ng-show" || key == "style")
			{
				outstr += $" {key}='{Attributes[key]}' ";
			}
		}
		return outstr;
	}
}