using System;

using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Links;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Handlers
{
   public class EventItemHander
   {
      protected void OnItemMoved(object obj, EventArgs args)
      {
         Item item = Event.ExtractParameter(args, 0) as Item;

         if (item == null || StaticSettings.EventTemplate == null || item.TemplateID != StaticSettings.EventTemplate.ID)
         {
            return;
         }

         item.Editing.BeginEdit();

         if (Utilities.ValidateEventStructure(item))
         {
            DateTime itemDate = Utilities.GetDateForEvent(item);
            string startDate = DateUtil.ToIsoDate(itemDate);
            item.Fields[Objects.Event.StartDateField].Value = startDate;
            item.Fields[Objects.Event.EndDateField].Value = startDate;
         }

         item.Editing.EndEdit(true, false);

         Log.Info("Calendar Module: event is moved", item);
      }

      protected void OnItemSaved(object obj, EventArgs args)
      {
         Item item = Event.ExtractParameter(args, 0) as Item;

         try
         {
            if (item == null || Utilities.IsDateMappedToStruct(item))
            {
               return;
            }

            if (StaticSettings.EventTemplate != null && item.TemplateID == StaticSettings.EventTemplate.ID &&
                item.Fields[Objects.Event.StartDateField] != null &&
                item.Fields[Objects.Event.StartDateField].Value != string.Empty)
            {
               string contentPath = Utilities.GetActualDatePath(item);

               if (contentPath == string.Empty)
               {
                  return;
               }

               Item root = item.Database.GetItem(contentPath);
               if (root == null)
               {
                  root = Utilities.CreateDatePath(item.Parent.Parent.Parent.Parent, contentPath);
               }

               if (root != null)
               {
                  item.MoveTo(root);

                  if (item.Fields[Objects.Event.StartDateField] != null &&
                      !string.IsNullOrEmpty(item.Fields[Objects.Event.StartDateField].Value) &&
                      item.Fields[Objects.Event.EndDateField] != null)
                  {
                     item.Editing.BeginEdit();
                     item.Fields[Objects.Event.EndDateField].Value = item.Fields[Objects.Event.StartDateField].Value;
                     item.Editing.EndEdit();
                  }

                  Log.Info("Calendar Module: event is saved", item);
               }
            }
         }
         catch
         {
         }
      }
   }
}