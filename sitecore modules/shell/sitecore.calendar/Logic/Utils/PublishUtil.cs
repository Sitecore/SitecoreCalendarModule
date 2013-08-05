using System.Collections;
using System.Linq;

using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Publishing;

namespace Sitecore.Modules.EventCalendar.Utils
{
   using System;

   using Events;

   public class PublishUtil
   {
      #region Methods

      public static void Publishing(Item eventItem, bool refreshEventPath, bool deleteItem)
      {
         if (StaticSettings.EventTargetDatabase.Database != StaticSettings.EventSourceDatabase.Database)
         {
            var databases = GetTargets(StaticSettings.EventTargetDatabase.Database);

            if (refreshEventPath)
            {
               if (StaticSettings.EventSourceDatabase.GetItem(eventItem.Parent.Parent.Parent.ID) == null)
               {
                  PublishManager.WaitFor(AutoPublish(eventItem.Parent.Parent.Parent, databases, false, true));
               }
               if (StaticSettings.EventSourceDatabase.GetItem(eventItem.Parent.Parent.ID) == null)
               {
                  PublishManager.WaitFor(AutoPublish(eventItem.Parent.Parent, databases, false, true));
               }
               if (StaticSettings.EventSourceDatabase.GetItem(eventItem.Parent.ID) == null)
               {
                  PublishManager.WaitFor(AutoPublish(eventItem.Parent, databases, false, true));
               }
            }

            //if (deleteItem)
            //{
            //   foreach (var database in databases)
            //   {
            //      Item item = database.GetItem(eventItem.ID);
            //      item.Delete();                  
            //   }
            //}
            //else
            //{
               PublishManager.WaitFor(AutoPublish(eventItem, databases, false, false));
            //}

            var publisher = new Publisher( 
                                    new PublishOptions(
                                    StaticSettings.EventTargetDatabase.Database,
                                    StaticSettings.EventSourceDatabase.Database,
                                    PublishMode.SingleItem,
                                    eventItem.Language,
                                    DateTime.Now));
            
         }
      }

      public static Handle AutoPublish(Item item, Database[] targets, bool deep, bool skipIfExist)
      {
         if (skipIfExist)
         {
            var databases = from database in targets
                            where database.GetItem(item.ID) == null
                            select database;

            targets = databases.ToArray();
         }

         LanguageCollection languages = LanguageManager.GetLanguages(item.Database);
         if ((languages == null) || (languages.Count == 0))
         {
            Log.Warn("No languages were found for publishing.", new string[0]);
         }

         return PublishManager.PublishItem(item, targets, languages.ToArray(), deep, true);
      }

      #endregion

      #region Private Methods

      private static Database[] GetTargets(Database database)
      {
         Item itemNotNull = database.SelectSingleItem("/sitecore/system/publishing targets");
         var list = new ArrayList();
         foreach (Item item2 in itemNotNull.Children)
         {
            string name = item2["Target database"];
            Database targetDatabase = Factory.GetDatabase(name, false);
            if (targetDatabase != null)
            {
               list.Add(targetDatabase);
            }
         }
         return Assert.ResultNotNull(list.ToArray(typeof(Database)) as Database[]);
      }


      #endregion

   }
}