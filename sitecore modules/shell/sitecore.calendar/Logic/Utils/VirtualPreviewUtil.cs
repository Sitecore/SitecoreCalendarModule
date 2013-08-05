using System.Web.UI;

namespace Sitecore.Modules.EventCalendar.sitecore_modules.shell.sitecore.calendar.Logic.Utils
{
	public class VirtualPreviewUtil
	{
		public static bool IsVirtualPreview(Page page)
		{
			if (page != null)
			{
				if (page.GetType().FullName.EndsWith("palette_aspx"))// == "ASP.sitecore_shell_virtualpreviewcalendar_aspx"))
				{
					return true;
				}
			}

			return false;
		}
	}
}
