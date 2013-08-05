using System;

using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class MonthyRecurrence : EventCommand
   {
      #region Methods

      protected override void Execute(EventCommandContext context)
      {
         AddMonthly(Utilities.FirstMonthDay(Utilities.StringToDate(context.Schedule.StartDate)),
                    context);
      }

      private static void AddMonthly(DateTime start, EventCommandContext context)
      {
         if (context.Event == null || context.Schedule == null ||
             start > Utilities.StringToDate(context.Schedule.EndDate) ||
             context.Schedule.Frequency == 0 || context.EventList == null)
         {
            return;
         }

         switch (context.Schedule.Sequence)
         {
            case Sequence.First:
            case Sequence.Second:
            case Sequence.Third:
            case Sequence.Fourth:
               AddSequence(start, context);
               break;

            case Sequence.Last:

               DateTime tmp = start.AddMonths(1);
               tmp = tmp.Subtract(TimeSpan.FromDays(1));

               while (true)
               {
                  if (
                     string.Compare(tmp.DayOfWeek.ToString(), context.Schedule.DaysOfWeek.ToString(),
                                    true) == 0)
                  {
                     AddEvent(tmp, context);
                     break;
                  }

                  tmp = tmp.Subtract(TimeSpan.FromDays(1));
               }

               break;
            default:
               break;
         }

         AddMonthly(start.AddMonths(context.Schedule.Frequency), context);
      }

      private static void AddSequence(DateTime start, EventCommandContext context)
      {
         DateTime tmp = start;

         while (true)
         {
            if (
               string.Compare(tmp.DayOfWeek.ToString(), context.Schedule.DaysOfWeek.ToString(), true) ==
               0)
            {
               if (context.Schedule.Sequence != Sequence.None &&
                   context.Schedule.Sequence != Sequence.First)
               {
                  tmp = tmp.AddDays(((int) context.Schedule.Sequence) * 7);
               }

               if (tmp >= Utilities.StringToDate(context.Schedule.StartDate))
               {
                  AddEvent(tmp, context);
               }

               break;
            }

            tmp = tmp.AddDays(1);
         }
      }

      #endregion
   }
}