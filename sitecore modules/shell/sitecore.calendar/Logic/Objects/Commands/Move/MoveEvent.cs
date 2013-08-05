using System;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Modules.EventCalendar.Exceptions;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class MoveEvent : Command
   {
      #region Methods

      public override void Execute(CommandContext context)
      {
         if (context is MoveEventContext)
         {
            Execute((MoveEventContext) context);
         }       
      }

      protected virtual void Execute(MoveEventContext context)
      {
         Item item = StaticSettings.EventTargetDatabase.GetItem(ID.Parse(context.EventID));

         if (item == null)
         {
            Log.Error("Calendar Module: WebService - cannot find event", null);
            return;
         }

         if (SecurityManager.CanWrite(item) != true)
         {
            return;
         }

         if (item.TemplateID != StaticSettings.EventTemplate.ID)
         {
            throw new EventCalendarException(
               String.Format(ResourceManager.Localize("UNSUPPORT_TEMPLATE"), item.Name, item.TemplateName,
                             StaticSettings.EventTemplate.Name));
         }

         item.Editing.BeginEdit();
         item.Fields[Event.StartDateField].Value = DateUtil.ToIsoDate(Utilities.StringToDate(context.NewDate));

         if (Utilities.StringToDate(context.NewDate) > Utilities.IsoStringToDate(item.Fields[Event.EndDateField].Value))
         {
            item.Fields[Event.EndDateField].Value = item.Fields[Event.StartDateField].Value;
         }

         item.Fields[Event.ScheduleIDField].Value = string.Empty;

         item.Editing.EndEdit();

         PublishUtil.Publishing(item, true, false);
      }

      #endregion
   }
}