using System;
using System.Collections.Specialized;
using System.IO;

using Sitecore.Configuration;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Install;
using Sitecore.Install.Framework;
using Sitecore.Jobs.AsyncUI;
using Sitecore.Modules.EventCalendar.Configuration;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Applications.Install;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.EventCalendar.Configure
{
   public class AddInstall : IPostStep
   {
      public void Run(ITaskOutput output, NameValueCollection metaData)
      {
         string packageName = ResourceManager.Localize("DemoPackageName");
         if (StaticSettings.IsStarterKit)
         {
            if (!string.IsNullOrEmpty(packageName) && 
                 File.Exists(AppDomain.CurrentDomain.BaseDirectory + Path.Combine(Settings.TempFolderPath, packageName)))
            {
               string res = JobContext.Confirm(ResourceManager.Localize("INSTALL_CALENDAR_DEMO_ITEMS"));

               if (res == "yes")
               {

                  Installer installer = new Installer();
                  installer.InstallPackage(AppDomain.CurrentDomain.BaseDirectory +
                                           Path.Combine(Settings.TempFolderPath, packageName));

                  DemoItems.CreateEvents();
               }
            }
            else
            {
               DemoItems.CreateDefaultCalendarsList();
            }
         }
         else
         {
            if (!string.IsNullOrEmpty(packageName))
            {
               File.Delete(Path.Combine(ApplicationContext.PackagePath, packageName));
            }
            DemoItems.CreateDefaultCalendarsList();
         }
      }
   }
}