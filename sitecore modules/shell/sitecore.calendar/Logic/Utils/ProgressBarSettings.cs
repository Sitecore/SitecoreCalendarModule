using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public class ProgressBarSettings
   {
      #region Constants

      public static readonly string ImageField = "Image";
      public static readonly string TextField = "Text";

      #endregion

      #region Attributes

      private ImageField _image = null;
      private string _text = null;

      #endregion

      #region Accessor Methods

      public string Image
      {
         get
         {
            if (_image != null)
            {
               return StaticSettings.MediaPrefix + _image.MediaItem.Paths.MediaPath + StaticSettings.MediaSuffix;
            }
            return "";
         }
      }

      public string Text
      {
         get
         {
            return _text;
         }
         set
         {
            _text = value;
         }
      }

      #endregion

      #region Initialization Methods

      public ProgressBarSettings(BaseItem settings_item, string site_settings_path)
      {
         Init(settings_item, site_settings_path);
      }

      private void Init(BaseItem settings_item, string site_settings_path)
      {
         if (settings_item == null)
         {
            Log.Warn("Calendar Module: progress bar settings item isn't exist", this);
            settings_item = new ModuleSettings(site_settings_path).ProgressBarSettings;
         }

         _image = settings_item.Fields[ImageField];
         _text = settings_item[TextField];
      }

      #endregion
   }
}