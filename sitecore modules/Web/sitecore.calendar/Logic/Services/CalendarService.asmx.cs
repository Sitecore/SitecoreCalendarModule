using System;
using System.ComponentModel;
using System.Web.Security;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;

using Sitecore.Common;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.EventCalendar.Services
{
   /// <summary>
   /// Summary description for CalendarService
   /// </summary>
   [WebService(Namespace = "http://tempuri.org/")]
   [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
   [ToolboxItem(false)]
   // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
      // [System.Web.Script.Services.ScriptService]
   public class CalendarService : System.Web.Services.WebService
   {
      [WebMethod]
      public void MoveEvent(string eventID, string newDate, Credentials credentials)
      {
         Error.AssertObject(credentials, "credentials");
         Assert.ArgumentNotNullOrEmpty(eventID, "eventID");
         Assert.ArgumentNotNullOrEmpty(newDate, "newDate");

         Login(credentials);

         CalendarActions.MoveEvent(eventID, newDate);
      }

      [WebMethod]
      public void UpdateTime(string eventID, string startTime, string endTime, bool updateSeries,
                             Credentials credentials)
      {
         Error.AssertObject(credentials, "credentials");
         Assert.ArgumentNotNullOrEmpty(eventID, "eventID");
         Assert.ArgumentNotNullOrEmpty(startTime, "startTime");
         Assert.ArgumentNotNullOrEmpty(endTime, "endTime");

         Login(credentials);
         CalendarActions.UpdateTime(eventID, startTime, endTime, updateSeries);
      }

      [WebMethod]
      public void CreateEvent(string evtXml, string scheduleXml, string optionsXml, Credentials credentials)
      {
         Error.AssertObject(credentials, "credentials");
         Assert.ArgumentNotNullOrEmpty(evtXml, "evtXml");
         Assert.ArgumentNotNull(scheduleXml, "scheduleXml");
         Assert.ArgumentNotNullOrEmpty(optionsXml, "optionsXml");

         Login(credentials);
         CalendarActions.CreateEvent(Event.Parse(evtXml), Schedule.Parse(scheduleXml), Options.Parse(optionsXml));
      }

      [WebMethod]
      public void DeleteEvent(string eventID, bool deleteSeries, Credentials credentials)
      {
         Error.AssertObject(credentials, "credentials");
         Assert.ArgumentNotNullOrEmpty(eventID, "eventID");

         Login(credentials);
         CalendarActions.DeleteEvent(eventID, deleteSeries);
      }

      [WebMethod]
      [XmlInclude(typeof(Event))]
      [XmlInclude(typeof(Schedule))]
      [XmlInclude(typeof(Options))]
      public void SaveEventInfo(string evtXml, string scheduleXml, string optionsXml, bool updateSeries, Credentials credentials)
      {
         Error.AssertObject(credentials, "credentials");
         Assert.ArgumentNotNullOrEmpty(evtXml, "evtXml");
         Assert.ArgumentNotNull(scheduleXml, "scheduleXml");
         Assert.ArgumentNotNullOrEmpty(optionsXml, "optionsXml");

         Login(credentials);

         CalendarActions.UpdateEvent(Event.Parse(evtXml), Schedule.Parse(scheduleXml), Options.Parse(optionsXml),
                                   updateSeries);
      }

      #region Private Methods

      private void Login(Credentials credentials)
      {
         Error.AssertObject(credentials, "credentials");
         if (Sitecore.Context.IsLoggedIn)
         {
            if (Sitecore.Context.User.Name.Equals(credentials.UserName,
                                                  StringComparison.OrdinalIgnoreCase))
            {
               return;
            }
            Sitecore.Context.Logout();
         }
         Assert.IsTrue(Membership.ValidateUser(credentials.UserName, credentials.Password),
               "Unknown username or password.");


         AuthenticationManager.Login(credentials.UserName, false);

      }

      #endregion
   }
}