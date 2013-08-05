using System.Web;

using Sitecore.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel.License;

namespace Sitecore.Modules.EventCalendar
{
   public static class SecurityManager
   {
      public static bool IsLoggedIn
      {
         get
         {
            return Context.IsLoggedIn;
         }
      }

      public static string UserName
      {
         get
         {
            if (IsLoggedIn != true)
            {
               return string.Empty;
            }

            User usr = Context.User;

            if (usr == null)
            {
               return string.Empty;
            }

            return usr.Name;
         }
      }

      public static bool CanRead(Item item)
      {
         if (item == null)
         {
            return false;
         }

         return item.Access.CanRead();
      }

      public static bool CanWrite(Item item)
      {
         if (item == null)
         {
            return false;
         }

         return item.Access.CanWrite();
      }

      internal static void Init()
      {
         if (!(License.HasModule("Sitecore.Calendar") || License.HasModule("Sitecore.Express")))
         {
            HttpContext.Current.Response.Redirect("/sitecore/nolicense.aspx?license=Sitecore.Calendar");
         }
      }
   }
}