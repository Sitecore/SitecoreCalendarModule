using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class EventCommandContext : CommandContext
   {
      #region Fields

      private BranchItem branch;

      #endregion

      #region Methods

      public EventCommandContext(Event evt, Schedule schedule, Options options) : this(evt, schedule, options, null)
      {
      }

      public EventCommandContext(Event evt, Schedule schedule, Options options, BranchItem branch)
      {
         Assert.ArgumentNotNull(evt, "evt");

         Event = evt;
         Schedule = schedule;
         Options = options;
         this.branch = branch;
      }

      #endregion

      #region Properties

      public ExtendStandartViewSettings ViewSettings
      {
         get
         {
            var exSettings = new ExtendStandartViewSettings();
            exSettings.Init(SettingsItem, SiteSettingsPath);

            return exSettings;
         }
      }

      public EventListManager EventListManager
      {
         get
         {
            if (SettingsItem != null && !string.IsNullOrEmpty(SiteSettingsPath))
            {
               Item[] eventList = Utilities.GetEventListForView(SettingsItem, SiteSettingsPath);
               return new EventListManager(eventList, SiteSettingsPath);
            }

            return null;
         }
      }

      public BranchItem Branch
      {
         get
         {
            if (branch != null)
            {
               return branch;
            }

            return ViewSettings.EventBranch;
         }
      }

      public EventList EventList
      {
         get
         {
            var mngr = EventListManager;
            if (mngr != null)
            {
                return mngr.EventLists[CalendarID];
            }

            return new EventList(StaticSettings.EventTargetDatabase.GetItem(CalendarID));
         }
      }

      public Item SettingsItem
      {
         get
         {
            return StaticSettings.EventTargetDatabase.SelectSingleItem(ControlSettingsPath);
         }
      }

      public string ControlSettingsPath
      {
         get
         {
            return Options.ControlSettingsPath;
         }
      }

      public string SiteSettingsPath
      {
         get
         {
            return Options.SiteSettingsPath;
         }
      }

      public string CalendarID
      {
         get
         {
            return Options.CalendarID;
         }
      }

      public Event Event { get; private set; }

      public Schedule Schedule { get; private set; }

      public Options Options { get; set; }

      #endregion
   }
}