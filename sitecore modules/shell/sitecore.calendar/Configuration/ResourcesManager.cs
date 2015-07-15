using Sitecore.Globalization;
using Sitecore.Modules.EventCalendar.Utils;
using System.Reflection;

namespace Sitecore.EventCalendar.Core.Configuration
{
   public class ResourceManager
   {
      private static System.Resources.ResourceManager _rm =
         new System.Resources.ResourceManager(StaticSettings.ResourceName, Assembly.GetExecutingAssembly());

      /// <summary>
      /// Localize String 
      /// </summary>
      public static string Localize(string resIdentifier)
      {
         return Translate.Text(_rm.GetString(resIdentifier));
      }
   }
}