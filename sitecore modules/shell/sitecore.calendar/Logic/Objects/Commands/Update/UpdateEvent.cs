using System.Collections.Generic;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class UpdateEvent : Command
   {
      #region Methods

      public override void Execute(CommandContext context)
      {
         if (context is UpdateCommandContext)
         {
            Execute((UpdateCommandContext) context);
         }
      }

      protected virtual void Execute(UpdateCommandContext context)
      {
         Item item = StaticSettings.EventTargetDatabase.GetItem(context.Event.ID);

         Update(context, context.EventListManager.GetCalendar(context.Event), item.Branch);
      }

      private void Update(UpdateCommandContext context, EventList oldCalendar, BranchItem branch)
      {
         if (!SecurityManager.CanWrite(context.Event.GetTargetItem()))
         {
            return;
         }

         bool isClearSchedule = !context.UpdateSeries;

         if (context.Event.IsDateScopeChanged && oldCalendar.ID.ToString() == context.CalendarID && !context.UpdateSeries)
         {
            isClearSchedule = true;
            context.Event.ScheduleID = string.Empty;
            MoveEvent(context);
         }

         if (context.Event != null)
         {
            if (context.Schedule != null && !isClearSchedule)
            {
               context.Event.ScheduleID = context.Schedule.ID;
            }

            if ((context.Schedule != null && context.UpdateSeries &&
                (context.Schedule.IsChanged || context.Event.IsDateScopeChanged)) || 
                (context.UpdateSeries && oldCalendar.ID.ToString() != context.CalendarID))
            {
               this.UpdateAllSeriesTogetherWithSchedule(context, branch);
               return;
            }

            if (oldCalendar.ID.ToString() != context.CalendarID)
            {
               UpdateThisOccurrence(context, branch);
               return;
            }

            var events = new List<Event>();
            if (context.UpdateSeries && context.Schedule != null)
            {
               events.AddRange(context.Schedule.GetTargetEvents());
            }
            else
            {
               events.Add(new Event(context.Event.GetTargetItem()));
            }

            SyncEvents(context, events);
         }
      }

      public void MoveEvent(UpdateCommandContext context)
      {
         CalendarActions.MoveEvent(context.Event.ID, context.Event.StartDate);
      }

      public void SyncEvents(UpdateCommandContext context, IEnumerable<Event> events)
      {
         CalendarActions.SyncEvents(context.Event, events, context.EventList, context.EventListManager);
      }

      public void UpdateThisOccurrence(UpdateCommandContext context,
                                                BranchItem branch)
      {
         CalendarActions.DeleteEvent(context.Event.ID, false);
         CalendarActions.CreateEvent(context.Event, null,
                                             context.Options, branch);
      }

      public void UpdateAllSeriesTogetherWithSchedule(UpdateCommandContext context,
                                                      BranchItem branch)
      {
         CalendarActions.DeleteEvent(context.Event.ID, true);

         string eventEndDate = context.Event.EndDate;
         if (context.Event.EndDate != context.Schedule.EndDate &&
            context.Event.StartDate == context.Event.EndDate)
         {
            context.Event.EndDate = context.Schedule.EndDate;
         }

         if (context.Event.StartDate != context.Schedule.StartDate &&
             context.Event.StartDate == eventEndDate)
         {
            context.Event.StartDate = context.Schedule.StartDate;
         }

         CalendarActions.CreateEvent(context.Event, context.Schedule,
                                                     context.Options, branch);
      }

      #endregion
   }
}