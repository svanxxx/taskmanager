using System;

public partial class CommitsControl : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Pager.Attributes["statename"] = DatasetName() + "state";
	}
	
	public string Hidden()
	{
		string st = Attributes["hide"];
		if (st == null)
		{
			return "false";
		}
		return "true";
	}
	public string DatasetName()
	{
		string st = Attributes["dataset"];
		if (string.IsNullOrEmpty(st))
		{
			return "commits";
		}
		return st;
	}
}