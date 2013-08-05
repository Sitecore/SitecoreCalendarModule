using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class DateSelectorSettings
   {
      #region Constants

      public static readonly string FormatField = "Format";
      public static readonly string LinkTextField = "Link Text";
      public static readonly string MarginField = "Margin";
      public static readonly string ShowAllCalendarsField = "Show All";
      public static readonly string TitleField = "Title";
      public static readonly string TodayLinkTextField = "Today Link Text";

      #endregion

      #region Attributes

      private string _format = null;
      private string _linkText = null;
      private string _margin = null;
      private string _title = null;
      private string _todayLinkText = null;

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

      public string TodayLinkText
      {
         get
         {
            return _todayLinkText;
         }
         set
         {
            _todayLinkText = value;
         }
      }

      #endregion

      #region Initialization Methods

      public DateSelectorSettings(BaseItem settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private void Init(BaseItem settings_item, string site_settings_path)
      {
         if (settings_item == null)
         {
            Log.Warn("Calendar Module: date selector settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).CalendarSelectorSettings;
         }

         Format = settings_item[FormatField];
         LinkText = settings_item[LinkTextField];
         Margin = settings_item[MarginField];
         Title = settings_item[TitleField];
         TodayLinkText = settings_item[TodayLinkTextField];
      }

      #endregion
   }
}