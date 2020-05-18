using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class NetworkScanner
{
	public class MacIpPair
	{
		public string MacAddress;
		public string IpAddress;
		public string Name;
	}
	static public ConcurrentDictionary<long, MacIpPair> GetAllMAchines(string subnetPrefix = "192.168.0.")
	{
		ConcurrentDictionary<long, MacIpPair> mip = new ConcurrentDictionary<long, MacIpPair>();
		System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
		pProcess.StartInfo.FileName = "arp";
		pProcess.StartInfo.Arguments = "-a ";
		pProcess.StartInfo.UseShellExecute = false;
		pProcess.StartInfo.RedirectStandardOutput = true;
		pProcess.StartInfo.CreateNoWindow = true;
		pProcess.Start();
		string cmdOutput = pProcess.StandardOutput.ReadToEnd();
		string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

		foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
		{
			string ip = m.Groups["ip"].Value;
			if (!ip.StartsWith(subnetPrefix))
			{
				continue;
			}
			mip[mip.Count] = new MacIpPair()
			{
				MacAddress = m.Groups["mac"].Value.ToUpper().Replace("-", string.Empty),
				IpAddress = m.Groups["ip"].Value
			};
		}

		Parallel.For(0, mip.Count, i => {
			string[] nums = mip[i].IpAddress.Split('.');
			byte[] numsB = new byte[nums.Length];
			for (int j = 0; j < nums.Length; j++)
			{
				numsB[j] = Convert.ToByte(nums[j]);
			}
			try
			{
				mip[i].Name = Dns.GetHostEntry(new IPAddress(numsB)).HostName.Split('.')[0];
			}
			catch (Exception) { }
		});

		return mip;
	}
}