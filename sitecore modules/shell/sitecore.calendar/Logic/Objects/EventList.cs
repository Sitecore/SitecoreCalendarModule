using System;
using System.Collections.Generic;

using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Exceptions;
using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Objects
{
   public class EventList
   {
      #region Constants

      private static readonly string _sqlAfterDate =
         "{0}/*[@@name>='{1}']/*[(@@name>='{2}' and parent::*/@@name='{1}' ) or (parent::*/@@name>'{1}')]/*[(@@name>='{3}' and parent::*/parent::*/@@name='{1}') or (parent::*/@@name>'{2}') or (parent::*/parent::*/@@name>'{1}')]/*";

      private static readonly string _sqlBeforeDate =
         "{0}/*[@@name<='{1}']/*[(@@name<='{2}' and parent::*/@@name='{1}' ) or (parent::*/@@name<'{1}')]/*[(@@name<='{3}' and parent::*/parent::*/@@name='{1}') or (parent::*/@@name<'{2}') or (parent::*/parent::*/@@name<'{1}')]/*";

      private static readonly string _sqlDate = "{0}/*[@@name='{1}']/*[@@name='{2}']/*[@@name='{3}']/*";
      public static readonly string EventBoxColorField = "Event Box Color";
      public static readonly string EventEditIconField = "Event Edit Icon";
      public static readonly string EventTextColorField = "Event Text Color";
      public static readonly string SourceDatabaseField = "Source";
      public static readonly string TargetDatabaseField = "Target";
      public static readonly string TitleField = "Title";

      #endregion

      #region Private Fields

      private readonly Item _eventListItem = null;
      private readonly string _eventListPath = null;
      private string _eventEditIconPath = null;
      private bool _selected = true;
      private string _title = String.Empty;

      #endregion

      #region Initialization Methods

      protected EventList()
      {
      }

      public EventList(Item eventListItem)
      {
         _eventListItem = eventListItem;
      }

      #endregion

      #region Accessor Methods

      public string BackgroundColor
      {
         get
         {
            return _eventListItem.Fields[EventBoxColorField].Value;
         }
      }

      public string EventEditIcon
      {
         get
         {
            if (_eventEditIconPath == null)
            {
               ImageField edit_icon_fld = _eventListItem.Fields[EventEditIconField];
               if (edit_icon_fld != null)
               {
                  _eventEditIconPath = StaticSettings.MediaPrefix + edit_icon_fld.MediaItem.Paths.MediaPath + StaticSettings.MediaSuffix;
               }
            }

            return _eventEditIconPath;
         }
      }

      public string EventListPath
      {
         get
         {
            if (_eventListPath == null && _eventListItem != null)
            {
               return _eventListItem.Paths.FullPath;
            }
            else
            {
               return null;
            }
         }
      }

      public ID ID
      {
         get
         {
            return _eventListItem.ID;
         }
      }

      public string IDKey
      {
         get
         {
            return _eventListItem.ID.ToString();
         }
      }

      public bool Selected
      {
         get
         {
            return _selected;
         }
         set
         {
            _selected = value;
         }
      }

      public string TextColor
      {
         get
         {
            return _eventListItem.Fields[EventTextColorField].Value;
         }
      }

      public string Title
      {
         get
         {
            if (_title == String.Empty)
            {
               _title = _eventListItem.Fields[TitleField].Value;
            }

            return _title;
         }
      }

      #endregion

      #region General Functionality Methods

      public bool CanWrite
      {
         get
         {
            return SecurityManager.CanWrite(_eventListItem);
         }
      }

      public List<Event> GetEventsByDate(DateTime date)
      {
         List<Event> list = new List<Event>();

         string path = EventListPath + "/" + Utilities.NormalizeDate(date);
         Item dateRoot = StaticSettings.EventSourceDatabase.SelectSingleItem(path);
         if (dateRoot != null)
         {
            foreach (Item item in Utilities.GetChildrenUnsorted(dateRoot))
            {
               list.Add(new Event(item.ID));
            }
         }

         return list;
      }

      public bool HasEvents(DateTime date)
      {
         string path = EventListPath + "/" + Utilities.NormalizeDate(date);
         Item dateRoot = StaticSettings.EventSourceDatabase.SelectSingleItem(path);

         if (dateRoot != null)
         {
            return dateRoot.HasChildren;
         }

         return false;
      }

      public void GetEvents(DateTime date, EventsOrder order, int eventLimit,
                            ref SortedList<DateTime, SortedList<String, List<Event>>> eventList)
      {
         if (eventList == null)
         {
            return;
         }

         string[] normDate = Utilities.NormalizeDate(date).Split(new char[] {'/'});
         ;

         string xPath = string.Empty;

         switch (order)
         {
            case EventsOrder.Descending:
               xPath = String.Format(_sqlBeforeDate, EventListPath, normDate[0], normDate[1], normDate[2]);
               break;
            case EventsOrder.Ascending:
               xPath = String.Format(_sqlAfterDate, EventListPath, normDate[0], normDate[1], normDate[2]);
               break;
            case EventsOrder.OneDayOnly:
               xPath = String.Format(_sqlDate, EventListPath, normDate[0], normDate[1], normDate[2]);
               break;
            default:
               break;
         }

         Item[] items = StaticSettings.EventSourceDatabase.Database.SelectItems(xPath);

         if (items == null)
         {
            return;
         }

         int vector = 1;
         int direction = 0;

         if (order == EventsOrder.Descending)
         {
            vector = -1;
            direction = items.Length - 1;
         }

         for (int i = 0; i < items.Length; i++)
         {
            Event evnt = new Event(items[direction + i*vector]);
            DateTime dt = Utilities.StringToDate(evnt.StartDate);
            string time = evnt.StartTime + " " + evnt.EndTime;

            if (eventList.ContainsKey(dt))
            {
               SortedList<String, List<Event>> dateList = eventList[dt];

               if (dateList != null)
               {
                  if (dateList.ContainsKey(time))
                  {
                     List<Event> timeList = dateList[time];

                     if (timeList != null)
                     {
                        timeList.Add(evnt);
                     }
                  }
                  else
                  {
                     List<Event> timeList = new List<Event>();
                     timeList.Add(evnt);
                     dateList.Add(time, timeList);
                  }
               }
            }
            else
            {
               SortedList<String, List<Event>> dateList = new SortedList<String, List<Event>>();
               List<Event> timeList = new List<Event>();
               timeList.Add(evnt);
               dateList.Add(time, timeList);
               eventList.Add(dt, dateList);
            }
         }
      }

      public void DeleteEvent(string evID, bool deleteSeries)
      {
         CalendarActions.DeleteEvent(evID, deleteSeries);
      }

      public void DeleteEvent(Event evt, bool deleteSeries)
      {
         DeleteEvent(evt.ToString(), deleteSeries);
      }

      public void DeleteEvent(Event ev)
      {
         DeleteEvent(ev, false);
      }

      public void MoveEvent(Event ev)
      {
         CalendarActions.MoveEvent(ev.ID, ev.StartDate);
      }

      public void AddEvent(Event ev, BranchItem branch)
      {
         CreateEvent(ev, branch);
      }

      public Item CreateEvent(Event ev, BranchItem branch)
      {
         CalendarActions.CreateEvent(ev, null, new Options { CalendarID = ID.ToString() }, branch);
         return ev.GetItem();
      }

      #endregion
   }
}