using System;

using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class AgendaViewSettings
   {
      #region Constants

      public const string SelectedDateOption = "Selected Date";
      public const string TodayOption = "Today";
      public static readonly string CalendarListField = StandardViewSettings.CalendarListField;
      public static readonly string DateFormatField = "Date Format";
      public static readonly string EventLimitField = "Event Limit";
      public static readonly string EventOrderField = "Event Order";
      public static readonly string MarginField = "Margin";
      public static readonly string ShowAllCalendarsField = StandardViewSettings.ShowAllCalendarsField;
      public static readonly string ShowEventDateField = "Show Event Date";
      public static readonly string ShowEventTimeField = "Show Event Time";
      public static readonly string StartDateField = "Start Date";
      public static readonly string TitleField = "Title";

      #endregion

      #region Attributes

      private string _dateFormat = null;
      private int _eventLimit = 0;
      private Item[] _eventLists = null;
      private string _margin = null;
      private bool _showEventDate = true;
      private bool _showEventTime = true;
      private StartDateOption _startDate = StartDateOption.Today;
      private string _title = null;
      private Item settings = null;

      #endregion

      #region Accessor Methods

      public string DateFormat
      {
         get
         {
            return _dateFormat;
         }
         set
         {
            _dateFormat = value;
         }
      }

      public string ID
      {
         get
         {
            return settings.ID.ToString();
         }
      }

      public int EventLimit
      {
         get
         {
            return _eventLimit;
         }
         set
         {
            _eventLimit = value;
         }
      }

      public Item[] EventLists
      {
         get
         {
            return _eventLists;
         }
         set
         {
            _eventLists = value;
         }
      }

      public string Margin
      {
         get
         {
            return _margin;
         }
         set
         {
            _margin = value;
         }
      }

      public bool ShowEventDate
      {
         get
         {
            return _showEventDate;
         }
         set
         {
            _showEventDate = value;
         }
      }

      public bool ShowEventTime
      {
         get
         {
            return _showEventTime;
         }
         set
         {
            _showEventTime = value;
         }
      }

      public StartDateOption StartDate
      {
         get
         {
            return _startDate;
         }
         set
         {
            _startDate = value;
         }
      }

      public string Title
      {
         get
         {
            return _title;
         }
         set
         {
            _title = value;
         }
      }

      #endregion

      #region Initialization Methods

      public AgendaViewSettings(Item settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private void Init(Item settings_item, string site_settings_path)
      {
         if (settings_item == null)
         {
            Log.Warn("Calendar Module: agenda view settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).AgendaViewSettings;
         }
         settings = settings_item;

         string limit = settings_item[EventLimitField];
         if (limit.Length > 0)
         {
            try
            {
               EventLimit = int.Parse(limit);
            }
            catch (Exception e)
            {
               Log.Warn("Calendar Module Agenda View Settings: Exception trying to parse Event Limit Field!", this);
               Log.Warn(e.Message, this);
               EventLimit = 0;
            }
         }

         Margin = settings_item[MarginField];
         ShowEventDate = MainUtil.GetBool(settings_item[ShowEventDateField], false);
         ShowEventTime = MainUtil.GetBool(settings_item[ShowEventTimeField], false);
         _dateFormat = settings_item[DateFormatField];
         if (string.IsNullOrEmpty(_dateFormat))
         {
            _dateFormat = "MM/dd/yyyy";
         }
         string date_option = settings_item[StartDateField];
         switch (date_option)
         {
            case TodayOption:
               StartDate = StartDateOption.Today;
               break;
            case SelectedDateOption:
               StartDate = StartDateOption.SelectedDate;
               break;
         }
         Title = settings_item[TitleField];

         EventLists = Utilities.GetEventListForView(settings_item, site_settings_path);
      }

      #endregion
   }
}