using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using Sitecore.Diagnostics;
using Sitecore.Web;

namespace Sitecore.Modules.EventCalendar.Logic.Utils
{
   public class AjaxManager
   {
      #region Methods

      public static bool ContainsService(ScriptManager manager, string service)
      {
         Assert.ArgumentNotNull(manager, "manager");
         Assert.ArgumentNotNullOrEmpty(service, "service");

         return manager.Services.FirstOrDefault(s => s.Path == service) != null;
      }

      #endregion


      #region Properties

      public static ScriptManager Current
      {
         get
         {
            if (Context.Page != null && Context.Page.Page != null)
            {
               return ScriptManager.GetCurrent(Context.Page.Page);
            }
            return null;
         }
      }

      #endregion

   }
}
