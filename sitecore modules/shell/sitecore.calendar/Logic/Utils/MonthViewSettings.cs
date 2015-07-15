using System;

using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class MonthViewSettings : ExtendStandartViewSettings
   {
      #region Constants

      public static readonly string AddEventIconField = "Add Event Icon";
      public static readonly string AltAddEventIconField = "Alt Add Event Icon";
      public static readonly string EventLimitField = "Event Limit";
      public static readonly string EventOverflowLinkTextField = "Event Overflow Link Text";
      public static readonly string HighlightWeekendsField = "Highlight Weekends";

      public static readonly string LinkOption = "Show Link";
      public static readonly string MarginField = "Margin";
      public static readonly string ScrollbarOption = "Show Scrollbars";
      public static readonly string ShowEventTimeField = "Show Event Time";

      #endregion

      #region Attributes

      private string _addEventIcon = null;
      private string _altAddEventIcon = null;
      private int _eventLimit = 0;
      private EventsListOverflow _eventOverflowBehavior = EventsListOverflow.ShowEllipsis;
      private string _eventOverflowLinkText = null;
      private bool _highlightWeekends = false;
      private string _margin = null;
      private bool _showEventTime = true;

      #endregion

      #region Accessor Methods

      public string AddEventIcon
      {
         get
         {
            return _addEventIcon;
         }
         set
         {
            _addEventIcon = value;
         }
      }

      public string AltAddEventIcon
      {
         get
         {
            return _altAddEventIcon;
         }
         set
         {
            _altAddEventIcon = value;
         }
      }

      public int EventLimit
      {
         get
         {
            return _eventLimit;
         }
         set
         {
            _eventLimit = value;
         }
      }

      public EventsListOverflow EventOverflowBehavior
      {
         get
         {
            return _eventOverflowBehavior;
         }
         set
         {
            _eventOverflowBehavior = value;
         }
      }

      public string EventOverflowLinkText
      {
         get
         {
            return _eventOverflowLinkText;
         }
         set
         {
            _eventOverflowLinkText = value;
         }
      }

      public bool HighlightWeekends
      {
         get
         {
            return _highlightWeekends;
         }
         set
         {
            _highlightWeekends = value;
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

      public bool ShowEventTime
      {
         get
         {
            return _showEventTime;
         }
         set
         {
            _showEventTime = value;
         }
      }

      #endregion

      #region Initialization Methods

      public MonthViewSettings(Item settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private new void Init(Item settings_item, string site_settings_path)
      {
         if (settings_item == null)
         {
            Log.Warn("Calendar Module: month view settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).MonthViewSettings;
         }

         ImageField addIcon = settings_item.Fields[AddEventIconField];
         AddEventIcon = "~/media" + addIcon.MediaItem.Paths.MediaPath + ".ashx";

         ImageField altIcon = settings_item.Fields[AltAddEventIconField];
         AltAddEventIcon = "~/media" + altIcon.MediaItem.Paths.MediaPath + ".ashx";

         string limit = settings_item[EventLimitField];
         if (limit.Length > 0)
         {
            try
            {
               EventLimit = int.Parse(limit);
            }
            catch (Exception e)
            {
               Log.Warn("Calendar Module Month View Settings: Exception trying to parse Event Limit Field!", this);
               Log.Warn(e.Message, this);
               EventLimit = 0;
            }
         }

         EventOverflowLinkText = settings_item[EventOverflowLinkTextField];

         HighlightWeekends = MainUtil.GetBool(settings_item[HighlightWeekendsField], false);

         Margin = settings_item[MarginField];

         ShowEventTime = MainUtil.GetBool(settings_item[ShowEventTimeField], false);

         base.Init(settings_item, site_settings_path);
      }

      #endregion
   }
}