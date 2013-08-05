namespace Sitecore.Modules.EventCalendar.Shell.Controls.Helper
{
   public class RenderingSettings
   {
      private string id;
      private string placeholderName;
      private string siteSettingsPath;
      private string viewSettingsPath;

      public RenderingSettings(string viewSettingsPath, string placeholderName, string siteSettingsPath)
      {
         this.viewSettingsPath = viewSettingsPath;
         this.placeholderName = placeholderName;
         this.siteSettingsPath = siteSettingsPath;
      }

      public RenderingSettings(string id, string viewSettingsPath, string placeholderName, string siteSettingsPath)
         : this(viewSettingsPath, placeholderName, siteSettingsPath)
      {
         this.id = id;
      }

      public string ID
      {
         get
         {
            return id;
         }
         set
         {
            id = value;
         }
      }

      public string PlaceholderName
      {
         get
         {
            return placeholderName;
         }
         set
         {
            placeholderName = value;
         }
      }

      public string ViewItemPath
      {
         get
         {
            return viewSettingsPath;
         }
         set
         {
            viewSettingsPath = value;
         }
      }

      public string SiteSettingsPath
      {
         get
         {
            return siteSettingsPath;
         }
         set
         {
            siteSettingsPath = value;
         }
      }
   }
}