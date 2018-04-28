using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTOHELPER;
using System.IO;

public partial class VersionChanges : GTOHelper
{
    protected void Page_Load(object sender, EventArgs e)
    {
		VersionLabel.Text = "";
		string[] lines = File.ReadAllLines(Server.MapPath("~/ChangeLog/") + "changelog.txt");		
		for (int i = lines.Length - 1; i >= 0; i--)
		{
			VersionLabel.Text += lines[i] + "<br/>";
			if (i < lines.Length - 256)
				break;
		}		
    }
}