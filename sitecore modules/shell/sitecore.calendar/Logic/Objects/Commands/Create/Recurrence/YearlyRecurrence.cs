using System;

using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class YearlyRecurrence : EventCommand
   {
      #region Methods

      protected override void Execute(EventCommandContext context)
      {
         AddYearly(Utilities.FirstMonthDay(Utilities.StringToDate(context.Schedule.StartDate)),
                   context);
      }

      private static void AddYearly(DateTime start, EventCommandContext context)
      {
         if (context.Event == null || context.Schedule == null ||
             start > Utilities.StringToDate(context.Schedule.EndDate) ||
             context.Schedule.Frequency == 0 || context.EventList == null)
         {
            return;
         }

         try
         {
            var tmp = new DateTime(start.Year, (int) context.Schedule.Month,
                                   context.Schedule.Frequency);

            if (tmp >= Utilities.StringToDate(context.Schedule.StartDate) &&
                tmp <= Utilities.StringToDate(context.Schedule.EndDate))
            {
               AddEvent(tmp, context);
            }
         }
         catch
         {
            Log.Warn("Calendar: cannot add a yearly recurrence event:" + context.Event.Name, context);
         }

         AddYearly(start.AddYears(1), context);
      }

      #endregion
   }
}