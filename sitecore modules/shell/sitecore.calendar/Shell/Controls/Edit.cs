using System;

namespace Sitecore.Modules.EventCalendar.Shell.Controls
{
   public class Edit : Web.UI.HtmlControls.Edit
   {
      public EventHandler OnChanged;

      protected override void OnPreRender(EventArgs e)
      {
         base.OnPreRender(e);
         Style["width"] = "100%";
         Attributes.Add("onblur", Sitecore.Context.ClientPage.GetClientEvent(ID + ".PlaceholderChange"));
      }

      protected virtual void PlaceholderChange()
      {
         if (OnChanged != null)
         {
            OnChanged(this, null);
         }
      }
   }
}