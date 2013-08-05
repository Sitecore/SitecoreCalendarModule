using System;
using Sitecore.Modules.EventCalendar.Utils;

using DaysCollection = Sitecore.Modules.EventCalendar.Utils.DaysOfWeek;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
	public class WeeklyRecurrence : EventCommand
   {
      #region Methods

      protected override void Execute(EventCommandContext context)
      {
         addWeekly(context, Utilities.FirstCalendarDay(Utilities.StringToDate(context.Schedule.StartDate)));
      }

      private static void addWeekly(EventCommandContext context, DateTime start)
      {
         if (context.Event != null && context.Schedule != null && start <= Utilities.StringToDate(context.Schedule.EndDate) &&
             context.Schedule.Frequency > 0 && context.EventList != null)
         {
            addByDayOfWeek(start, context);

            start = start.AddDays(context.Schedule.Frequency * 7);

            addWeekly(context, start);
         }
      }

      private static void addByDayOfWeek(DateTime start, EventCommandContext context)
      {
         int number = 0;
         foreach (DaysCollection value in Enum.GetValues(typeof(DaysCollection)))
         {
            if (value != 0) // 0 is 'None' in DaysCollection enum
            {
            	++number;

            	if ((value & context.Schedule.DaysOfWeek) == value)
               {
                  AddEvent(start.AddDays(number), context);
               }
            }
         }
      }

      #endregion
   }
}