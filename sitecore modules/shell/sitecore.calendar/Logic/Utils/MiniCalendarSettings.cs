using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class MiniCalendarSettings
   {
      #region Constants

      public static readonly string CalendarListField = "Calendar List";
      public static readonly string LinkBehaviorField = "Link Behavior";
      public static readonly string MarginField = "Margin";
      public static readonly string RedirectToField = "Redirect To";
      public static readonly string ShowAllCalendarsField = "Show All";
      public static readonly string TitleField = "Title";

      #endregion

      #region Attributes

      private Item[] _eventLists = null;
      private string _linkBehavior = null;
      private string _margin = null;
      private string _redirectTo = null;
      private ModuleSettings _siteSpecificSettings = null;
      private string _title = null;

      #endregion

      #region Accessor Methods

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

      public string LinkBehavior
      {
         get
         {
            return _linkBehavior;
         }
         set
         {
            _linkBehavior = value;
         }
      }

      public string RedirectTo
      {
         get
         {
            return _redirectTo;
         }
         set
         {
            _redirectTo = value;
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

      public MiniCalendarSettings(BaseItem settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private void Init(BaseItem settings_item, string site_settings_path)
      {
         if (settings_item == null)
         {
            Log.Warn("Calendar Module: mini calendar settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).MiniCalendarSettings;
         }

         _siteSpecificSettings = new ModuleSettings(site_settings_path);

         LinkBehavior = settings_item[LinkBehaviorField];
         LinkField redirect_field = settings_item.Fields[RedirectToField];

         Item target_item = redirect_field.TargetItem;

         if (target_item != null)
         {
            if (redirect_field.IsMediaLink)
            {
               MediaItem media = new MediaItem(target_item);
               RedirectTo = "~/" + MediaManager.GetMediaUrl(media);
            }
            else if (redirect_field.IsInternal)
            {
               RedirectTo = LinkManager.GetItemUrl(target_item);
            }
            else 
            {
               RedirectTo = redirect_field.Url;
            }
         }
         else
         {
            RedirectTo = redirect_field.Url;
         }

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