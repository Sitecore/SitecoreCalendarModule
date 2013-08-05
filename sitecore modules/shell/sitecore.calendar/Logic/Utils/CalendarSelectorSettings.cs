using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class CalendarSelectorSettings
   {
      #region Constants

      public static readonly string CalendarListField = "Calendar List";
      public static readonly string FormatField = "Format";
      public static readonly string LinkTextField = "Link Text";
      public static readonly string MarginField = "Margin";
      public static readonly string ShowAllCalendarsField = "Show All";
      public static readonly string TitleField = "Title";

      #endregion

      #region Attributes

      private Item[] _eventLists = null;
      private string _format = null;
      private string _linkText = null;
      private string _margin = null;
      private ModuleSettings _siteSpecificSettings = null;
      private string _title = null;

      #endregion

      #region Accessor Methods

      public string Format
      {
         get
         {
            return _format;
         }
         set
         {
            _format = value;
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

      public string LinkText
      {
         get
         {
            return _linkText;
         }
         set
         {
            _linkText = value;
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

      public CalendarSelectorSettings(BaseItem settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private void Init(BaseItem settings_item, string site_settings_path)
      {
         _siteSpecificSettings = new ModuleSettings(site_settings_path);

         if (settings_item == null)
         {
            Log.Warn("Calendar Module: calendar selector settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).CalendarSelectorSettings;
         }

         Format = settings_item[FormatField];
         LinkText = settings_item[LinkTextField];
         Margin = settings_item[MarginField];
         Title = settings_item[TitleField];

         Item[] event_lists;
         if (MainUtil.GetBool(settings_item[ShowAllCalendarsField], false))
         {
            event_lists = new Item[1];
            event_lists[0] = _siteSpecificSettings.CalendarEventsRoot;
         }
         else
         {
            MultilistField calendarListFld = settings_item.Fields[CalendarListField];
            event_lists = calendarListFld.GetItems();
         }
         EventLists = event_lists;
      }

      #endregion
   }
}