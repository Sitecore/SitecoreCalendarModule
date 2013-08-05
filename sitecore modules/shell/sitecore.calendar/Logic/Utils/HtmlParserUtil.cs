using System;

namespace Sitecore.Modules.EventCalendar.sitecore_modules.shell.sitecore.calendar.Logic.Utils
{
	using System.Text.RegularExpressions;

	public class HtmlParserUtil
	{
		public static string RemoveAllOnFunctions(string html)
		{
			html = Regex.Replace(html, "on\\w*=\"(.+?)\"", String.Empty);
			html = Regex.Replace(html, "on\\w*='(.+?)'", String.Empty);

			return html;
		}

		public static string RemoveAllHrefAttributesFromLinks(string html)
		{
			html = Regex.Replace(html, "<a href=\"(.+?)\"", "<a");

			return html;
		}

		public static string RemoveFormTagById(string html, string formId)
		{
			string pattern = String.Format("<form.*?(id=\"{0}\")>", formId);
			html = Regex.Replace(html, pattern, String.Empty);
			html = Regex.Replace(html, "</form>", String.Empty);

			return html;
		}
	}
}
