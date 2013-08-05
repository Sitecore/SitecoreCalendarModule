using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Modules.EventCalendar.Exceptions;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class MoveEventTime : Command
   {
      #region Methods

      public override void Execute(CommandContext context)
      {
         if (context is MoveEventTimeContext)
         {
            Execute((MoveEventTimeContext)context);
         }
      }

      protected virtual void Execute(MoveEventTimeContext context)
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
               String.Format(
                  String.Format(ResourceManager.Localize("UNSUPPORT_TEMPLATE"), item.Name, item.TemplateName,
                                StaticSettings.EventTemplate.Name)));
         }

         item.Editing.BeginEdit();

         if (!string.IsNullOrEmpty(context.StartTime))
         {
            item.Fields[Event.StartTimeField].Value = Utilities.NormalizeTime(context.StartTime);
         }
         if (!string.IsNullOrEmpty(context.EndTime))
         {
            item.Fields[Event.EndTimeField].Value = Utilities.NormalizeTime(context.EndTime);
         }

         item.Editing.EndEdit();

         Event evt = new Event(item);

         if (evt.ScheduleID != null)
         {
            if (context.UpdateSeries)
            {
               SyncEventTimes(GetEventSet(evt, context.UpdateSeries), evt);
            }
            else
            {
               item.Editing.BeginEdit();
               item.Fields[Event.ScheduleIDField].Value = string.Empty;
               item.Editing.EndEdit();
            }
         }

         PublishUtil.Publishing(item, false, false);
      }

      private void SyncEventTimes(IEnumerable<Event> events, Event pattern)
      {
         Item item = null;
         foreach (var evetnItem in events)
         {
            item = evetnItem.GetTargetItem();
            item.Editing.BeginEdit();
            item.Fields[Event.StartTimeField].Value = pattern.StartTime;
            item.Fields[Event.EndTimeField].Value = pattern.EndTime;
            item.Editing.EndEdit();
            PublishUtil.Publishing(item, false, false);
         }
      }

      private IEnumerable<Event> GetEventSet(Event evt, bool updateSeries)
      {
         Schedule schedule = evt.GetSchedule();

         List<Event> events = new List<Event>();
         if (schedule != null && updateSeries)
         {
            events.AddRange(schedule.GetTargetEvents());
         }

         return events;
      }

      #endregion

   }
}
