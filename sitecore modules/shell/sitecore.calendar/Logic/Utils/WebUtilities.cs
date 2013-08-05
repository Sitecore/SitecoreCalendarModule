using System.Web.UI;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public static class WebUtilities
   {
      private static readonly string bodyTag = "<body";
      private static readonly string onloadScript = "onload='";

      public static void InsertOnLoadCode(ControlCollection controls, string method)
      {
         foreach (Control control in controls)
         {
            LiteralControl literal = control as LiteralControl;
            if (literal != null)
            {
               int bodyPos = literal.Text.IndexOf(bodyTag);
               if (bodyPos > -1)
               {
                  int onloadPos = literal.Text.IndexOf(onloadScript, bodyPos);
                  if (onloadPos > -1)
                  {
                     literal.Text = literal.Text.Insert(onloadPos + onloadScript.Length, method);
                  }
                  else
                  {
                     literal.Text =
                        literal.Text.Insert(bodyPos + bodyTag.Length, string.Format(" onload='{0}'", method));
                  }
                  break;
               }
            }
         }
      }
   }
}