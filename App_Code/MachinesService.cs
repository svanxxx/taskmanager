using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class MachinesService : WebService
{
	public MachinesService() { }
	[WebMethod(EnableSession = true)]
	public List<Machine> getMachines()
	{
		CurrentContext.Validate();
		return MachineFactory.Enum();
	}
	[WebMethod(EnableSession = true, CacheDuration = 120)]
	public List<string> getDomainComputers()
	{
		CurrentContext.Validate();

		List<string> ComputerNames = new List<string>();

		DirectoryEntry entry = new DirectoryEntry("LDAP://" + Settings.CurrentSettings.COMPANYDOMAIN);
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
	[WebMethod(EnableSession = true)]
	public void shutMachine(string m)
	{
		CurrentContext.Validate();
		Machine ma = MachineFactory.FindOrCreate(m);
		Process process = new Process();
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.FileName = "shutdown";
		process.StartInfo.Arguments = string.Format(@"/s /m \\{0} /t 0", ma.PCNAME);
		process.Start();
	}
	[WebMethod(EnableSession = true)]
	public void wakeMachine(string m)
	{
		CurrentContext.Validate();
		Machine mach = MachineFactory.FindOrCreate(m);
		if (string.IsNullOrEmpty(mach.MAC))
		{
			return;
		}
		string[] macs = mach.MAC.Split(' ');
		foreach (var mac in macs)
		{
			Process process = new Process();
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = HttpRuntime.AppDomainAppPath + "bin\\WolCmd.exe";
			process.StartInfo.Arguments = mac + " 192.168.0.1 255.255.255.0 3";
			process.Start();
		}
	}
	[WebMethod(EnableSession = true)]
	public void scanMachine(string m)
	{
		CurrentContext.Validate();
		try
		{
			Machine ma = MachineFactory.FindOrCreate(m);
			string scope = string.Format("\\\\{0}\\root\\CIMV2", ma.PCNAME);
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, "SELECT * FROM Win32_NetworkAdapterConfiguration");
			string newmac = "";
			string sIPAddress = "";
			foreach (ManagementObject queryObj in searcher.Get())
			{
				object o = queryObj["MACAddress"];
				if (o == null)
				{
					continue;
				}
				if (string.IsNullOrEmpty(newmac))
					newmac = o.ToString().Replace(":", "");
				else
					newmac += " " + o.ToString().Replace(":", "");

				string[] arrIPAddress = (string[])(queryObj["IPAddress"]);
				if (arrIPAddress != null)
				{
					sIPAddress = arrIPAddress.FirstOrDefault(s => s.Contains('.'));
					if (!string.IsNullOrEmpty(sIPAddress))
					{
						ma.IP = sIPAddress;
					}
				}
			}

			var cpu = new ManagementObjectSearcher(scope, "select * from Win32_Processor").Get().Cast<ManagementObject>().First();
			var wmi = new ManagementObjectSearcher(scope, "select * from Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
			if (!string.IsNullOrEmpty(newmac))
			{
				int memory = Convert.ToInt32(wmi["TotalVisibleMemorySize"]) / 1024;
				ma.DETAILS = wmi["Caption"].ToString() + "<br/>" + cpu["Name"].ToString() + "<br/>" + memory.ToString() + "Mb";
				ma.MAC = newmac;
				MachineFactory.Update(ma);
			}
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
	[WebMethod(EnableSession = true)]
	public void catMachine(string m, string category)
	{
		if (string.IsNullOrEmpty(m))
		{
			return;
		}
		CurrentContext.Validate();
		Machine ms = MachineFactory.FindOrCreate(m);
		ms.CATEGORY = category;
		MachineFactory.Update(ms);
	}
	[WebMethod(EnableSession = true)]
	public void remMachine(string m)
	{
		CurrentContext.Validate();
		MachineFactory.Delete(m);
	}
	[WebMethod(EnableSession = true, CacheDuration = 30)]
	public List<Machine> scanAllMachines()
	{
		CurrentContext.Validate();
		var ips = NetworkScanner.GetAllMAchines();
		foreach (int i in ips.Keys)
		{
			var ip = ips[i];
			if (string.IsNullOrEmpty(ip.Name))
			{
				continue;
			}
			var m = MachineFactory.FindOrCreate(ip.Name.ToUpper());
			bool bchange = false;
			if (m.IP != ip.IpAddress)
			{
				m.IP = ip.IpAddress;
				bchange = true;
			}
			if (m.MAC != ip.MacAddress)
			{
				m.MAC = ip.MacAddress;
				bchange = true;
			}
			if (bchange)
			{
				MachineFactory.Update(m);
			}
		}
		return getMachines();
	}
	[WebMethod(EnableSession = true, CacheDuration = 30)]
	public List<Machine> reScanMachines()
	{
		CurrentContext.Validate();
		var ips = NetworkScanner.GetAllMAchines();
		foreach (int i in ips.Keys)
		{
			var ip = ips[i];
			if (string.IsNullOrEmpty(ip.Name))
			{
				continue;
			}
			var m = MachineFactory.Find(ip.Name.ToUpper());
			if (m != null)
			{
				bool bchange = false;
				if (m.IP != ip.IpAddress)
				{
					m.IP = ip.IpAddress;
					bchange = true;
				}
				if (m.MAC != ip.MacAddress)
				{
					m.MAC = ip.MacAddress;
					bchange = true;
				}
				if (bchange)
				{
					MachineFactory.Update(m);
				}
			}
		}
		return getMachines();
	}
}