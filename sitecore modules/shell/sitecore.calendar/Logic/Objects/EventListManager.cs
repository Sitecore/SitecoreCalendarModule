using System;
using System.Collections.Generic;
using System.Web;

using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Modules.EventCalendar.Core;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.SecurityModel;

using DaysCollection = Sitecore.Modules.EventCalendar.Utils.DaysOfWeek;

namespace Sitecore.Modules.EventCalendar.Objects
{
   public class EventListManager
   {
      #region Constants

      public static readonly string SchedulePrefix = "Schedule";

      #endregion Constants

      #region Private Variables

      private readonly Dictionary<string, EventList> _eventLists = new Dictionary<string, EventList>();

      #endregion Private Variables

      #region Initialization Methods

      public EventListManager(Item[] eventlist_root_items, string site_settings_path)
      {
         SiteSettings = new ModuleSettings(site_settings_path);
         _eventLists = ExtractEventLists(eventlist_root_items);
      }

      #endregion Initialization Methods

      public ModuleSettings SiteSettings { get; private set; }

      public Dictionary<string, EventList> EventLists
      {
         get
         {
            return _eventLists;
         }
      }

      public bool CanWrite
      {
         get
         {
            foreach (KeyValuePair<String, EventList> cal in ActiveEventLists)
            {
               if (cal.Value.CanWrite)
               {
                  return true;
               }
            }
            return false;
         }
      }

      public Dictionary<string, EventList> ActiveEventLists
      {
         get
         {
            List<string> selectedList = null;
            string filePath = HttpContext.Current.Request.FilePath;
            string url = HttpContext.Current.Request.RawUrl;
            if ((StaticSettings.ServiceReference.ToLower() == filePath.ToLower()) ||
                (HttpContext.Current.Session[StaticSettings.SessionStateUrl] != null &&
                 (string) HttpContext.Current.Session[StaticSettings.SessionStateUrl] == url))
            {
               selectedList = HttpContext.Current.Session[StaticSettings.SessionStateCalendarsList] as List<string>;
            }
            else
            {
               HttpContext.Current.Session.Remove(StaticSettings.SessionStateCalendarsList);
            }

            Dictionary<string, EventList> list = new Dictionary<string, EventList>();

            foreach (EventList evlist in EventLists.Values)
            {
               if (selectedList != null && selectedList.Contains(evlist.IDKey) != true)
               {
                  continue;
               }

               list.Add(evlist.IDKey, evlist);
            }
            return list;
         }
      }

      private static Dictionary<string, EventList> ExtractEventLists(IEnumerable<Item> items)
      {
         Dictionary<string, EventList> list = new Dictionary<string, EventList>();
         if (items != null)
         {
            foreach (Item this_item in items)
            {
               if (this_item != null)
               {
                  EventList value;
                  if (this_item.TemplateID == CalendarIDs.CalendarFolderTemplate)
                  {
                     ChildList children = this_item.Children;
                     Dictionary<string, EventList> sublist = ExtractEventLists(children.ToArray());

                     foreach (KeyValuePair<String, EventList> subitem in sublist)
                     {
                        if (!list.TryGetValue(subitem.Key, out value))
                        {
                           list.Add(subitem.Key, subitem.Value);
                        }
                     }
                  }
                  else if (this_item.TemplateID == CalendarIDs.EventListTemplate)
                  {
                     if (!list.TryGetValue(this_item.ID.ToString(), out value))
                     {
                        list.Add(this_item.ID.ToString(), new EventList(this_item));
                     }
                  }
               }
            }
         }
         return list;
      }

      public bool HasEvents(DateTime date)
      {
         foreach (KeyValuePair<String, EventList> cal in EventLists)
         {
            if (cal.Value.Selected != true)
            {
               continue;
            }

            if (cal.Value.HasEvents(date))
            {
               return true;
            }
         }

         return false;
      }

      public int GetEventsCount(DateTime date)
      {
         int total = 0;

         foreach (KeyValuePair<String, EventList> cal in EventLists)
         {
            if (cal.Value.Selected != true)
            {
               continue;
            }

            total += cal.Value.GetEventsByDate(date).Count;
         }

         return total;
      }

      public IList<Event> GetSortedEvents(DateTime date, int countSort, IComparer<Event> comparer)
      {
         List<Event> list = new List<Event>();

         foreach (EventList evlist in ActiveEventLists.Values)
         {
            list.AddRange(evlist.GetEventsByDate(date));
         }

         if (list.Count > 0)
         {
            countSort = countSort == 0 ? countSort : list.Count;
            list.Sort(0, countSort, comparer);
         }

         return list;
      }

      public bool HasActiveEvents(DateTime date)
      {
         List<string> selectedList =
            HttpContext.Current.Session[StaticSettings.SessionStateCalendarsList] as List<string>;

         foreach (KeyValuePair<String, EventList> cal in ActiveEventLists)
         {
            if (cal.Value.Selected != true)
            {
               continue;
            }

            if (selectedList != null && selectedList.Contains(cal.Key) != true)
            {
               continue;
            }

            if (cal.Value.HasEvents(date))
            {
               return true;
            }
         }

         return false;
      }

      public EventList GetCalendar(Event ev)
      {
         return GetCalendar(StaticSettings.EventSourceDatabase.GetItem(ev.ID));
      }

      public EventList GetCalendar(string idCalendar)
      {
         Item cal = StaticSettings.EventSourceDatabase.GetItem(idCalendar);
         return new EventList(cal);
      }

      public EventList GetCalendar(Item ev)
      {
         Item sourceItem = Utilities.GetCalendarByEvent(ev);
         if (sourceItem == null)
         {
            return null;
         }

         foreach (KeyValuePair<String, EventList> cal in EventLists)
         {
            if (cal.Value.ID == sourceItem.ID)
            {
               return cal.Value;
            }
         }

         return null;
      }

      public bool CreateSchedule(Schedule schedule)
      {
         if (schedule == null)
         {
            return false;
         }

         Item root = SiteSettings.SchedulesRoot;
         if (root == null)
         {
            return false;
         }

         using (new SecurityDisabler())
         {
            Item newSched = root.Add(SchedulePrefix + root.Children.Count, StaticSettings.ScheduleTemplate);
            schedule.SaveRecurrence(newSched);
            schedule.ID = newSched.ID.ToString();
         }

         return true;
      }

      public static void UpdateSchedule(ID scheduleID)
      {
         if (scheduleID == ID.Null)
         {
            return;
         }

         Item itm = StaticSettings.EventTargetDatabase.Database.Items[scheduleID];

         ItemLink[] links = Globals.LinkDatabase.GetReferences(itm);

         if (links.GetLength(0) == 1)
         {
            itm.Delete();
         }
      }

      public void UpdateEvent(Event evnt, Schedule schedule, string calendarID, bool updateSeries, BranchItem branchID)
      {
         if (evnt == null)
         {
            return;
         }

         EventList newCal = EventLists[calendarID];
         EventList currentCal = GetCalendar(evnt);

         if (updateSeries == false)
         {
            evnt.ScheduleID = string.Empty;
            evnt.Save();
            PublishUtil.Publishing(evnt.GetTargetItem(), true, false);

            if (newCal != null && currentCal != null && currentCal.ID != newCal.ID)
            {
               currentCal.DeleteEvent(evnt);

               newCal.AddEvent(evnt, branchID);
            }

            return;
         }

         if (schedule == null || schedule.IsNew || schedule.IsChanged)
         {
            DeleteEvent(evnt.ID, true, true);
            CreateEvent(evnt, schedule, newCal, branchID);

            return;
         }

         if (newCal != null && currentCal != null)
         {
            DeleteEvent(evnt.ID, true, true);

            evnt.StartDate = schedule.StartDate;
            evnt.EndDate = schedule.EndDate;

            CreateEvent(evnt, schedule, newCal, branchID);
         }

         if (evnt.IsChanged != true)
         {
            return;
         }

         ItemLink[] links = Globals.LinkDatabase.GetReferences(schedule.GetTargetItem());
         foreach (ItemLink link in links)
         {
            evnt.SaveToItem(link.GetSourceItem(), true);
         }
      }

      public void MoveEvent(Event ev)
      {
         if (ev != null)
         {
            EventList currentCal = GetCalendar(ev);

            if (currentCal != null)
            {
               currentCal.MoveEvent(ev);
            }
         }
      }

      public static void DeleteEvent(string eventID, bool deleteSeries, bool preserveSchedule)
      {
         if (string.IsNullOrEmpty(eventID))
         {
            return;
         }

         Item item = StaticSettings.EventTargetDatabase.GetItem(ID.Parse(eventID));
         if (item == null)
         {
            Log.Error("Calendar Module: cannot find event", null);
            return;
         }

         Item schedule = ((LinkField) item.Fields[Event.ScheduleIDField]).TargetItem;

         if (deleteSeries == false || schedule == null)
         {
            Item parent = item.Parent;

            item.Delete();
            PublishUtil.Publishing(item, false, true);
            return;
         }

         ItemLink[] links = Globals.LinkDatabase.GetReferences(schedule);

         foreach (ItemLink link in links)
         {
            link.GetSourceItem().Delete();
         }

         if (preserveSchedule != true)
         {
            schedule.Delete();
         }
      }

      public void CreateEvent(Event eventItem, Schedule schedule, EventList calendar, BranchItem branch)
      {
         if (eventItem == null || calendar == null)
         {
            return;
         }

         calendar.AddEvent(eventItem, branch);

         if (schedule != null)
         {
            if (schedule.IsNew)
            {
               schedule.StartDate = eventItem.StartDate;
               schedule.EndDate = eventItem.EndDate;

               CreateSchedule(schedule);
            }

            switch (schedule.Recurrence)
            {
               case Recurrence.Daily:

                  addDaily(Utilities.StringToDate(schedule.StartDate), eventItem, schedule, calendar);

                  break;
               case Recurrence.Weekly:

                  addWeekly(Utilities.FirstCalendarDay(Utilities.StringToDate(schedule.StartDate)), eventItem, schedule,
                            calendar);

                  break;
               case Recurrence.Monthly:

                  addMonthly(Utilities.FirstMonthDay(Utilities.StringToDate(schedule.StartDate)), eventItem, schedule,
                             calendar);

                  break;
               case Recurrence.Yearly:

                  addYearly(Utilities.FirstMonthDay(Utilities.StringToDate(schedule.StartDate)), eventItem, schedule,
                            calendar);
                  break;
               default:
                  break;
            }
         }
      }

      private static void addDaily(DateTime start, Event eventItem, Schedule schedule, EventList calendar)
      {
         if (eventItem == null || schedule == null || start > Utilities.StringToDate(schedule.EndDate) ||
             schedule.Frequency == 0 || calendar == null)
         {
            return;
         }

         eventItem.StartDate = Utilities.NormalizeDate(start);
         eventItem.EndDate = Utilities.NormalizeDate(start);

         eventItem.ScheduleID = schedule.ID;

         calendar.AddEvent(eventItem, StaticSettings.EventBranch);

         addDaily(start.AddDays(schedule.Frequency), eventItem, schedule, calendar);
      }

      private static void addWeekly(DateTime start, Event eventItem, Schedule schedule, EventList calendar)
      {
         if (eventItem == null || schedule == null || start > Utilities.StringToDate(schedule.EndDate) ||
             schedule.Frequency == 0 || calendar == null)
         {
            return;
         }

         addByDayOfWeek(start, eventItem, schedule, calendar);

         start = start.AddDays(schedule.Frequency*7);

         addWeekly(start, eventItem, schedule, calendar);
      }

      private static void addYearly(DateTime start, Event eventItem, Schedule schedule, EventList calendar)
      {
         if (eventItem == null || schedule == null || start > Utilities.StringToDate(schedule.EndDate) ||
             schedule.Frequency == 0 || calendar == null)
         {
            return;
         }

         DateTime tmp = new DateTime(start.Year, (int) schedule.Month, schedule.Frequency);

         if (tmp >= Utilities.StringToDate(schedule.StartDate) && tmp <= Utilities.StringToDate(schedule.EndDate))
         {
            addEvent(eventItem, tmp, calendar, schedule);
         }

         addYearly(start.AddYears(1), eventItem, schedule, calendar);
      }

      private static void addMonthly(DateTime start, Event eventItem, Schedule schedule, EventList calendar)
      {
         if (eventItem == null || schedule == null || start > Utilities.StringToDate(schedule.EndDate) ||
             schedule.Frequency == 0 || calendar == null)
         {
            return;
         }

         switch (schedule.Sequence)
         {
            case Sequence.First:
            case Sequence.Second:
            case Sequence.Third:
            case Sequence.Fourth:
               addSequence(start, eventItem, schedule, calendar);
               break;

            case Sequence.Last:

               DateTime tmp = start.AddMonths(1);
               tmp = tmp.Subtract(TimeSpan.FromDays(1));

               while (true)
               {
                  if (string.Compare(tmp.DayOfWeek.ToString(), schedule.DaysOfWeek.ToString(), true) == 0)
                  {
                     addEvent(eventItem, tmp, calendar, schedule);
                     break;
                  }
                  else
                  {
                     tmp = tmp.Subtract(TimeSpan.FromDays(1));
                  }
               }

               break;
            default:
               break;
         }

         addMonthly(start.AddMonths(schedule.Frequency), eventItem, schedule, calendar);
      }

      private static void addByDayOfWeek(DateTime start, Event eventItem, Schedule schedule, EventList calendar)
      {
         if ((schedule.DaysOfWeek & DaysCollection.Monday) == DaysCollection.Monday)
         {
            addEvent(eventItem, start, calendar, schedule);
         }

         if ((schedule.DaysOfWeek & DaysCollection.Tuesday) == DaysCollection.Tuesday)
         {
            addEvent(eventItem, start.AddDays(1), calendar, schedule);
         }

         if ((schedule.DaysOfWeek & DaysCollection.Wednesday) == DaysCollection.Wednesday)
         {
            addEvent(eventItem, start.AddDays(2), calendar, schedule);
         }

         if ((schedule.DaysOfWeek & DaysCollection.Thursday) == DaysCollection.Thursday)
         {
            addEvent(eventItem, start.AddDays(3), calendar, schedule);
         }

         if ((schedule.DaysOfWeek & DaysCollection.Friday) == DaysCollection.Friday)
         {
            addEvent(eventItem, start.AddDays(4), calendar, schedule);
         }

         if ((schedule.DaysOfWeek & DaysCollection.Saturday) == DaysCollection.Saturday)
         {
            addEvent(eventItem, start.AddDays(5), calendar, schedule);
         }

         if ((schedule.DaysOfWeek & DaysCollection.Sunday) == DaysCollection.Sunday)
         {
            addEvent(eventItem, start.AddDays(6), calendar, schedule);
         }
      }

      private static void addEvent(Event eventItem, DateTime date, EventList calendar, Schedule schedule)
      {
         if (date > Utilities.StringToDate(schedule.EndDate))
         {
            return;
         }

         eventItem.StartDate = Utilities.NormalizeDate(date);
         eventItem.EndDate = Utilities.NormalizeDate(date);
         eventItem.ScheduleID = schedule.ID;
         calendar.AddEvent(eventItem, StaticSettings.EventBranch);
      }

      private static void addSequence(DateTime start, Event eventItem, Schedule schedule, EventList calendar)
      {
         DateTime tmp = start;

         while (true)
         {
            if (string.Compare(tmp.DayOfWeek.ToString(), schedule.DaysOfWeek.ToString(), true) == 0)
            {
               if (schedule.Sequence != Sequence.None && schedule.Sequence != Sequence.First)
               {
                  tmp = tmp.AddDays(((int) schedule.Sequence)*7);
               }

               if (tmp >= Utilities.StringToDate(schedule.StartDate))
               {
                  addEvent(eventItem, tmp, calendar, schedule);
               }

               break;
            }
            else
            {
               tmp = tmp.AddDays(1);
            }
         }
      }
   }
}