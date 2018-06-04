using GTOHELPER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Master : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Control c = FindControl("logina");
		LiteralControl l = (LiteralControl)c.Controls[0];
		if (CurrentContext.User != null)
		{
			l.Text = l.Text.Replace("-Login-", CurrentContext.User.PERSON_NAME);
		}
	}
}