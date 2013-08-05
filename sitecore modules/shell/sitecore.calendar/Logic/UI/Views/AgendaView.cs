using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.UI;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
	using sitecore_modules.shell.sitecore.calendar.Logic.UI;

	public class AgendaView : BaseView
   {
      #region Private Variables

      private DateTime _agendaStartDate;
      private string _agendaTitle;
      private int _eventLimit;
      private bool _showEventDate;
      private bool _showEventTime;
      private AgendaViewSettings settings;
      private string siteSettings = null;

      #endregion

      #region Field Access Methods

      public string AgendaTitle
      {
         get
         {
            return _agendaTitle;
         }
         set
         {
            _agendaTitle = value;
         }
      }

      public DateTime AgendaStartDate
      {
         get
         {
            return _agendaStartDate;
         }
         set
         {
            _agendaStartDate = value;
         }
      }

      public int EventLimit
      {
         get
         {
            return _eventLimit;
         }
         set
         {
            _eventLimit = value;
         }
      }

      public bool ShowEventDate
      {
         get
         {
            return _showEventDate;
         }
         set
         {
            _showEventDate = value;
         }
      }

      public bool ShowEventTime
      {
         get
         {
            return _showEventTime;
         }
         set
         {
            _showEventTime = value;
         }
      }

      #endregion

      #region Initialization Methods

      public AgendaView(string idCalendar, Item agenda_settings, string site_settings_path)
      {
         init(idCalendar, agenda_settings, site_settings_path);
      }

      private void init(string idCalendar, Item agenda_settings, string site_settings_path)
      {
         settings = new AgendaViewSettings(agenda_settings, site_settings_path);
         siteSettings = site_settings_path;
         IdCalendar = idCalendar;

         Attributes["startHour"] = "-1";
         Attributes["numHours"] = "24";

         AgendaTitle = settings.Title;
         switch (settings.StartDate)
         {
            case StartDateOption.Today:
               AgendaStartDate = DateTime.Now;
               break;

            case StartDateOption.SelectedDate:
               if (HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate] == null)
               {
                  AgendaStartDate = DateTime.Now;
               }
               else
               {
                  AgendaStartDate = (DateTime)HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate];
               }
               break;
         }
         EventLimit = settings.EventLimit;
         ShowEventDate = settings.ShowEventDate;
         ShowEventTime = settings.ShowEventTime;
         Margin = settings.Margin;
         EventListMgr = new EventListManager(settings.EventLists, site_settings_path);
      }

      #endregion

      #region Render Methods

      public override Control RenderHeader()
      {
         return null;
      }

      protected override void Render(HtmlTextWriter writer)
      {
         SortedList<DateTime, SortedList<string, List<Event>>> eventsList =
            new SortedList<DateTime, SortedList<String, List<Event>>>();

         foreach (KeyValuePair<String, EventList> event_list in EventListMgr.ActiveEventLists)
         {
            if (event_list.Value.Selected)
            {
               event_list.Value.GetEvents(AgendaStartDate, EventsOrder.Ascending, EventLimit, ref eventsList);
            }
         }

         int vector = 1;
         int direction = 0;

         writer.Write(string.Format("<div style='margin:{0}; width:auto'>", Margin));
         writer.Write(
            string.Format("<table id='idGrid' style='margin:0px;' mode='agenda' date='{0}' {1}><tr><td>",
                          Utilities.NormalizeDate(CurrentDate), AdditionalAttributes));

         if (!String.IsNullOrEmpty(AgendaTitle))
         {
            writer.Write(String.Format("<h2>{0}</h2>", AgendaTitle));
         }

         writer.Write("<ul class='agenda-links'>");

         int items_displayed = 0;
         for (int i = 0; i < eventsList.Count; i++)
         {
            SortedList<String, List<Event>> dateList = eventsList.Values[direction + i*vector];

            foreach (KeyValuePair<string, List<Event>> timeList in dateList)
            {
               foreach (Event evnt in timeList.Value)
               {
                  writer.Write("<li>");

                  RenderEvent(evnt.ID, Utilities.StringToDate(evnt.StartDate).ToString(settings.DateFormat),
                                    string.Format("{0}-{1}", 
                                    Utilities.GetTime(evnt.StartTime).ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern),  
                                    Utilities.GetTime(evnt.EndTime).ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern)),
                              evnt.Path == string.Empty
                                 ? string.Format(
                                      "onclick='EventWizard.getInstance(\"{0}\").ExpandEvent(this, \"{1}\", \"{2}\");'",
                                      IdCalendar, siteSettings, settings.ID) : string.Format("href='{0}'", evnt.Path),
                              String.IsNullOrEmpty(evnt.Title) ? evnt.Name : evnt.Title, writer);

                  writer.Write("</li>");
                  items_displayed++;
                  if (items_displayed == EventLimit)
                  {
                     break;
                  }
               }
               if (items_displayed == EventLimit)
               {
                  break;
               }
            }
            if (items_displayed == EventLimit)
            {
               break;
            }
         }

         writer.Write("</ul>");
         writer.Write("</td></tr></table><div></div>");
         writer.Write("</div>");
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

      protected virtual void RenderEvent(string id, string date, string timeSpan, string path, string title,
                                         HtmlTextWriter writer)
      {
         writer.Write(string.Format("<div readonly='{0}'>", ReadOnly));
         if (ShowEventDate && ShowEventTime)
         {
            writer.Write(
               string.Format(
                  "<span class='agendaDate'>{1}</span><br /><a class='textEvent' eventID='{0}' link='' {2}>{3}</a>", id,
                  (date + "<br/>" + timeSpan), path, title));
         }
         else if (ShowEventDate)
         {
            writer.Write(
               string.Format(
                  "<span class='agendaDate'>{1}</span><br /><a class='textEvent' eventID='{0}' link='' {2}>{3}</a>", id,
                  date, path, title));
         }
         else if (ShowEventTime)
         {
            writer.Write(
               string.Format(
                  "<span class='agendaDate'>{1}</span><br /><a class='textEvent' eventID='{0}' link='' {2}>{3}</a>", id,
                  timeSpan, path, title));
         }
         else
         {
            writer.Write(string.Format("<a class='textEvent' eventID='{0}' link='' {1}>{2}</a>", id, path, title));
         }
         writer.Write("</div>");
      }

      protected override void RenderDataBeforeEvent(HtmlTextWriter writer, Event ev)
      {
      }

      protected override void RenderDataAfterEvent(HtmlTextWriter writer, Event ev)
      {
      }

			protected override CalendarWebControl CreateControlInstance(Page page)
			{
				return null;
			}

      #endregion
   }
}