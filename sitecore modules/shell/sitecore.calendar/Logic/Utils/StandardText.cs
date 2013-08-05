using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Core;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public static class StandardText
   {
      public static string Get(ID id)
      {
         Item item = StaticSettings.ContextDatabase.GetItem(id);

         if (item != null)
         {
            return item["text"];
         }

         return string.Empty;
      }

      public static string Get(string text_label)
      {
         Item metaData = StaticSettings.ContextDatabase.GetItem(CalendarIDs.MetaDataFolder);

         if (metaData != null)
         {
            Item textItem = metaData.Children[text_label];

            if (textItem != null)
            {
               return textItem["text"];
            }
         }

         return string.Empty;
      }
   }
}