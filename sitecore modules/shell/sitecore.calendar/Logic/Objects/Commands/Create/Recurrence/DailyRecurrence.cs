using System;

using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class DailyRecurrence : EventCommand
   {
      #region Methods

      protected override void Execute(EventCommandContext context)
      {
         addDaily(Utilities.StringToDate(context.Schedule.StartDate), context);
      }

      private static void addDaily(DateTime start, EventCommandContext context)
      {
         if (((context.Event != null) && (context.Schedule != null)) &&
             (((start <= Utilities.StringToDate(context.Schedule.EndDate)) &&
               (context.Schedule.Frequency != 0)) && (context.EventList != null)))
         {
            context.Event.StartDate = Utilities.NormalizeDate(start);
            context.Event.EndDate = Utilities.NormalizeDate(start);
            context.Event.ScheduleID = context.Schedule.ID;

            context.EventList.AddEvent(context.Event, StaticSettings.EventBranch);

            addDaily(start.AddDays(context.Schedule.Frequency), context);
         }
      }

      #endregion
   }
}