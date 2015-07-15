using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Core;
using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.SecurityModel;
using System;

namespace Sitecore.Modules.EventCalendar.Configuration
{
   public static class DemoItems
   {
      public const int countMonth = 1;
      public const DayOfWeek DeveloperDay = DayOfWeek.Tuesday;
      public const int DeveloperResourceTitleCount = 2;

      public const int DeveloperTitles = 2;
      public const DayOfWeek EndUserDay = DayOfWeek.Wednesday;
      public const int EndUserResourceTitleCount = 3;
      public const int EndUserTitles = 3;
      public const int LocationResourceCount = 5;
      public const int LunchResourceDescriptionCount = 3;
      public const int LunchResourceLocationCount = 3;
      public const int LunchResourceTitleCount = 3;
      public const int PartyResourceDescriptionCount = 6;
      public const int PartyResourceLocationCount = 6;
      public const int PartyResourceTitleCount = 6;
      public static readonly string CalendarEventsRoot = "{C332D9C3-B3D8-4D30-A174-C706B3346407}";
      public static readonly string DefaultCalendarName = "Default Calendars";
      public static readonly string DefaultEventListName = "Default Calendar";
      public static readonly string DefaultModuleSettings = "{C337C648-8385-4873-8369-6D928EF196F3}";

      public static readonly string DescriptionDefault = "";

      public static readonly string descriptionToday =
         "The Sitecore Calendar module has been installed ";

      public static readonly string DeveloperDefaultTitle = "Developer Training";

      public static readonly string DeveloperResourceTitle = "DveloperTitle";
      public static readonly string DeveloperTrainingList = "{5C2BDD0F-29A5-44BF-8133-EB8432F3F375}";
      public static readonly string DeveloperViewSettings = "{D42E746E-9F74-449D-B950-20ECEA16E470}";
      public static readonly string EndUserDefaultTitle = "End User Training";
      public static readonly string EndUserLocationDefault = "Training Room 101";
      public static readonly string EndUserResourceTitle = "EndUserTitle";
      public static readonly string EndUserTrainingList = "{8E385285-F09E-48D2-A1CD-4DDBD8C55FB2}";
      public static readonly string LocationDefault = "Training Room 100";
      public static readonly string LocationResource = "Location";
      public static readonly string LunchDefaultTitle = "Lunch";
      public static readonly string LunchList = "{D9F2ACF2-A463-4BAE-9A08-16BDD8591BF9}";

      public static readonly string LunchResourceDescription = "LunchDescription";
      public static readonly string LunchResourceEndTime = "LunchEndTime";
      public static readonly string LunchResourceLocation = "LunchLocation";
      public static readonly string LunchResourceStartDate = "LunchStartDate";
      public static readonly string LunchResourceStartTime = "LunchStartTime";
      public static readonly string LunchResourceTitle = "Lunch";
      public static readonly string PartyDefaultTitle = "Party";
      public static readonly string PartyList = "{0B14849C-A46B-433A-B835-C9A18692C1F2}";
      public static readonly string PartyResourceDescription = "PartyDescription";
      public static readonly string PartyResourceEndTime = "PartyEndTime";
      public static readonly string PartyResourceLocation = "PartyLocation";
      public static readonly string PartyResourceStartDate = "PartyStartDate";
      public static readonly string PartyResourceStartTime = "PartyStartTime";
      public static readonly string PartyResourceTitle = "Party";
      private static readonly Random random = new Random();
      public static readonly string titleToday = "Today";

      public static readonly string TrainingMasterID = "{B51C6447-670C-4FF8-9FD8-84E78F9E7C10}";

      private static int offsetHours;

      public static void CreateEvents()
      {
         var module = new ModuleSettings(null);

         var eventManager = new EventListManager(new Item[1] {module.CalendarEventsRoot}, null);

         using (new SecurityDisabler())
         {
            CreateDeveloperTrainingEvents(eventManager);
            CreateEndUserTrainingEvents(eventManager);
            CreatePartiesEvents(eventManager);
            CreateLunchEvents(eventManager);
         }
      }

      public static void CreateDefaultCalendarsList()
      {
         Database master = StaticSettings.MasterDatabase.Database;
         Item parent = master.GetItem(CalendarEventsRoot);

         if (!IsFindChild(parent, CalendarIDs.CalendarListBranch))
         {
            BranchItem branch = master.Branches[CalendarIDs.CalendarListBranch];
            parent = Context.Workflow.AddItem(DefaultCalendarName, branch, parent);

            if (!IsFindChild(parent, CalendarIDs.EventListBranch))
            {
               branch = master.Branches[CalendarIDs.EventListBranch];
               Context.Workflow.AddItem(DefaultEventListName, branch, parent);
            }
         }
      }

      private static bool IsFindChild(Item parent, ID branch)
      {
         foreach (Item child in parent.Children)
         {
            if (child.Branch != null && branch == child.BranchId)
            {
               return true;
            }
         }
         return false;
      }

      private static DateTime FindNextDayOfWeek(DateTime date, DayOfWeek day)
      {
         while (date.DayOfWeek != day)
         {
            date = date.AddDays(1);
         }
         return date;
      }

      private static void NewEvent(EventDetails details, bool useOrdinalInformation, int pos)
      {
         var evt = new Event();

         if (pos == -1)
         {
            pos = Math.Min(Math.Min(details.TitleCount, details.LocationCount),
                           details.DescriptionCount);
         }

         evt.Name = GetResourceString(details.Title,
                                      useOrdinalInformation ? pos : details.TitleCount,
                                      details.DefaultTitle, !useOrdinalInformation);
         evt.Title = evt.Name;

         evt.Location = GetResourceString(details.Location,
                                          useOrdinalInformation ? pos : details.LocationCount,
                                          details.DefaultLocation, !useOrdinalInformation);
         evt.Description = GetResourceString(details.Description,
                                             useOrdinalInformation ? pos : details.DescriptionCount,
                                             details.DefaultDescription, !useOrdinalInformation);

         evt.StartDate = Utilities.NormalizeDate(details.Date);
         evt.EndDate = Utilities.NormalizeDate(details.Date);

         evt.StartTime = details.StartTime;
         evt.EndTime = details.EndTime;

         details.List.CreateEvent(evt, details.Branch);
      }

      private static string GetResourceString(string name, int count, string defaultValue,
                                              bool useRandom)
      {
         if (string.IsNullOrEmpty(name))
         {
            return defaultValue;
         }

         int rand = useRandom ? random.Next(count) : count;
         string value = ResourceManager.Localize(name + rand);

         if (!string.IsNullOrEmpty(value))
         {
            return value;
         }

         return defaultValue;
      }

      private static void CreateDeveloperTrainingEvents(EventListManager eventManager)
      {
         DateTime start = FindNextDayOfWeek(DateTime.Today, DeveloperDay);

         var evt = new Event
         	{
                Description = string.Empty,
                StartDate = Utilities.NormalizeDate(start),
                EndDate = Utilities.NormalizeDate(start.AddMonths(2)),
                StartTime = "09:00",
                EndTime = "17:00",
                Location = LocationDefault,
                Title = DeveloperDefaultTitle
            };

         var scheduler = new Schedule
                            {
                               DaysOfWeek = (Utils.DaysOfWeek.Tuesday | Utils.DaysOfWeek.Thursday),
                               StartDate = Utilities.NormalizeDate(start),
                               EndDate = Utilities.NormalizeDate(start.AddMonths(2)),
                               Recurrence = Recurrence.Weekly,
                               Frequency = 1
                            };

         var options = new Options
                          {
                             CalendarID = DeveloperTrainingList,
                             ControlSettingsPath = DeveloperViewSettings,
                             SiteSettingsPath = DefaultModuleSettings
                          };

         CalendarActions.CreateEvent(evt, scheduler, options);
      }

      private static void CreateEndUserTrainingEvents(EventListManager eventManager)
      {
         DateTime start = FindNextDayOfWeek(DateTime.Today, DeveloperDay);

         var evt = new Event
            {
                Description = string.Empty,
                StartDate = Utilities.NormalizeDate(start),
                EndDate = Utilities.NormalizeDate(start.AddMonths(2)),
                StartTime = "09:00",
                EndTime = "17:00",
                Location = EndUserLocationDefault,
                         Title = EndUserDefaultTitle
            };

         var scheduler = new Schedule
                            {
                               DaysOfWeek = (Utils.DaysOfWeek.Monday | Utils.DaysOfWeek.Friday),
                               StartDate = Utilities.NormalizeDate(start),
                               EndDate = Utilities.NormalizeDate(start.AddMonths(2)),
                               Recurrence = Recurrence.Weekly,
                               Frequency = 1
                            };

         var options = new Options
                          {
                             CalendarID = EndUserTrainingList,
                             ControlSettingsPath = DeveloperViewSettings,
                             SiteSettingsPath = DefaultModuleSettings
                          };

         CalendarActions.CreateEvent(evt, scheduler, options);
      }

      private static void CreatePartiesEvents(EventListManager eventManager)
      {
         var details = new EventDetails();
         details.List = eventManager.GetCalendar(PartyList);

         details.Title = PartyResourceTitle;
         details.TitleCount = PartyResourceTitleCount;
         details.Location = PartyResourceLocation;
         details.LocationCount = PartyResourceLocationCount;
         details.Description = PartyResourceDescription;
         details.DescriptionCount = PartyResourceDescriptionCount;
         details.DefaultDescription = PartyDefaultTitle;
         details.DefaultLocation = DescriptionDefault;
         details.DefaultDescription = DescriptionDefault;
         details.Branch = StaticSettings.EventBranch;

         for (int i = 0; i < PartyResourceTitleCount; ++i)
         {
            details.StartTime = GetResourceString(PartyResourceStartTime, i, "13:00", false);
            details.EndTime = GetResourceString(PartyResourceEndTime, i, "14:00", false);

            string[] date = GetResourceString(PartyResourceStartDate, i, "", false).Split('|');
            var day = (DayOfWeek) Enum.Parse(typeof (DayOfWeek), date[0], true);
            int skipDays = 7 * (int.Parse(date[1]) - 1);
            details.Date = FindNextDayOfWeek(DateTime.Today, day).AddDays(skipDays);

            NewEvent(details, true, i);
         }
         ++offsetHours;
      }

      private static void CreateLunchEvents(EventListManager eventManager)
      {
         DateTime start = DateTime.Today;

         var details = new EventDetails();
         details.List = eventManager.GetCalendar(LunchList);

         details.Title = LunchResourceTitle;
         details.TitleCount = LunchResourceTitleCount;
         details.Location = LunchResourceLocation;
         details.LocationCount = LunchResourceLocationCount;
         details.Description = LunchResourceDescription;
         details.DescriptionCount = LunchResourceDescriptionCount;
         details.DefaultDescription = LunchDefaultTitle;
         details.DefaultLocation = DescriptionDefault;
         details.DefaultDescription = DescriptionDefault;
         details.Date = start;
         details.Branch = StaticSettings.EventBranch;

         for (int i = 0; i < LunchResourceTitleCount; ++i)
         {
            details.StartTime = GetResourceString(LunchResourceStartTime, i, "12:00", false);
            details.EndTime = GetResourceString(LunchResourceEndTime, i, "13:00", false);

            string[] date = GetResourceString(LunchResourceStartDate, i, "", false).Split('|');
            var day = (DayOfWeek) Enum.Parse(typeof (DayOfWeek), date[0], true);
            int skipDays = 7 * (int.Parse(date[1]) - 1);
            details.Date = FindNextDayOfWeek(DateTime.Today, day).AddDays(skipDays);

            NewEvent(details, true, i);
         }
         ++offsetHours;
      }
   }
}