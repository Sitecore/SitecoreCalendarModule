using System;
using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public static class Settings
   {
      #region Constants

      public const string ModulePath = "/sitecore/system/modules/calendar";

      public const string CalendarsListRoot = "/sitecore/system/modules/calendar/calendars";
      public const string ModulePathUrl = "/sitecore%20modules/shell/sitecore.calendar";
      public const string Schedules = "/sitecore/system/modules/calendar/schedules";
      public const string SitecoreRoot = "/sitecore/content";
      public const string EventWizardPath = "~/sitecore modules/shell/sitecore.calendar/controls/EventWizard.ascx";
      public const string ViewForm = "/layouts/calendar.aspx";
      public const string QSDayView = "dayView";
      public const string QSDate = "date";
      public const string FolderLocations = "/locations";
      public const string FolderMails = "/mails";
      public const string BlankGIF = @"/sitecore modules/shell/sitecore.calendar/Images/blank.gif";
      public const string StyleSheet = @"/sitecore%20modules/shell/sitecore.calendar/themes/{0}/{0}.css";
      public const string TitleField = "Title";
      public const string LocationField = "Location";
      public const string DescriptionField = "Description";
      public const string StartDateField = "Start Date";
      public const string EndDateField = "End Date";
      public const string StartTimeField = "Start Time";
      public const string EndTimeField = "End Time";
      public const string ScheduleIDField = "ScheduleID";
      public const string DefaultDotField = "DefaultDotImage";
      public const string CurrentMonthDotField = "CurrentMonthDotImage";
      private const string OtherMonthDotField = "OtherMonthDotImage";
      public const string DotImageField = "DotImage";
      public const string TextColorField = "TextColor";
      public const string BackgroundColorField = "BackgroundColor";
      private const string ThemeField = "Theme";
      private const string ReadOnlyField = "ReadOnly";
      public const string SelectedField = "Selected";
      public const string SourceField = "Source";
      public const string DatabaseField = "Database";
      private const string HighlightWeekendsField = "HighlightWeekends";
      private const string RenderEventInBlockField = "RenderEventInBlock";
      private const string ShowAsDropDownListField = "ShowAsDropDownList";
      private const string AgendaTitleField = "AgendaTitle";
      private const string SelectorTitleField = "SelectorTitle";
      private const string SelectorLinkTextField = "SelectorLinkText";
      private const string QuickDatePickerTitleField = "QuickDatePickerTitle";
      private const string QuickDatePickerLinkTextField = "QuickDatePickerLinkText";
      private const string QuickDatePickerTodayLinkTextField = "QuickDatePickerTodayLinkText";
      private const string LinkBehaviourField = "LinkBehaviour";

      private const string AgendaStartDateField = "AgendaStartDate";

      public const string QueryStringTheme = "t"; //TODO: to make it customizable

      public const string ShowDotsField = "ShowDots";
      public const string EventsOverflowField = "EventsOverflow";
      public const string ShowOtherMonthField = "ShowOtherMonth";
      public const string ShowHeaderField = "ShowHeader";
      public const string ShowTimeField = "ShowTime";
      public const string DateFormatField = "DateFormat";

      public const string RecurrentSymbol = "[R]";

      public const string RecurOnce = "Once";
      public const string RecurDaily = "Daily";
      public const string RecurWeekly = "Weekly";
      public const string RecurMonthly = "Monthly";
      public const string RecurYearly = "Yearly";

      public const string RecurEveryN = "RecurEveryN";
      public const string RecurDaysOfWeek = "RecurDaysOfWeek";
      public const string RecurSeqOrdinal = "RecurEveryNSeqOrdinal";
      public const string RecurMonthName = "RecurMonthName";

      public const string SchedulePrefix = "Schedule";

      public const string Checked = "1";
      public const string Unchecked = "0";

      public const int HoursSpan = 1;
      public const int MinutesSpan = 30;
      public const string EntryPoint = "/sitecore%20modules/shell/sitecore.calendar/controls/calendar.aspx";

      public const string ScriptDayManager = "/sitecore modules/shell/sitecore.calendar/Scripts/Managers/DayManager.js";
      public const string ScriptMonthManager = "/sitecore modules/shell/sitecore.calendar/Scripts/Managers/MonthManager.js";
      public const string ScriptWeekManager = "/sitecore modules/shell/sitecore.calendar/Scripts/Managers/WeekManager.js";
      public const string ScriptUtils = "/sitecore modules/shell/sitecore.calendar/Scripts/Utils.js";
      public const string ScriptEvent = "/sitecore modules/shell/sitecore.calendar/Scripts/Event.js";
      public const string ScriptEventPopup = "/sitecore modules/shell/sitecore.calendar/Scripts/EventPopups.js";
      public const string ScriptProxy = "/sitecore modules/shell/sitecore.calendar/Scripts/Proxy.js";
      public const string ScriptValidator = "/sitecore modules/shell/sitecore.calendar/Scripts/Validator.js";
      public const string SessionStateCurrentDate = "scEventCalendarCurrentDate";
      public const string SessionStateCalendarsList = "scEventCalendarCalendarList";

      private static SitecoreDatabase _masterDatabase = null;

      private static SitecoreDatabase _database = null;

      private static TemplateItem _calendarTemplate = null;
      private static TemplateItem _eventTemplate = null;
      private static TemplateItem _scheduleTemplate = null;
      private static TemplateItem _dayTemplate = null;

      private static MasterItem _eventMaster = null;

      private static string _defaultDot = null;
      private static string _currentMonthDot = null;
      private static string _otherMonthDot = null;
      private static bool _readOnly = false;
      private static bool _highlightWeekends = false;
      private static bool _renderEventInBlock = false;
      private static bool _showAsDropDownList = false;
      private static string _agendaTitle = string.Empty;
      private static string _themeName = null;

      private static bool _showDots = false;
      private static EventsListOverflow _eventsOverflow = EventsListOverflow.ShowEllipsis;
      private static bool _showOtherMonth = false;
      private static bool _showHeader = false;
      private static bool _showTime = false;
      private static DateFormat _dateFormat = DateFormat.DayMonthYear;
      private static string _selectorTitle = string.Empty;
      private static string _selectorLinkText = string.Empty;

      private static string _quickDatePickerTitle = string.Empty;
      private static string _quickDatePickerLinkText = string.Empty;
      private static string _quickDatePickerTodayLinkText = string.Empty;

      private static string _agendaStartDate = string.Empty;
      private static LinkBehaviour _linkBehaviourDatePicker = LinkBehaviour.None;

      public static readonly string ScriptManagerID = Guid.NewGuid().ToString();
  
      public static Item _calendarRootItem = null;

      #endregion

      static Settings()
      {
         Init();
      }

      public static void Init()
      {
          _masterDatabase = new SitecoreDatabase("master");
          using (new SecurityDisabler())
          {
             try
             {
                _database = new SitecoreDatabase(DatabaseName);
             }
             catch (NullReferenceException)
             {
                _database = _masterDatabase;
             }
             finally
             {
                if (_database == null)
                {
                   _database = _masterDatabase;
                }
             }

             _calendarTemplate = Database.GetItem("/sitecore/templates/calendar/calendar.calendar");

             _eventTemplate = Database.GetItem("/sitecore/templates/calendar/calendar.event");

             _dayTemplate = Database.GetItem("/sitecore/templates/calendar/calendar.day");

             _scheduleTemplate = Database.GetItem("/sitecore/templates/calendar/calendar.schedule");

             _eventMaster = Database.GetItem("/sitecore/masters/calendar.event");

             CalendarsListRootItem = Database.GetItem(CalendarsListRoot);

             _defaultDot = CalendarsListRootItem.Fields[DefaultDotField].Value;
             _currentMonthDot = CalendarsListRootItem.Fields[CurrentMonthDotField].Value;
             _otherMonthDot = CalendarsListRootItem.Fields[OtherMonthDotField].Value;
             _themeName = CalendarsListRootItem.Fields[ThemeField].Value;
             _readOnly = CalendarsListRootItem.Fields[ReadOnlyField].Value == "1";

             _highlightWeekends = CalendarsListRootItem.Fields[HighlightWeekendsField].Value == "1";
             _renderEventInBlock = CalendarsListRootItem.Fields[RenderEventInBlockField].Value == "1";
             _showAsDropDownList = CalendarsListRootItem.Fields[ShowAsDropDownListField].Value == "1";
             _agendaTitle = CalendarsListRootItem.Fields[AgendaTitleField].Value;

             _showDots = CalendarsListRootItem.Fields[ShowDotsField].Value == "1";

             _agendaStartDate = CalendarsListRootItem.Fields[AgendaStartDateField].Value;

             if( String.IsNullOrEmpty( CalendarsListRootItem.Fields[EventsOverflowField].Value)!= true)
             {
                _eventsOverflow = (EventsListOverflow) Enum.Parse( typeof( EventsListOverflow),
                                                                   CalendarsListRootItem.Fields[EventsOverflowField].Value);
             }

             _showOtherMonth = CalendarsListRootItem.Fields[ ShowOtherMonthField ].Value == "1";

             _showHeader = CalendarsListRootItem.Fields[ShowHeaderField].Value == "1";

             _showTime = CalendarsListRootItem.Fields[ShowTimeField].Value == "1";

             _selectorTitle = CalendarsListRootItem.Fields[SelectorTitleField].Value;

             _selectorLinkText = CalendarsListRootItem.Fields[SelectorLinkTextField].Value;

             _quickDatePickerTitle = CalendarsListRootItem.Fields[QuickDatePickerTitleField].Value;

             _quickDatePickerLinkText = CalendarsListRootItem.Fields[QuickDatePickerLinkTextField].Value;

             _quickDatePickerTodayLinkText = CalendarsListRootItem.Fields[QuickDatePickerTodayLinkTextField].Value;

             _linkBehaviourDatePicker =
                (CalendarsListRootItem.Fields[LinkBehaviourField].Value == string.Empty)
                   ? LinkBehaviour.None
                   : (LinkBehaviour) Enum.Parse(
                                        typeof (LinkBehaviour),
                                        CalendarsListRootItem.Fields[LinkBehaviourField].Value);
             
             if (String.IsNullOrEmpty(CalendarsListRootItem.Fields[DateFormatField].Value) != true)
             {
                _dateFormat = (DateFormat)Enum.Parse(typeof(DateFormat),
                                                     CalendarsListRootItem.Fields[DateFormatField].Value);
             }
          }
       }

      #region Properties


      public static LinkBehaviour LinkBehaviourDatePicker
      {
         get { return _linkBehaviourDatePicker; }
         set
         {
            CalendarsListRootItem.Fields[LinkBehaviourField].Value = value.ToString();

            _linkBehaviourDatePicker = value;
         }
      }


      public static string AgendaStartDate
      {
         get
         {
            return _agendaStartDate;
         }
         set
         {
            CalendarsListRootItem.Fields[AgendaStartDateField].Value = value;

            _agendaStartDate = value;
         }
      }

      public static string SelectorTitle
      {
         get { return _selectorTitle; }
         set
         {
            CalendarsListRootItem.Fields[SelectorTitleField].Value = value;

            _selectorTitle = value;
         }
      }

      public static string SelectorLinkText
      {
         get
         {
            return _selectorLinkText;
         }
         set
         {
            CalendarsListRootItem.Fields[SelectorLinkTextField].Value = value;

            _selectorLinkText = value;
         }
      }

      public static string QuickDatePickerTitle
      {
         get
         {
            return _quickDatePickerTitle;
         }
         set
         {
            CalendarsListRootItem.Fields[QuickDatePickerTitleField].Value = value;

            _quickDatePickerTitle = value;
         }
      }

      public static string QuickDatePickerLinkText
      {
         get
         {
            return _quickDatePickerLinkText;
         }
         set
         {
            CalendarsListRootItem.Fields[QuickDatePickerLinkTextField].Value = value;

            _quickDatePickerLinkText = value;
         }
      }


      public static string QuickDatePickerTodayLinkText
      {
         get
         {
            return _quickDatePickerTodayLinkText;
         }
         set
         {
            CalendarsListRootItem.Fields[QuickDatePickerTodayLinkTextField].Value = value;

            _quickDatePickerTodayLinkText = value;
         }
      }

      public static bool ShowDots
      {
         get
         {
            return _showDots;
         }
         set
         {
            CalendarsListRootItem.Fields[ShowDotsField].Value =
               (value ? "1" : "0");

            _showDots = value;
         }
      }

      public static EventsListOverflow EventsOverflow
      {
         get
         {
            return _eventsOverflow;
         }
         set
         {
            CalendarsListRootItem.Fields[EventsOverflowField].Value = value.ToString();

            _eventsOverflow = value;
         }
      }

      public static bool ShowOtherMonth
      {
         get
         {
            return _showOtherMonth;
         }
         set
         {
            CalendarsListRootItem.Fields[ShowOtherMonthField].Value =
               (value ? "1" : "0");

            _showOtherMonth = value;
         }
      }

      public static bool ShowHeader
      {
         get
         {
            return _showHeader;
         }
         set
         {
            CalendarsListRootItem.Fields[ShowHeaderField].Value =
               (value ? "1": "0" );

            _showHeader = value;
         }
      }

      public static bool ShowTime
      {
         get
         {
            return _showTime;
         }
         set
         {
            CalendarsListRootItem.Fields[ShowTimeField].Value =
               (value ? "1" : "0" );

            _showTime = value;
         }
      }

      public static DateFormat DateFormat
      {
         get
         {
            return _dateFormat;
         }
         set
         {
            CalendarsListRootItem.Fields[DateFormatField].Value = value.ToString();

            _dateFormat = value;
         }
      }

      public static Item CalendarsListRootItem
      {
         get
         {
            return _calendarRootItem;
         }
         set
         {
            _calendarRootItem = value;
         }
      }

      public static string AgendaTitle
      {
         get
         {
            return _agendaTitle;
         }
         set
         {
            CalendarsListRootItem.Fields[AgendaTitleField].Value = value;

            _agendaTitle = value;
         }
      }


      public static bool HighlightWeekends
      {
         get
         {
            return _highlightWeekends;
         }
         set
         {
            CalendarsListRootItem.Fields[HighlightWeekendsField].Value =
                ( value ? "1" : "0" );

            _highlightWeekends = value;
         }
      }

      public static bool RenderEventInBlock
      {
         get
         {
            return _renderEventInBlock;
         }
         set
         {
            CalendarsListRootItem.Fields[RenderEventInBlockField].Value = 
               ( value ? "1" : "0");

            _renderEventInBlock = value;
         }
      }

      public static bool ShowAsDropDownList
      {
         get
         {
            return _showAsDropDownList;
         }
         set
         {
            CalendarsListRootItem.Fields[ShowAsDropDownListField].Value =
               ( value ? "1" : "0" );

            _showAsDropDownList = value;
         }
      }

      public static bool ReadOnly
      {
         get
         {
            return _readOnly;
         }
         set
         {
            CalendarsListRootItem.Fields[ReadOnlyField].Value =
               ( value ? "1" : "0");

            _readOnly = value;
         }
      }

      public static string ThemeName
      {
         get
         {
            return _themeName;
         }
         set
         {
            if (!CalendarsListRootItem.Editing.IsEditing)
            {
               CalendarsListRootItem.Editing.BeginEdit();

               CalendarsListRootItem.Fields[ThemeField].Value = value;

               CalendarsListRootItem.Editing.EndEdit();
            }

            _themeName = value;
         }
      }

      public static string Theme
      {
         get
         {
            return string.Format(StyleSheet, _themeName);
         }
      }


      public static SitecoreDatabase Database
      {
         get
         {
            return _database;
         }
      }

      public static string DatabaseName
      {
         get
         {
            return CalendarsListRootItem.Fields[DatabaseField].Value;
         }
         set
         {
            CalendarsListRootItem.Fields[DatabaseField].Value = value;
         }
      }

      public static TemplateItem DayTemplate
      {
         get
         {
            return _dayTemplate;
         }
      }

      public static TemplateItem CalendarTemplate
      {
         get
         {
            return _calendarTemplate;
         }
      }


      public static TemplateItem EventTemplate
      {
         get
         {
            return _eventTemplate;
         }
      }


      public static TemplateItem ScheduleTemplate
      {
         get
         {
            return _scheduleTemplate;
         }
      }


      public static MasterItem EventMaster
      {
         get
         {
            return _eventMaster;
         }
      }

      public static string DefaultDotImage
      {
         get
         {
            return _defaultDot;
         }
         set
         {
            CalendarsListRootItem.Fields[DefaultDotField].Value = value;
            _defaultDot = value;
         }
      }

      public static string CurrentMonthDotImage
      {
         get
         {
            return _currentMonthDot;
         }
         set
         {
            CalendarsListRootItem.Fields[CurrentMonthDotField].Value = value;
            _currentMonthDot = value;
         }
      }


      public static string OtherMonthDotImage
      {
         get
         {
            return _otherMonthDot;
         }
         set
         {
            CalendarsListRootItem.Fields[OtherMonthDotField].Value = value;
            _otherMonthDot = value;
         }
      }

      #endregion
   }


   #region Enumerations

   public enum Month
   {
      None = 0,
      January = 1,
      February = 2,
      March = 3,
      April = 4,
      May = 5,
      June = 6,
      Jule = 7,
      August = 8,
      September = 9,
      October = 10,
      November = 11,
      December = 12
   }


   public enum Recurrence
   {
      Once = 0,
      Daily = 1,
      Weekly = 2,
      Monthy = 3,
      Yearly = 4
   }


   public enum Sequence
   {
      None = -1,
      First = 0,
      Second = 1,
      Third = 2,
      Fourth = 3,
      Last = 4
   }


   [Flags]
   public enum DaysOfWeek
   {
      None = 0,
      Monday = 1,
      Tuesday = 2,
      Wednesday = 4,
      Thursday = 8,
      Friday = 16,
      Saturday = 32,
      Sunday = 64
   }


   public enum View
   {
      Month,
      Week,
      Day,
      Agenda
   }


   public enum LinkBehaviour
   {
      None,
      Redirect,
      ShowWizard,
   }

   public enum EventsOrder
   {
      Ascending,
      Descending,
      OneDayOnly
   }

   public enum DisplayEvent 
   {
      Inline,
      Block
   }

   public enum EventsListOverflow
   {
      ShowScrollbars,
      ShowEllipsis
   }

   public enum ViewCommand
   {
      SwitchToDayView,
      SetCurrentDate
   }

   public enum DateFormat
   {
      DayMonthYear,
      MonthDayYear,
      YearMonthDay,
      YearDayMonth,
      YearMonth,
      MonthYear
   }

   #endregion
}
