using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class ModuleSettings
   {
      #region Constants

      public static readonly string AgendaViewField = "Agenda View";
      public static readonly string CalendarSelectorField = "Calendar Selector";
      public static readonly string DateSelectorField = "Date Selector";
      public static readonly string DayViewField = "Day View";
      public static readonly string EventListsFolderField = "Event Lists Folder";
      public static readonly string EventSourceDatabaseField = "Event Source Database";
      public static readonly string EventTargetDatabaseField = "Event Target Database";
      public static readonly string MiniCalendarField = "Mini Calendar";
      public static readonly string ModuleDefaultSettingsItemPath = "/sitecore/system/modules/calendar";
      public static readonly string MonthViewField = "Month View";
      public static readonly string ProgressBarField = "Progress Bar";
      public static readonly string SchedulesFolderField = "Schedules Folder";
      public static readonly string SettingsSettingsViewField = "Standart View";
      public static readonly string WeekViewField = "Week View";

      #endregion

      #region Private Variables

      private readonly string settingsItemPath = ModuleDefaultSettingsItemPath;

      #endregion

      #region Accessor Methods

      public string ModuleSettingsItemPath
      {
         get
         {
            return settingsItemPath;
         }
      }

      public Item CalendarEventsRoot
      {
         get
         {
            return GetSettings(EventListsFolderField);
         }
      }

      public Item SchedulesRoot
      {
         get
         {
            return GetSettings(SchedulesFolderField);
         }
      }

      public Item AgendaViewSettings
      {
         get
         {
            Item settings = GetSettings("Agenda View");
            return (settings ?? new ModuleSettings(null).AgendaViewSettings);
         }
      }

      public Item DateSelectorSettings
      {
         get
         {
            Item settings = GetSettings("Date Selector");
            return (settings ?? new ModuleSettings(null).DateSelectorSettings);
         }
      }

      public Item DayViewSettings
      {
         get
         {
            Item settings = GetSettings("Day View");
            return (settings ?? new ModuleSettings(null).DayViewSettings);
         }
      }

      public Item MiniCalendarSettings
      {
         get
         {
            Item settings = GetSettings("Mini Calendar");
            return (settings ?? new ModuleSettings(null).MiniCalendarSettings);
         }
      }

      public Item MonthViewSettings
      {
         get
         {
            Item settings = GetSettings("Month View");
            return (settings ?? new ModuleSettings(null).MonthViewSettings);
         }
      }

      public Item WeekViewSettings
      {
         get
         {
            Item settings = GetSettings("Week View");
            return (settings ?? new ModuleSettings(null).WeekViewSettings);
         }
      }

      public Item CalendarSelectorSettings
      {
         get
         {
            Item settings = GetSettings("Calendar Selector");
            return (settings ?? new ModuleSettings(null).CalendarSelectorSettings);
         }
      }

      public Item ProgressBarSettings
      {
         get
         {
            Item settings = GetSettings("Progress Bar");
            return (settings ?? new ModuleSettings(null).ProgressBarSettings);
         }
      }

      public ID SiteID
      {
         get
         {
            return GetActiveDatabase().GetItem(settingsItemPath).ID;
         }
      }

      #endregion

      private Item GetSettings(string fieldName)
      {
         var settings = GetSettingsItem();
         return settings.Database.GetItem(settings[fieldName]);
      }

      public Item GetSettingsItem()
      {
         var database = StaticSettings.EventSourceDatabase.Database;

         if (database != null)
         {
            return database.GetItem(settingsItemPath);
         }

         SitecoreDatabase masterDB = StaticSettings.ContextDatabase;
         return masterDB.GetItem(settingsItemPath);
      }

      public Database GetActiveDatabase()
      {
         return StaticSettings.EventSourceDatabase.Database ?? StaticSettings.ContextDatabase.Database;
      }

      #region Initialization Methods

      public ModuleSettings(string settings_item_path)
      {
         if (!(string.IsNullOrEmpty(settings_item_path)))
         {
            if (StaticSettings.EventSourceDatabase.Database.GetItem(settings_item_path) != null ||
                StaticSettings.ContextDatabase.GetItem(settings_item_path) != null)
            {
               settingsItemPath = settings_item_path;
            }
         }
      }

      #endregion
   }
}