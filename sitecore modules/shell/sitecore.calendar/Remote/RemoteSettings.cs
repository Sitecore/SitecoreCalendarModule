using System.Collections.Specialized;
using System.Configuration;

using Sitecore.Configuration;
using Sitecore.Modules.EventCalendar.Network;

namespace Sitecore.Modules.EventCalendar.Remote
{
   public class RemoteSettings
   {
      #region Constants

      public static readonly string RemoteServiceConnectionNameKey = "remoteCalendarService";
      public static readonly string HostKey = "host";
      public static readonly string PasswordKey = "password";
      public static readonly string TimeOutKey = "timeout";
      public static readonly string UserKey = "user";

      #endregion

      #region Fields

      private static Credentials credentials;
      private static bool isServiceInit;
      private static CalendarService service;

      #endregion

      #region Properties

      public static string ServiceConnectionName
      {
         get
         {
            return Settings.GetSetting(RemoteServiceConnectionNameKey);
         }
      }

      private static NameValueCollection ServiceParameters
      {
         get
         {
            string name = ServiceConnectionName;
            if (!string.IsNullOrEmpty(ServiceConnectionName) &&
                ConfigurationManager.ConnectionStrings[name] != null)
            {
               string connection = ConfigurationManager.ConnectionStrings[name].ConnectionString;
               return StringUtil.GetNameValues(connection, '=', ';');
            }

            return null;
         }
      }

      public static CalendarService Service
      {
         get
         {
            if (service == null && !isServiceInit)
            {
               NameValueCollection parameters = ServiceParameters;
               if (parameters != null && parameters.Count > 0)
               {
                  service = new CalendarService { Url = parameters[HostKey], Timeout = int.Parse(parameters[TimeOutKey]) };
               }

               isServiceInit = true;
            }

            return service;
         }
      }

      public static Credentials ServiceCredentials
      {
         get
         {
            if (credentials == null)
            {
               if (service != null)
               {
                  NameValueCollection parameters = ServiceParameters;
                  credentials = new Credentials
                  {
                     UserName = parameters[UserKey],
                     Password = parameters[PasswordKey]
                  };
               }
            }
            return credentials;
         }
      }

      #endregion
   }
}