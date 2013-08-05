using System.Web.UI;

using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
	using sitecore_modules.shell.sitecore.calendar.Logic.UI;
	using sitecore_modules.shell.sitecore.calendar.Logic.Utils;

	public abstract class AjaxControl : CalendarWebControl
   {
      protected virtual bool initAjax()
      {
				if (!VirtualPreviewUtil.IsVirtualPreview(this.Page))
				{
					ScriptManager scriptManager = AjaxManager.Current;
					if (AjaxManager.Current == null)
					{
						scriptManager = new ScriptManager();
						scriptManager.ID = StaticSettings.ScriptManagerID;

						Controls.Add(scriptManager);
					}

					if (!scriptManager.EnablePartialRendering)
					{
						scriptManager.EnablePartialRendering = true;
					}

					if (!AjaxManager.ContainsService(scriptManager, StaticSettings.ServiceReference))
					{
						scriptManager.Services.Add(new ServiceReference(StaticSettings.ServiceReference));
					}
				}

      	return true;
      }
   }
}