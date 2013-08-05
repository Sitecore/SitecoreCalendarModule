using System;
using System.Text;
using System.Threading;
using System.Web.UI;

using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
		using Globalization;
		using sitecore_modules.shell.sitecore.calendar.Logic.UI;

	public delegate void EventsRenderer(HtmlTextWriter writer, DateTime date);

   public abstract class BaseView : CalendarWebControl, ICalendarView
   {
      #region Attributes

      private DateTime _currentDate = DateTime.Now;
      private EventListManager _eventListMgr = null;
      private string _idCalendar = null;

      private LinkBehaviour _linkBehaviour = LinkBehaviour.ShowWizard;
      private bool _readOnly = false;
      private string _recurrentEventPrefix = null;
      private string _recurrentEventSuffix = null;
      private bool _reinit = false;
      private bool _showHeader = false;

      private bool _showOtherMonth = true;
      private StandardViewSettings _viewSetting = null;

			

      #endregion

      #region Accessor Methods

      protected string IdCalendar
      {
         get
         {
            return _idCalendar;
         }
         set
         {
            _idCalendar = value;
         }
      }

      public EventListManager EventListMgr
      {
         get
         {
            return _eventListMgr;
         }
         set
         {
            _eventListMgr = value;
            _readOnly = IsReadOnly();
         }
      }

      public LinkBehaviour LinkBehaviour
      {
         get
         {
            return _linkBehaviour;
         }
         set
         {
            _linkBehaviour = value;
         }
      }

      public bool ReadOnly
      {
         get
         {
            return _readOnly;
         }
      }

      public string RecurrentEventPrefix
      {
         get
         {
            return _recurrentEventPrefix;
         }
         set
         {
            _recurrentEventPrefix = value;
         }
      }

      public string RecurrentEventSuffix
      {
         get
         {
            return _recurrentEventSuffix;
         }
         set
         {
            _recurrentEventSuffix = value;
         }
      }

      public bool ShowHeader
      {
         get
         {
            return _showHeader;
         }
         set
         {
            _showHeader = value;
         }
      }

      public bool ShowOtherMonth
      {
         get
         {
            return _showOtherMonth;
         }
         set
         {
            _showOtherMonth = value;
         }
      }

      public DateTime CurrentDate
      {
         get
         {
            return _currentDate;
         }
         set
         {
            _currentDate = value;
         }
      }

      private bool IsReadOnly()
      {
         return (!_eventListMgr.CanWrite || _eventListMgr.ActiveEventLists.Count == 0 || StaticSettings.MasterDatabase == null);
      }

      #endregion

      #region Initialization Methods

      protected void init(StandardViewSettings view_settings, string site_settings_path)
      {
         _viewSetting = view_settings;
         RecurrentEventPrefix = view_settings.RecurrentEventPrefix;
         RecurrentEventSuffix = view_settings.RecurrentEventSuffix;
         ShowHeader = view_settings.ShowHeader;
         EventListMgr = new EventListManager(view_settings.EventLists, site_settings_path);
      }

      protected override void OnInit(EventArgs e)
      {
				base.OnInit(e);

				if (this.Page != null)
				{
					if (!this.Page.IsPostBack)
					{
						IsReinitNeed = true;
					}

					Page.RegisterRequiresControlState(this);
				}
      }

      #endregion

      #region Control State Methods

      protected override object SaveControlState()
      {
         return _currentDate;
      }

      protected override void LoadControlState(object state)
      {
         if (state != null)
         {
            _currentDate = (DateTime)state;
         }
      }

      #endregion

      #region Render Methods

      protected virtual string AdditionalAttributes
      {
         get
         {
            StringBuilder attributes = new StringBuilder();

            foreach (string key in Attributes.Keys)
            {
               attributes.AppendFormat("{0}='{1}' ", key, Attributes[key]);
            }
            return attributes.ToString();
         }
      }

      public bool IsReinitNeed
      {
         get
         {
            return _reinit;
         }
         set
         {
            _reinit = value;
         }
      }

      public virtual Control RenderHeader()
      {
         return null;
      }

      protected abstract override void Render(HtmlTextWriter writer);

      public virtual void RenderSwitcher(HtmlTextWriter writer)
      {
         if (_viewSetting.SwitchViewMode)
         {
            StringBuilder html = new StringBuilder();

            html.AppendFormat(
               "<div class='typeViewSelector'><a class='viewModeLink' link='' onclick='ViewMode.getInstance().SwitchToDayView(\"{0}\");'>{1}</a>",
               _idCalendar, Translate.Text("Day"));
            html.AppendFormat(
               "<a class='viewModeLink' link='' onclick='ViewMode.getInstance().SwitchToWeekView(\"{0}\");'>{1}</a>",
               _idCalendar, Translate.Text("Week"));
            html.AppendFormat(
               "<a class='viewModeLink' link='' onclick='ViewMode.getInstance().SwitchToMonthView(\"{0}\");'>{1}</a></div>",
               _idCalendar, Translate.Text("Month"));

            writer.Write(html);
         }
      }

      protected virtual void RenderTimingColumn(HtmlTextWriter writer)
      {
         StringBuilder html = new StringBuilder();
         html.AppendFormat(
            "<table id='timeRullerTbl{0}' class='scCTimRuler' border='0' cellpadding='0' cellspacing='0' onselect='return false;'>",
            IdCalendar);

         DateTime date = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
         for (int i = 0; i < 24; i++)
         {
            html.AppendFormat("<tr><td class='firstHalf' time='{0}'>{1}",
                     date.ToString("HH:mm"),
                     date.ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern));

            html.Append("</td></tr><tr><td class='secHalf'>&nbsp;</td></tr>");
            date = date.AddHours(1);
         }

         html.Append("</table>");
         writer.Write(html);
      }

      protected virtual void RenderTimingGrid(HtmlTextWriter writer, DateTime date, EventsRenderer renderer)
      {
         writer.Write("<div style='position:relative; width:100%; height:100%;' >");

         string id = string.Format("dayTbl_{0}_{1}_{2}", date.Day, date.Month, date.Year);
         writer.Write(
            string.Format(
               "<table id='{0}' date='{1}' DayTable width='100%' height='100%' dayCellTable='true'  border='0' cellpadding='0' cellspacing='0' onselect='return false;'>",
               id, Utilities.NormalizeDate(date)));

         for (int i = 0; i < 24; i++)
         {
            writer.Write("<tr>");
            writer.Write(string.Format("<td class='firstHalfDay' time='{0}:00'>", i));

            if (i == 0 & renderer != null)
            {
               renderer(writer, date);
            }

            writer.Write("&nbsp;</td></tr><tr>");
            writer.Write(string.Format("<td class='secHalfDay' time='{0}:30'>", i));
            writer.Write("&nbsp;</td></tr>");
         }

         writer.Write("</table>");

         writer.Write("</div>");
      }

      protected virtual void RenderEventToolbar(HtmlTextWriter writer)
      {
         var sb = new StringBuilder();
         sb.AppendFormat("<div class='eventToolbarFrame' id='idEventToolbar{0}' contentEditable='false'>", IdCalendar);
         sb.Append("<table border=0 cellpadding='0' cellspacing='0' class='eventToolbarTable'>");
         sb.Append("<tr><td>");
         sb.AppendFormat("<input class='eventToolbarInput' type=text id='idEventTitle{0}' /></td></tr>", IdCalendar);

         sb.AppendFormat("<tr><td><select id='idCalendarsList{0}' class='eventToolbarCombo'>", IdCalendar);

         sb.Append("</select></td></tr>");
         sb.Append("<tr><td style='border-top:solid white 1px;'>");

				 sb.AppendFormat(
						"<a href=# class='eventToolbarButton' onclick='EventToolbar.getInstance(\"{0}\").dispatchMessage(\"{0}\", \"event:create\"); return false;'>Create</a>",
						IdCalendar);
				 sb.AppendFormat(
						"<a href=# class='eventToolbarButton' id='idExpandButton{0}' onclick='EventToolbar.getInstance().dispatchMessage(\"{0}\", \"event:expand\");return false;'>Expand</a>",
						IdCalendar);
				 sb.AppendFormat(
						"<a href=# class='eventToolbarCancel' onclick='EventToolbar.getInstance(\"{0}\").dispatchMessage(\"{0}\", \"event:cancel\"); return false;'>Cancel</a>",
						IdCalendar);

         sb.Append("</td></tr></table></div>");

         writer.Write(sb.ToString());
      }

      protected virtual void RenderEvent(HtmlTextWriter writer, string onclientclick, Event ev, string title,
                                         string dotImage)
      {
         if (ev.ReadOnly || ReadOnly)
         {
            RenderRecurrentEventPrefix(writer, ev);
            writer.Write(
               string.Format("<a class='textEvent' eventID='{0}' link='' {1}>{2}</a>", ev.ID,
                             ev.Path != string.Empty ? string.Format("href='{0}'", ev.Path) : onclientclick, title));
         }
         else
         {
            writer.Write(
               string.Format(
                  "<img class='eventImage' src='{0}' style='background-color:inherit' readonly='true' eventID='{1}' {2} align='absmiddle' title='Click to edit' border='0' alt='' width='16px' height='16px' />",
                  dotImage, ev.ID, onclientclick));
            RenderRecurrentEventPrefix(writer, ev);
            writer.Write(title);
         }
         RenderRecurrentEventSuffix(writer, ev);
      }

      protected abstract void RenderDataBeforeEvent(HtmlTextWriter writer, Event ev);

      protected abstract void RenderDataAfterEvent(HtmlTextWriter writer, Event ev);

      protected virtual void RenderRecurrentEventPrefix(HtmlTextWriter writer, Event ev)
      {
         if (ev.IsRecurrentEvent)
         {
            writer.Write(string.Format("<span class='recurrent'>{0}</span>", RecurrentEventPrefix));
         }
      }

      protected virtual void RenderRecurrentEventSuffix(HtmlTextWriter writer, Event ev)
      {
         if (ev.IsRecurrentEvent)
         {
            writer.Write(string.Format("<span class='recurrent'>{0}</span>", RecurrentEventSuffix));
         }
      }

      protected virtual void RenderEvents(HtmlTextWriter writer, DateTime date, string eventAttributes,
                                          string dotAttributes, string cssClassReadOnly, string cssClassCanWrite,
                                          bool isUseBackgroundColor)
      {
         foreach (EventList evList in EventListMgr.ActiveEventLists.Values)
         {
            foreach (Event ev in evList.GetEventsByDate(date))
            {
               if (ev.IsVisible)
               {
                  RenderEventSection(writer, ev, evList, date, eventAttributes, dotAttributes, cssClassReadOnly,
                                     cssClassCanWrite, isUseBackgroundColor);
               }
            }
         }
      }

      protected virtual void RenderEventSection(HtmlTextWriter writer, Event ev, EventList evList, DateTime date,
                                                string eventAttributes, string dotAttributes, string cssClassReadOnly,
                                                string cssClassCanWrite, bool isUseBackgroundColor)
      {
         string title = (ev.Title == string.Empty) ? ev.Name : ev.Title;
         string cssStyle = ((!ReadOnly && !ev.ReadOnly) || isUseBackgroundColor)
                              ? buildRuntimeStyle(evList.TextColor, evList.BackgroundColor) : "";

         writer.Write(
            string.Format(
               "<div id='{0}' {1} eventID='{0}' align='left' startTime='{2}' endTime='{3}' readonly='{4}' link='{5}' {6} class='{7}' startHour='{8}' isrecur='{9}'>",
               ev.ID, cssStyle, ev.StartTime, ev.EndTime, ReadOnly ? true : ev.ReadOnly,
               (LinkBehaviour == LinkBehaviour.Redirect) ? ev.Path : String.Empty, eventAttributes,
               (ReadOnly || ev.ReadOnly) ? cssClassReadOnly : cssClassCanWrite, date.Hour, ev.IsRecurrentEvent ? "1" : "0"));

         RenderDataBeforeEvent(writer, ev);
         RenderEvent(writer, dotAttributes, ev, title, evList.EventEditIcon);
         RenderDataAfterEvent(writer, ev);

         writer.Write("</div>");
      }

      protected virtual string buildRuntimeStyle(string textColor, string backgroundColor)
      {
         string runtimeStyle = string.Empty;
         if (textColor != string.Empty)
         {
            runtimeStyle += "color:" + textColor + ";";
         }
         if (backgroundColor != string.Empty)
         {
            runtimeStyle += "background-color:" + backgroundColor + ";";
         }

         if (runtimeStyle != string.Empty)
         {
            runtimeStyle = "style=" + runtimeStyle;
         }

         return runtimeStyle;
      }

      #endregion
   }
}