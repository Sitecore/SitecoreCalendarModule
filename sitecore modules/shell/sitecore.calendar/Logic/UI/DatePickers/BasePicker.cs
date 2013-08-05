using System;
using System.Web;

using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
   public abstract class BasePicker : AjaxControl
   {
      #region Attributes

      private string _title = string.Empty;

      #endregion

      #region Accessor Methods

      public string Title
      {
         get
         {
            return _title;
         }
         set
         {
            _title = value;
         }
      }

      protected virtual DateTime CurrentDate
      {
         get
         {
            if (HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate] == null)
            {
               HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate] = DateTime.Now;
               return DateTime.Now;
            }

            return (DateTime) HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate];
         }
         set
         {
            HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate] = value;
         }
      }

      #endregion
   }
}