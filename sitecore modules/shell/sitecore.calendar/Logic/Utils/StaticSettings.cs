using System;
using System.Web;

using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Core;
using Sitecore.Sites;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public static class StaticSettings
   {
      #region Constants

      public const char DateSeparator = '/';
      public const int HoursSpan = 1;
      public const int MinutesSpan = 30;

      public static readonly string ModulePathUrl = "/sitecore modules/shell/sitecore.calendar";
      public static readonly string BlankGIF = ModulePathUrl + @"/Images/blank.gif";
      public static readonly string EventWizardPath = "~" + ModulePathUrl + @"/controls/EventWizard.ascx";
      public static readonly string MediaPrefix = "/~/media";
      public static readonly string MediaSuffix = ".ashx";

      internal static readonly string ResourceName = "Sitecore.Modules.EventCalendar.Properties.Resources";
      public static readonly string SessionStateCalendarsList = "scEventCalendarCalendarList";
      public static readonly string SessionStateCurrentDate = "scEventCalendarCurrentDate";
      public static readonly string SessionStateUrl = "scEventCalendarUrl";

      public static readonly string DefaultServiceReference =
         "/sitecore modules/Web/Sitecore.Calendar/Logic/Services/WebService.asmx";

      public static readonly string SitecoreRoot = "/sitecore/content";
      public static readonly string StarterKitRevision = SitecoreRoot + "/Meta-Data/Starter Kit Revision";

      public static readonly string StarterKitRoot = SitecoreRoot + "/home";
      public static readonly string StyleSheet = ModulePathUrl + @"/themes/{0}/{0}.css";

      public static readonly string ThemeField = "Theme";
      public static readonly string ViewModeProperty = "ViewMode";

      public static readonly ID DefaultDeviceID = new ID("{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}");

      #endregion

      #region Variables

      private static readonly string additionalFields = Settings.GetSetting("Calendar.AdditionalFields");
      private static readonly TemplateItem dayTemplate = null;

      public static readonly string ScriptManagerID = Guid.NewGuid().ToString();

      public static Item calendarRootItem = null;
      private static SitecoreDatabase eventSourceDatabase = null;
      private static SitecoreDatabase eventTargetDatabase = null;
      private static SitecoreDatabase masterDatabase = null;

      #endregion

      #region Initialization

      static StaticSettings()
      {
         Init();
      }

      public static bool IsStarterKit
      {
         get
         {
            return ContextDatabase.Database.GetItem(StarterKitRevision) != null;
         }
      }

      public static void Init()
      {
         masterDatabase = new SitecoreDatabase(Factory.GetDatabase("master", false));
      }

      #endregion

      #region Properties

      public static string ServiceReference
      {
         get
         {
            return Settings.GetSetting("calendarService", DefaultServiceReference);
         }
      }

      public static string[] AdditionalFields
      {
         get
         {
            if (string.IsNullOrEmpty(additionalFields))
            {
               return new string[0] {};
            }
            return additionalFields.Split('|');
         }
      }

      public static string Theme
      {
         get
         {
            if (HttpContext.Current.Session["theme"] != null)
            {
               return string.Format(StyleSheet, HttpContext.Current.Session["theme"]);
            }

            Item settings;
            if (IsStarterKit)
            {
               settings = ContextDatabase.SelectSingleItem(StarterKitRoot);
               return
                  string.Format(StyleSheet, ContextDatabase.GetItem(ID.Parse(settings.Fields[ThemeField].Value)).Name);
            }

            settings = ContextDatabase.SelectSingleItem(ModuleSettings.ModuleDefaultSettingsItemPath);
            return string.Format(StyleSheet, settings.Fields[ThemeField].Value);
         }
      }

      public static SitecoreDatabase EventSourceDatabase
      {
         get
         {
            if (Context.Site.DisplayMode == DisplayMode.Preview || Context.Site.DisplayMode == DisplayMode.Edit)
            {
               Assert.IsNotNull(EventTargetDatabase, "The \"Target\" database is null.");

               return EventTargetDatabase;
            }

            string sourceDb = StandardText.Get(CalendarIDs.SourceDatabase);

            if (eventSourceDatabase == null || eventSourceDatabase.Database.Name != sourceDb)
            {
               eventSourceDatabase = new SitecoreDatabase(sourceDb);
            }
            return eventSourceDatabase;
         }
         set
         {
            eventSourceDatabase = value;
         }
      }

      public static SitecoreDatabase EventTargetDatabase
      {
         get
         {
            string targetDb = StandardText.Get(CalendarIDs.TargetDatabase);

            if (eventTargetDatabase == null || eventTargetDatabase.Database.Name != targetDb)
            {
               try
               {
                  eventTargetDatabase = new SitecoreDatabase(targetDb);
               }
               catch
               {
                  eventTargetDatabase = null;
               }
            }
            return eventTargetDatabase;
         }
         set
         {
            eventTargetDatabase = value;
         }
      }

      public static SitecoreDatabase MasterDatabase
      {
         get
         {
            return masterDatabase;
         }
      }

      public static SitecoreDatabase ContextDatabase
      {
         get
         {
            if (Context.Database != null)
            {
               if (Context.Database.Name == "core" && Context.ContentDatabase != null)
               {
                  return new SitecoreDatabase(Context.ContentDatabase);
               }

               return new SitecoreDatabase(Context.Database);
            }
            Database database = Factory.GetDatabase("web", false);
            if (database == null)
            {
               return new SitecoreDatabase(Factory.GetDatabase("master"));
            }
            return new SitecoreDatabase(database);
         }
      }

      public static TemplateItem DayTemplate
      {
         get
         {
            return dayTemplate;
         }
      }

      public static TemplateItem EventTemplate
      {
         get
         {
            return ContextDatabase.GetItem(CalendarIDs.EventTemplate);
         }
      }

      public static TemplateItem ScheduleTemplate
      {
         get
         {
            return ContextDatabase.GetItem(CalendarIDs.ScheduleTemplate);
         }
      }

      public static BranchItem EventBranch
      {
         get
         {
            return ContextDatabase.GetItem(CalendarIDs.EventBranch);
         }
      }

      public static BranchItem YearBranch
      {
         get
         {
            return ContextDatabase.GetItem(CalendarIDs.YearBranch);
         }
      }

      public static BranchItem MonthBranch
      {
         get
         {
            return ContextDatabase.GetItem(CalendarIDs.MonthBranch);
         }
      }

      public static BranchItem DayBranch
      {
         get
         {
            return ContextDatabase.GetItem(CalendarIDs.DayBranch);
         }
      }

      #endregion
   }
}