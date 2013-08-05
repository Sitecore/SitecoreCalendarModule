using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
   public abstract class ExtendBaseView : BaseView
   {
      #region Attributes

      private ExtendStandartViewSettings settings;

      #endregion

      #region Accessor Methods

      public ExtendStandartViewSettings ViewSettings
      {
         get
         {
            return settings;
         }
      }

      #endregion

      #region Initialization Methods

      protected void init(ExtendStandartViewSettings view_settings, string site_settings_path)
      {
         settings = view_settings;
         base.init(view_settings, site_settings_path);
      }

      protected virtual void SetStandartSettings(Item settingsItem, string site_settings_path)
      {
         settings.Init(settingsItem, site_settings_path);
         init(settings, site_settings_path);
      }

      public bool IsDefaultSettingsItem(Item settings, string sitepath)
      {
         ModuleSettings moduleSettings = new ModuleSettings(sitepath);
         ExtendStandartViewSettings exSettings = new ExtendStandartViewSettings();

         exSettings.Init(settings, sitepath);

         return
            (exSettings.ID == moduleSettings.DayViewSettings.ID.ToString() ||
             exSettings.ID == moduleSettings.WeekViewSettings.ID.ToString()) &&
            !(exSettings.SwitchViewMode ||
              (exSettings.ShowHeader && exSettings.ID == moduleSettings.WeekViewSettings.ID.ToString()));
      }

      #endregion
   }
}