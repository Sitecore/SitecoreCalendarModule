using Sitecore.Data.Items;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public abstract class StandardViewSettings
   {
      #region Constants

      public static readonly string CalendarListField = "Calendar List";
      public static readonly string RecurrentEventPrefixField = "Recurrent Event Prefix";
      public static readonly string RecurrentEventSuffixField = "Recurrent Event Suffix";
      public static readonly string ShowAllCalendarsField = "Show All";
      public static readonly string ShowHeaderField = "Show Header";
      public static readonly string SwitchViewModeField = "Switch View Mode";

      #endregion

      #region Attributes

      private Item[] _eventLists = null;
      private string _recurrentEventPrefix = null;
      private string _recurrentEventSuffix = null;
      private bool _showHeader = false;
      private ModuleSettings _siteSpecificSettings = null;
      private bool _switchViewMode = false;

      #endregion

      #region Accessor Methods

      public bool SwitchViewMode
      {
         get
         {
            return _switchViewMode;
         }
         set
         {
            _switchViewMode = value;
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

      public bool ShowHeader
      {
         get
         {
            return _showHeader;
         }
         set
         {
            _showHeader = value;
         }
      }

      public string RecurrentEventPrefix
      {
         get
         {
            return _recurrentEventPrefix;
         }
         set
         {
            _recurrentEventPrefix = value;
         }
      }

      public string RecurrentEventSuffix
      {
         get
         {
            return _recurrentEventSuffix;
         }
         set
         {
            _recurrentEventSuffix = value;
         }
      }

      #endregion

      #region Initialization

      public void Init(Item settings_item, string site_settings_path)
      {
         _siteSpecificSettings = new ModuleSettings(site_settings_path);

         RecurrentEventPrefix = settings_item[RecurrentEventPrefixField];
         RecurrentEventSuffix = settings_item[RecurrentEventSuffixField];
         ShowHeader = MainUtil.GetBool(settings_item[ShowHeaderField], false);
         SwitchViewMode = MainUtil.GetBool(settings_item[SwitchViewModeField], false);

         EventLists = Utilities.GetEventListForView(settings_item, site_settings_path);
      }

      #endregion
   }
}