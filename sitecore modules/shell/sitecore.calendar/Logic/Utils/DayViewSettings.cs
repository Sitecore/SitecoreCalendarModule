using System;

using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class DayViewSettings : ExtendStandartViewSettings
   {
      #region Constants

      public static readonly string HeaderFormatField = "Header Format";
      public static readonly string MarginField = "Margin";
      public static readonly string NumHoursDisplayedField = "Number of Hours Displayed";
      public static readonly string ReturnToMonthViewTextField = "Return to Month View";
      public static readonly string ReturnToWeekViewTextField = "Return to Week View";
      public static readonly string StartHourField = "Start Hour";

      #endregion

      #region Attributes

      private string _headerFormat = null;
      private string _margin = null;
      private int _numHoursDisplayed = 8;
      private string _returnToMonthViewText = "month";
      private string _returnToWeekViewText = "week";
      private int _startHour = 0;

      #endregion

      #region Accessor Methods

      public string ReturnToWeekViewText
      {
         get
         {
            return _returnToWeekViewText;
         }
         set
         {
            _returnToWeekViewText = value;
         }
      }

      public string ReturnToMonthViewText
      {
         get
         {
            return _returnToMonthViewText;
         }
         set
         {
            _returnToMonthViewText = value;
         }
      }

      public string HeaderFormat
      {
         get
         {
            return _headerFormat;
         }
         set
         {
            _headerFormat = value;
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

      public int NumberHoursDisplayed
      {
         get
         {
            return _numHoursDisplayed;
         }
         set
         {
            _numHoursDisplayed = value;
         }
      }

      public int StartHour
      {
         get
         {
            return _startHour;
         }
         set
         {
            _startHour = value;
         }
      }

      #endregion

      #region Initialization Methods

      public DayViewSettings(Item settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private new void Init(Item settings_item, string site_settings_path)
      {
         if (settings_item == null)
         {
            Log.Warn("Calendar Module: day view settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).DayViewSettings;
         }

         HeaderFormat = settings_item[HeaderFormatField];
         if (string.IsNullOrEmpty(HeaderFormat))
         {
            HeaderFormat = "MM/dd/yyyy";
         }

         Margin = settings_item[MarginField];
         string num_hours = settings_item[NumHoursDisplayedField];
         if (num_hours.Length > 0)
         {
            try
            {
               NumberHoursDisplayed = int.Parse(num_hours);
            }
            catch (Exception e)
            {
               Log.Warn("Calendar Module Day View Settings: Exception trying to parse Num Hours Displayed Field!", this);
               Log.Warn(e.Message, this);
            }
         }
         _startHour = int.Parse(settings_item[StartHourField]);
         _returnToMonthViewText = settings_item[ReturnToMonthViewTextField];
         _returnToWeekViewText = settings_item[ReturnToWeekViewTextField];
         base.Init(settings_item, site_settings_path);
      }

      #endregion
   }
}