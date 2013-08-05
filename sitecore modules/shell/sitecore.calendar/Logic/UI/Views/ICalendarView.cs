using System;
using System.Web.UI;

namespace Sitecore.Modules.EventCalendar.UI
{
   public interface ICalendarView
   {
      DateTime CurrentDate
      {
         get;
         set;
      }

      Control RenderHeader();
   }
}