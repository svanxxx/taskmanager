using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTOHELPER;

public partial class Showtask : GTOHelper
{
	protected void Page_Load(object sender, EventArgs e)
	{
		for (int i = 0; i < Request.Files.Count; i++)
		{
			if (Request.Files[i].ContentLength > 0)
			{
				//save it
			}
		}
	}
}