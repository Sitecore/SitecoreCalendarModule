using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class ExtendStandartViewSettings : StandardViewSettings
   {
      #region Constants

      public static readonly string EventMasterField = "Event Branch";

      #endregion

      private BranchItem _eventBranch;
      private Item _settingsItem = null;

      public string ID
      {
         get
         {
            return _settingsItem.ID.ToString();
         }
      }

      public Item Item
      {
         get
         {
            return _settingsItem;
         }
      }

      public BranchItem EventBranch
      {
         get
         {
            return _eventBranch ?? StaticSettings.EventBranch;
         }
      }

      #region Initialization Methods

      protected void init(Item settings_item, string site_settings_path)
      {
         base.Init(settings_item, site_settings_path);
      }

      public new void Init(Item settings_item, string site_settings_path)
      {
         _eventBranch = ((LinkField) (settings_item.Fields[EventMasterField])).TargetItem;
         _settingsItem = settings_item;

         base.Init(settings_item, site_settings_path);
      }

      #endregion
   }
}