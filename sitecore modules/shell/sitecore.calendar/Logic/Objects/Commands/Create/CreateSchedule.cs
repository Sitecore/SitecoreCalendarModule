using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class CreateSchedule : EventCommand
   {
      #region Methods

      protected override void Execute(EventCommandContext context)
      {
         if (context.Schedule != null && context.EventListManager != null)
         {
            if (context.Schedule.IsNew)
            {
               context.Schedule.StartDate = context.Event.StartDate;
               context.Schedule.EndDate = context.Event.EndDate;

               Create(context.Schedule, context.Event, context.EventListManager);
            }

            CalendarActions.CreateRecurrence(context.Event, context.Schedule, context.Options);

            DeleteEmptyScheduler(context.Schedule);
         }
      }

      private void DeleteEmptyScheduler(Schedule schedule)
      {
         if (schedule != null && schedule.ID != ID.Null.ToString())
         {
            CalendarActions.DeleteScheduler(schedule);
         }
      }

      private static void Create(Schedule schedule, Event evt, EventListManager mgr)
      {
         if (schedule != null)
         {
            Item root = StaticSettings.EventTargetDatabase.GetItem(mgr.SiteSettings.SchedulesRoot.ID);
            if (root != null)
            {
               using (new SecurityDisabler())
               {
                  Item newSched =
                     root.Add(string.Join("", new[] {EventListManager.SchedulePrefix, "-", evt.Title}),
                        StaticSettings.ScheduleTemplate);

                  schedule.SaveRecurrence(newSched);
                  schedule.ID = newSched.ID.ToString();

                  PublishUtil.Publishing(newSched, false, false);
               }
            }
         }
      }

      #endregion
   }
}