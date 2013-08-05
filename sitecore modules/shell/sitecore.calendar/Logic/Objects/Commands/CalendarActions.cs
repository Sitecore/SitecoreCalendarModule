using System.Collections.Generic;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Objects.Commands;
using Sitecore.Shell.Framework;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.EventCalendar.Commands
{
   public class CalendarActions
   {
      #region Methods

      public static void MoveEvent(string eventID, string newDate)
      {
         Message message = Message.Parse(eventID, "event:move");
         var context = new MoveEventContext(eventID, newDate);
         Dispatcher.Dispatch(message, context);
      }

      public static void UpdateTime(string eventID, string startTime, string endTime)
      {
         UpdateTime(eventID, startTime, endTime, false);
      }

      public static void UpdateTime(string eventID, string startTime, string endTime, bool updateSeries)
      {
         var message = Message.Parse(eventID, "event:movetime");
         var moveTimeContext = new MoveEventTimeContext(eventID, startTime, endTime, updateSeries);
         Dispatcher.Dispatch(message, moveTimeContext);
      }

      public static void CreateEvent(Event ev, Schedule schedule, Options options)
      {
         CreateEvent(ev, schedule, options, null);
      }

      public static void CreateEvent(Event ev, Schedule schedule, Options options, BranchItem branch)
      {
         Message message = Message.Parse(ev, "event:create");
         var calendarContext = new EventCommandContext(ev, schedule, options, branch);
         Dispatcher.Dispatch(message, calendarContext);
      }

      public static void DeleteEvent(string eventID, bool deleteSeries)
      {
         var context = new DeleteEventContext(eventID, deleteSeries);
         Message message = Message.Parse(eventID, "event:delete");
         Dispatcher.Dispatch(message, context);
      }

      public static void UpdateEvent(Event evnt, Schedule schedule, Options options, bool updateSeries)
      {
         var context = new UpdateCommandContext(evnt, schedule, options, updateSeries);
         Message message = Message.Parse(evnt, "event:update");
         Dispatcher.Dispatch(message, context);
      }

      public static void CreateSchedule(Event ev, Schedule schedule, Options options)
      {
         Message message = Message.Parse(ev, "schedule:create");
         var context = new EventCommandContext(ev, schedule, options);
         Dispatcher.Dispatch(message, context);
      }

      public static void DeleteScheduler(Schedule schedule)
      {
         DeleteScheduler(schedule, false);
      }

      public static void CreateRecurrence(Event ev, Schedule schedule, Options options)
      {
         Message message = Message.Parse(ev,
                                         "recurrence:" +
                                         schedule.Recurrence.ToString().ToLower());
         var context = new EventCommandContext(ev, schedule, options);
         Dispatcher.Dispatch(message, context);
      }

      public static void DeleteScheduler(Schedule schedule, bool deleteSeries)
      {
         Message message = Message.Parse(schedule, "schedule:delete");

         var scheduleContext = new DeleteScheduleContext(schedule, deleteSeries);
         Dispatcher.Dispatch(message, scheduleContext);
      }

      public static void SyncEvents(Event pattern, IEnumerable<Event> events, EventList list, EventListManager manager)
      {
         Message message = Message.Parse(pattern, "event:sync");
         var deleteContext = new SyncEventContext(pattern, events, list,
                                                  manager);
         Dispatcher.Dispatch(message, deleteContext);
      }

      #endregion
   }
}