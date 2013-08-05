using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class CreateEvent : EventCommand
   {
      #region Methods

      protected override void Execute(EventCommandContext context)
      {
         if (context.EventList == null || context.Event == null)
         {
            Log.Error("Calendar Module: command - cannot find calendar", null);
            return;
         }

         if (string.IsNullOrEmpty(context.Event.EndDate))
         {
            context.Event.EndDate = context.Event.StartDate;
         }

         if (string.IsNullOrEmpty(context.Event.EndTime))
         {
            context.Event.EndTime = Utilities.GetDefaultEndTime(context.Event.StartTime);
         }

         if (context.Schedule == null)
         {
            Create(context.Event, context.EventList, context.Branch);
         }
         else
         {
            CalendarActions.CreateSchedule(context.Event, context.Schedule, context.Options);
         }
      }

      private void Create(Event eventItem, EventList calendar, BranchItem branch)
      {
         if (eventItem != null || calendar != null)
         {
            Item root = StaticSettings.EventTargetDatabase.GetItem(calendar.ID);

            if (root == null || SecurityManager.CanWrite(root) != true)
            {
               return;
            }

            Item parent = StaticSettings.EventTargetDatabase.GetItem(calendar.ID);

            if (parent != null)
            {
               parent = Utilities.CreateDatePath(parent, eventItem.StartDate);
            }

            if (SecurityManager.CanWrite(parent) != true || parent == null)
            {
               return;
            }

            Item child = parent.Add(Utilities.Sanitize(eventItem.Title), branch);

            eventItem.ID = child.ID.ToString();

            eventItem.Save();

            PublishUtil.Publishing(eventItem.GetTargetItem(), true, false);
         }
      }

      #endregion
   }
}