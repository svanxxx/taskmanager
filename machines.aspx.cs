using System;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using GTOHELPER;
using System.Data;
using System.Diagnostics;

public partial class Machines : GTOHelper
{
	string Machine
	{
		get
		{
			object res = Request.QueryString["machine"];
			if (res == null)
				return "";
			return res.ToString();
		}
	}
	private void WakeFunction(string MAC_ADDRESS)
	{
		Process process = new Process();
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.FileName = HttpRuntime.AppDomainAppPath + "bin\\WolCmd.exe";
		process.StartInfo.Arguments = MAC_ADDRESS + " 192.168.0.1 255.255.255.0 3";
		process.Start();
	}
	private void ShutDownFunction(string pc)
	{
		Process process = new Process();
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.FileName = "shutdown";
		process.StartInfo.Arguments = string.Format(@"/s /m \\{0} /t 0", pc);
		process.Start();
	}
	protected System.Web.UI.Control FindControlbyID(string ID, System.Web.UI.Control parent = null)
	{
		if (parent == null)
		{
			parent = this;
			return FindControlbyID(ID, parent);
		}

		foreach (System.Web.UI.Control ctrl in parent.Controls)
		{
			if (ctrl.ID == ID)
				return ctrl;

			System.Web.UI.Control ct = FindControlbyID(ID, ctrl);
			if (ct != null)
				return ct;
		}
		return null;
	}
	protected void AddCtrl(string txt)
	{
		System.Web.UI.Control ct = FindControlbyID("maindiv");
		if (ct == null)
			return;

		Literal l = new Literal();
		l.Text = txt;
		ct.Controls.Add(l);
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		/*
		if (string.IsNullOrEmpty(Machine))
		{
			hint.InnerHtml = "Select Machine:";

			if (Request.QueryString["find"] != null)
			{
				List<string> ComputerNames = GetComputers();
				foreach (string pc in ComputerNames)
				{
					AddCtrl(string.Format("<button type = 'button' class='btn btn-block btn-primary btn-lg machine'>{0}</button>", pc));
				}
			}
			else
			{
				AddCtrl(string.Format("<button type = 'button' class='btn btn-block btn-success btn-lg find'>{0}</button>", "Find machines"));
				foreach (DataRow rowCur in GetDataSet("Select PCNAME from PCS").Tables[0].Rows)
				{
					AddCtrl(string.Format("<button type = 'button' class='btn btn-block btn-primary btn-lg machine'>{0}</button>", rowCur["PCNAME"].ToString()));
				}
			}
		}
		else
		{
			hint.InnerHtml = string.Format("Select Action For {0}:", Machine);

			AddCtrl("<button type = 'button' class='btn btn-block btn-primary btn-lg maction' id='wake'>Wake Up</button>");
			AddCtrl("<button type = 'button' class='btn btn-block btn-primary btn-lg maction' id='shut'>Shut Down</button>");
			AddCtrl("<button type = 'button' class='btn btn-block btn-primary btn-lg maction' id='scan'>Scan for MAC</button>");
			AddCtrl("<button type = 'button' class='btn btn-block btn-primary btn-lg maction' id='remove'>Remove</button>");
			AddCtrl("<button type = 'button' class='btn btn-block btn-primary btn-lg back'>Back</button>");

			if (Request.QueryString["scan"] != null)
			{
				SQLExecute(string.Format("INSERT INTO PCS (PCNAME) SELECT '{0}' WHERE NOT EXISTS(SELECT PCNAME FROM PCS WHERE PCNAME = '{1}')", Machine, Machine));
				string mac = GetMAC(Machine);
				if (!string.IsNullOrEmpty(mac))
				{
					SQLExecute(string.Format("UPDATE PCS set MAC = '{0}' WHERE PCNAME = '{1}'", GetMAC(Machine), Machine));
				}
			}
			else if (Request.QueryString["shut"] != null)
			{
				ShutDownFunction(Machine);
			}
			else if (Request.QueryString["remove"] != null)
			{
				SQLExecute(string.Format("DELETE FROM PCS WHERE PCNAME = '{0}'", Machine));
				Response.Redirect(CurrentPageName);
			}
			else if (Request.QueryString["wake"] != null)
			{
				foreach (DataRow rowCur in GetDataSet(string.Format("SELECT MAC FROM PCS WHERE PCNAME = '{0}'", Machine)).Tables[0].Rows)
				{
					string mac = rowCur["MAC"].ToString();
					foreach (string onemac in mac.Split(' '))
					{
						WakeFunction(onemac);
					}
				}
			}
		}
		*/
	}
	public static List<string> GetComputers()
	{
		List<string> ComputerNames = new List<string>();

		DirectoryEntry entry = new DirectoryEntry("LDAP://mps");
		DirectorySearcher mySearcher = new DirectorySearcher(entry);
		mySearcher.Filter = ("(objectClass=computer)");
		mySearcher.SizeLimit = int.MaxValue;
		mySearcher.PageSize = int.MaxValue;

		foreach (SearchResult resEnt in mySearcher.FindAll())
		{
			string ComputerName = resEnt.GetDirectoryEntry().Name;
			if (ComputerName.StartsWith("CN="))
				ComputerName = ComputerName.Remove(0, "CN=".Length);
			ComputerNames.Add(ComputerName);
		}

		mySearcher.Dispose();
		entry.Dispose();

		return ComputerNames;
	}
}