using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Remote;

namespace Sitecore.Modules.EventCalendar
{
   using Caching;

   public class RemoteActionWrapper
   {
      #region Methods

      public static void MoveEvent(string eventID, string newDate)
      {
         if (RemoteSettings.Service != null)
         {
            RemoteSettings.Service.MoveEvent(eventID, newDate, RemoteSettings.ServiceCredentials);
            CacheManager.ClearAllCaches();
         }
         else
         {
            CalendarActions.MoveEvent(eventID, newDate);
         }
      }

      public static void UpdateTime(string eventID, string startTime, string endTime)
      {
         UpdateTime(eventID, startTime, endTime, false);
      }

      public static void UpdateTime(string eventID, string startTime, string endTime,
                                    bool updateSeries)
      {
         if (RemoteSettings.Service != null)
         {
            RemoteSettings.Service.UpdateTime(eventID, startTime, endTime, updateSeries,
                                              RemoteSettings.ServiceCredentials);
            CacheManager.ClearAllCaches();
         }
         else
         {
            CalendarActions.UpdateTime(eventID, startTime, endTime, updateSeries);
         }
      }

      public static void CreateEvent(Event ev, Schedule schedule, Options options)
      {
         if (RemoteSettings.Service != null)
         {
            RemoteSettings.Service.CreateEvent(ev.ToXml(),
                                               schedule == null ? string.Empty : schedule.ToXml(),
                                               options.ToXml(), RemoteSettings.ServiceCredentials);

            CacheManager.ClearAllCaches();
         }
         else
         {
            CalendarActions.CreateEvent(ev, schedule, options);
         }
      }

      public static void DeleteEvent(string eventID, bool deleteSeries)
      {
         if (RemoteSettings.Service != null)
         {
            RemoteSettings.Service.DeleteEvent(eventID, deleteSeries,
                                               RemoteSettings.ServiceCredentials);
            CacheManager.ClearAllCaches();
         }
         else
         {
            CalendarActions.DeleteEvent(eventID, deleteSeries);
         }
      }

      public static void UpdateEvent(Event evnt, Schedule schedule, Options options)
      {
         UpdateEvent(evnt, schedule, options, false);
      }

      public static void UpdateEvent(Event evnt, Schedule schedule, Options options,
                                     bool updateSeries)
      {
         if (RemoteSettings.Service != null)
         {
            RemoteSettings.Service.SaveEventInfo(evnt.ToXml(), schedule == null ? string.Empty : schedule.ToXml(), options.ToXml(),
                                                 updateSeries, RemoteSettings.ServiceCredentials);
            CacheManager.ClearAllCaches();
         }
         else
         {
            CalendarActions.UpdateEvent(evnt, schedule, options, updateSeries);
         }
      }

      #endregion
   }
}