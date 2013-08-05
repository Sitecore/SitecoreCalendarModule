using System;

using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class WeekViewSettings : ExtendStandartViewSettings
   {
      #region Constants

      public static readonly string HeaderFormatField = "Header Format";
      public static readonly string MarginField = "Margin";
      public static readonly string NumHoursDisplayedField = "Number of Hours Displayed";
      public static readonly string StartHourField = "Start Hour";

      #endregion

      #region Attributes

      private string headerFormat = null;
      private string margin = null;
      private int numHoursDisplayed = 8;
      private int startHour = 0;

      #endregion

      #region Accessor Methods

      public string HeaderFormat
      {
         get
         {
            return headerFormat;
         }
         set
         {
            headerFormat = value;
         }
      }

      public string Margin
      {
         get
         {
            return margin;
         }
         set
         {
            margin = value;
         }
      }

      public int NumberHoursDisplayed
      {
         get
         {
            return numHoursDisplayed;
         }
         set
         {
            numHoursDisplayed = value;
         }
      }

      public int StartHour
      {
         get
         {
            return startHour;
         }
         set
         {
            startHour = value;
         }
      }

      #endregion

      #region Initialization Methods

      public WeekViewSettings(Item settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private new void Init(Item settings_item, string site_settings_path)
      {
         if (settings_item == null)
         {
            Log.Warn("Calendar Module: week view settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).WeekViewSettings;
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
               Log.Warn("Calendar Module Week View Settings: Exception trying to parse Num Hours Displayed Field!", this);
               Log.Warn(e.Message, this);
            }
         }
         startHour = int.Parse(settings_item[StartHourField]);
         base.Init(settings_item, site_settings_path);
      }

      #endregion
   }
}