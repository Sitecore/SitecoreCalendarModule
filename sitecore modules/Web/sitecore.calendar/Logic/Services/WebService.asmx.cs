using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Exceptions;
using Sitecore.Modules.EventCalendar.Logic.Utils;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.UI;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Services
{
   [WebService(Namespace = "http://www.sitecore.net/modules/EventCalendar")]
   [ScriptService]
   public class WebService : System.Web.Services.WebService
   {
      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public void MoveEvent(string eventID, string newDate, string startTime, string endTime)
      {
         RemoteActionWrapper.MoveEvent(eventID, newDate);
         if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
         {
            RemoteActionWrapper.UpdateTime(eventID, startTime, endTime);
         }
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public void UpdateTime(string eventID, string startTime, string endTime, bool updateSeries)
      {
         RemoteActionWrapper.UpdateTime(eventID, startTime, endTime, updateSeries);
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public void CreateEvent(Event ev, Schedule schedule, Options options)
      {
         RemoteActionWrapper.CreateEvent(ev, schedule, options);
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public void DeleteEvent(string eventID, bool deleteSeries)
      {
         RemoteActionWrapper.DeleteEvent(eventID, deleteSeries);
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public void SaveEventInfo(Event evnt, Schedule schedule, bool updateSeries, Options options)
      {
         RemoteActionWrapper.UpdateEvent(evnt, schedule, options, updateSeries);
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public Event GetEventInfo(string eventID)
      {
         return new Event(ID.Parse(eventID));
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public Schedule GetScheduleInfo(string scheduleID)
      {
         if (string.IsNullOrEmpty(scheduleID))
         {
            return null;
         }

         return new Schedule(ID.Parse(scheduleID));
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public ArrayList GetCalendarsList(bool canWrite, Options options)
      {
         var list = new ArrayList();
         Item settingsItem = StaticSettings.EventSourceDatabase.GetItem(options.ControlSettingsPath);
         Item[] eventList = Utilities.GetEventListForView(settingsItem, options.SiteSettingsPath);
         var EventListMgr = new EventListManager(eventList, options.SiteSettingsPath);

         foreach (var cal in EventListMgr.ActiveEventLists)
         {
            if (cal.Value.Selected && (!canWrite || cal.Value.CanWrite))
            {
               list.Add(cal.Value.Title);
               list.Add(cal.Value.ID.ToString());
            }
         }

         return list;
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public EventList GetCalendar(string eventID, Options options)
      {
         EventList calendar;

         Item item = StaticSettings.EventSourceDatabase.GetItem(ID.Parse(eventID));
         if (item == null)
         {
            return null;
         }

         if (item.TemplateID != StaticSettings.EventTemplate.ID)
         {
            throw new EventCalendarException(
               String.Format(String.Format(ResourceManager.Localize("UNSUPPORT_TEMPLATE"), item.Name,
                                           item.TemplateName, StaticSettings.EventTemplate.Name)));
         }

         Item settingsItem = StaticSettings.EventSourceDatabase.GetItem(options.ControlSettingsPath);
         Item[] eventList = Utilities.GetEventListForView(settingsItem, options.SiteSettingsPath);
         var EventListMgr = new EventListManager(eventList, options.SiteSettingsPath);

         calendar = EventListMgr.GetCalendar(item);

         return calendar;
      }

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public bool IsRecurrent(string eventID)
      {
         if (string.IsNullOrEmpty(eventID))
         {
            return false;
         }

         Event ev = GetEventInfo(eventID);

         if (ev == null)
         {
            return false;
         }

         return ev.IsRecurrentEvent;
      }

      #region View mode

      [WebMethod(EnableSession = true)]
      [ScriptMethod]
      public string GetHTMLDayInMonthView(string date, Options options, int startHour, int hour)
      {
				if ((HttpContext.Current.Request.UserLanguages != null) &&
					 (!String.IsNullOrEmpty(HttpContext.Current.Request.UserLanguages[0])))
				{
					Utilities.SetCurrentCulture(HttpContext.Current.Request.UserLanguages[0]);
				}

         var moduleSettings = new ModuleSettings(options.SiteSettingsPath);
         Item site_settings = moduleSettings.GetSettingsItem();

         Item settingsItem = moduleSettings.GetActiveDatabase().GetItem(options.ControlSettingsPath);

         var stringWriter = new StringWriter();
         var writer = new HtmlTextWriter(stringWriter);

         MonthView view;
         if (Utilities.IsTemplateSettingsEqual(settingsItem, site_settings,
                                               ModuleSettings.MonthViewField))
         {
            view = new MonthView(options.CalendarID, settingsItem, options.SiteSettingsPath);
         }
         else
         {
            view = new MonthView(options.CalendarID, moduleSettings.MonthViewSettings,
                                 options.SiteSettingsPath);
            view.SetSettings(settingsItem, options.SiteSettingsPath);
         }

         view.IsReinitNeed = true;
         view.Attributes["startHour"] = startHour.ToString();
         view.Attributes["numHours"] = hour.ToString();
         view.CurrentDate = Utilities.StringToDate(options.Date);

         view.RenderDayCell(writer, Utilities.StringToDate(date));
         return stringWriter.ToString();
      }

      [WebMethod(EnableSession = true)]
      public string GetViewHTML(int num, int startHour, string viewName, string currentView,
                                int hour, Options options)
      {
				if ((HttpContext.Current.Request.UserLanguages != null) &&
					 (!String.IsNullOrEmpty(HttpContext.Current.Request.UserLanguages[0])))
				{
					Utilities.SetCurrentCulture(HttpContext.Current.Request.UserLanguages[0]);
				}

         var stringWriter = new StringWriter();
         var writer = new HtmlTextWriter(stringWriter);
         if (num > -1)
         {
            writer.Write(string.Format("<calendarNum>{0}</calendarNum>", num));
         }

         var moduleSettings = new ModuleSettings(options.SiteSettingsPath);
         Item site_settings = moduleSettings.GetSettingsItem();
         Item settingsItem = moduleSettings.GetActiveDatabase().GetItem(options.ControlSettingsPath);

         if (viewName == "day")
         {
            if (HttpContext.Current.Session[options.CalendarID] == null)
            {
               HttpContext.Current.Session.Add(options.CalendarID, currentView);
            }

            DayView view;
            if (Utilities.IsTemplateSettingsEqual(settingsItem, site_settings,
                                                  ModuleSettings.DayViewField))
            {
               view = new DayView(options.CalendarID, settingsItem, options.SiteSettingsPath);
            }
            else
            {
               view = new DayView(options.CalendarID, moduleSettings.DayViewSettings,
                                  options.SiteSettingsPath);
               view.SetSettings(settingsItem, options.SiteSettingsPath);
            }
            view.IsReinitNeed = true;

            view.CurrentDate = Utilities.StringToDate(options.Date);
            if (startHour > -1)
            {
               view.NumHoursDisplayed = hour;
               view.StartHour = startHour;
            }
            view.RenderControl(writer);
         }
         else
         {
            if (HttpContext.Current.Session[options.CalendarID] != null)
            {
               HttpContext.Current.Session.Remove(options.CalendarID);
            }

            if (viewName == "week")
            {
               WeekView view;
               if (Utilities.IsTemplateSettingsEqual(settingsItem, site_settings,
                                                     ModuleSettings.WeekViewField))
               {
                  view = new WeekView(options.CalendarID, settingsItem, options.SiteSettingsPath);
               }
               else
               {
                  view = new WeekView(options.CalendarID, moduleSettings.WeekViewSettings,
                                      options.SiteSettingsPath);
                  view.SetSettings(settingsItem, options.SiteSettingsPath);
               }
               view.IsReinitNeed = true;
               view.CurrentDate = Utilities.StringToDate(options.Date);
               if (startHour > -1)
               {
                  view.NumHoursDisplayed = hour;
                  view.StartHour = startHour;
               }
               view.RenderControl(writer);
            }
            else if (viewName == "month")
            {
               MonthView view;
               if (Utilities.IsTemplateSettingsEqual(settingsItem, site_settings,
                                                     ModuleSettings.MonthViewField))
               {
                  view = new MonthView(options.CalendarID, settingsItem, options.SiteSettingsPath);
               }
               else
               {
                  view = new MonthView(options.CalendarID, moduleSettings.MonthViewSettings,
                                       options.SiteSettingsPath);
                  view.SetSettings(settingsItem, options.SiteSettingsPath);
               }
               view.IsReinitNeed = true;
               view.CurrentDate = Utilities.StringToDate(options.Date);
               view.Attributes["startHour"] = startHour.ToString();
               view.Attributes["numHours"] = hour.ToString();
               view.RenderControl(writer);
            }
            else if (viewName == "agenda")
            {
               settingsItem = (viewName == currentView)
                                 ? settingsItem : moduleSettings.AgendaViewSettings;
               var view = new AgendaView(options.CalendarID, settingsItem, options.SiteSettingsPath);
               view.CurrentDate = Utilities.StringToDate(options.Date);
               view.RenderControl(writer);
            }
         }

         return stringWriter.ToString();
      }

      #endregion
   }
}