using System;
using System.Text.RegularExpressions;

public class BodyProcessor
{
	private static readonly Regex regex = new Regex("((http://|www\\.)([A-Z0-9.-:]{1,})\\.[0-9A-Z?;~&#=\\-_\\./]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly string link = "<a href=\"{0}{1}\">{2}</a>";
	private static readonly string img = "<img src='{0}'/>";
	private static readonly string ttident = "SHOWTASK.ASPX?TTID=";
	private static readonly string wikiident = "FIELDPROWIKI/INDEX.PHP?TITLE=";
	private static readonly string imgident = "GETATTACH.ASPX?IDRECORD=";

	public BodyProcessor()
	{
	}
	public static string ResolveLinks(string body)
	{
		if (string.IsNullOrEmpty(body))
			return body;

		foreach (Match match in regex.Matches(body))
		{
			if (!match.Value.Contains("://"))
			{
				body = body.Replace(match.Value, string.Format(link, "http://", match.Value, ShortenUrl(match.Value, 50)));
			}
			else
			{
				string valUP = match.Value.ToUpper();
				if (valUP.Contains(imgident) && valUP.Contains("EXT=PNG"))
				{
					body = body.Replace(match.Value, string.Format(img, match.Value));
				}
				else
				{
					body = body.Replace(match.Value, string.Format(link, string.Empty, match.Value, ShortenUrl(match.Value, 50)));
				}
			}
		}

		return body;
	}
	private static string ShortenUrl(string url, int max)
	{
		string urlUp = url.ToUpper();
		if (urlUp.Contains(ttident))
		{
			string ttid = urlUp.Substring(urlUp.IndexOf(ttident) + ttident.Length);
			DefectBase d = new DefectBase(Convert.ToInt32(ttid));
			return "TT" + ttid + " " + d.SUMMARY;
		}
		if (urlUp.Contains(wikiident))
		{
			string page = urlUp.Substring(urlUp.IndexOf(wikiident) + wikiident.Length);
			return "WIKI: " + page.Replace("_", " ").Replace("#", " - ");
		}
		if (urlUp.Contains(imgident))
		{
			string page = urlUp.Substring(urlUp.IndexOf(wikiident) + wikiident.Length);
			return "Attachment: click to see.";
		}

		if (url.Length <= max)
			return url;

		// Remove the protocal
		int startIndex = url.IndexOf("://");
		if (startIndex > -1)
			url = url.Substring(startIndex + 3);

		if (url.Length <= max)
			return url;

		// Remove the folder structure
		int firstIndex = url.IndexOf("/") + 1;
		int lastIndex = url.LastIndexOf("/");
		if (firstIndex < lastIndex)
			url = url.Replace(url.Substring(firstIndex, lastIndex - firstIndex), "...");

		if (url.Length <= max)
			return url;

		// Remove URL parameters
		int queryIndex = url.IndexOf("?");
		if (queryIndex > -1)
			url = url.Substring(0, queryIndex);

		if (url.Length <= max)
			return url;

		// Remove URL fragment
		int fragmentIndex = url.IndexOf("#");
		if (fragmentIndex > -1)
			url = url.Substring(0, fragmentIndex);

		if (url.Length <= max)
			return url;

		// Shorten page
		firstIndex = url.LastIndexOf("/") + 1;
		lastIndex = url.LastIndexOf(".");
		if (lastIndex - firstIndex > 10)
		{
			string page = url.Substring(firstIndex, lastIndex - firstIndex);
			int length = url.Length - max + 3;
			url = url.Replace(page, "..." + page.Substring(length));
		}

		return url;
	}
}