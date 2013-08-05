using System;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
	using sitecore_modules.shell.sitecore.calendar.Logic.UI;

	public class CustomLinkButton : LinkButton
   {
      protected override void Render(HtmlTextWriter writer)
      {
         writer.Write(string.Format("<a link=' ' onclick='{0}' class='customLink'>{1}</a>", OnClientClick, Text));
      }
   }

   public class MonthView : ExtendBaseView
   {
      #region Private Variables

      private readonly EventsListOverflow _eventsOverflow = EventsListOverflow.ShowEllipsis;
      private readonly CustomLinkButton _overflowLink = new CustomLinkButton();
      private string _altAddEventIcon = null;
      private int _eventLimit = 3;
      private string _eventOverflowLinkText = null;
      private bool _highlightWeekends = false;
      private bool _showEventTime = true;
      private string addEventIcon = null;
      private MonthViewSettings settings = null;
      private string siteSettings = null;

      private bool isFullInit = false;

      #endregion Private Variables

      #region Field Access Methods

      public string AddEventIcon
      {
         get
         {
            return addEventIcon;
         }
         set
         {
            addEventIcon = value;
         }
      }

      public string AltAddEventIcon
      {
         get
         {
            return _altAddEventIcon;
         }
         set
         {
            _altAddEventIcon = value;
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

      public EventsListOverflow EventOverflowBehavior
      {
         get
         {
            return _eventsOverflow;
         }
      }

      public string EventOverflowLinkText
      {
         get
         {
            return _eventOverflowLinkText;
         }
         set
         {
            _eventOverflowLinkText = value;
            _overflowLink.Text = _eventOverflowLinkText;
         }
      }

      public bool HighlightWeekends
      {
         get
         {
            return _highlightWeekends;
         }
         set
         {
            _highlightWeekends = value;
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

      public MonthView(string idCalendar, Item month_settings, string site_settings_path)
      {
         init(idCalendar, month_settings, site_settings_path);
      }

      private void init(string idCalendar, Item month_settings, string site_settings_path)
      {
         settings = new MonthViewSettings(month_settings, site_settings_path);
         siteSettings = site_settings_path;

         Attributes["startHour"] = "-1";
         Attributes["numHours"] = "24";
         IdCalendar = idCalendar;

         AddEventIcon = settings.AddEventIcon;
         AltAddEventIcon = settings.AltAddEventIcon;
         EventLimit = settings.EventLimit;
         EventOverflowLinkText = settings.EventOverflowLinkText;
         HighlightWeekends = settings.HighlightWeekends;
         Margin = settings.Margin;
         ShowEventTime = settings.ShowEventTime;

         init(settings, site_settings_path);
      }

      protected override void OnInit(EventArgs e)
      {
         base.OnInit(e);

         isFullInit = true;
         Controls.Add(_overflowLink);
      }

      #endregion

      #region Rendering Methods

      public override Control RenderHeader()
      {
         HtmlGenericControl header = new HtmlGenericControl();
         if (ShowHeader)
         {
            header.InnerHtml = RenderDaysOfWeekHeader();
         }
         return header;
      }

      public void SetSettings(Item settingsItem, string sitePath)
      {
         IsReinitNeed = true;
         if (!IsDefaultSettingsItem(settingsItem, sitePath))
         {
            if (settings.Item.TemplateID == settingsItem.TemplateID)
            {
               init(IdCalendar, settingsItem, sitePath);
            }
            else
            {
               base.SetStandartSettings(settingsItem, sitePath);
            }
         }
      }

      public virtual void RenderDayCell(HtmlTextWriter writer, DateTime date)
      {
         string headerClass;
         string addEventIcon;

         if (date.Month != CurrentDate.Month)
         {
            headerClass = "dayCellHeaderOM";
            addEventIcon = AltAddEventIcon;
         }
         else
         {
            headerClass = "dayCellHeader";
            addEventIcon = AddEventIcon;
         }

         StringBuilder html = new StringBuilder();

         html.AppendFormat("<div class='{0}'>", headerClass);

         string clickAction = string.Empty;

         if (!ReadOnly)
         {
            clickAction =
               string.Format(
                  "onclick='MonthManager.getInstance(\"{0}\").CreateEvent(event,\"{1}\", \"{2}\", \"{3}\");return false;' title='Click to Create New Event'",
                  IdCalendar, Utilities.NormalizeDate(date), siteSettings, settings.ID);
         }

         html.AppendFormat(
            "<a link='' class='dayNumber' onclick='ViewMode.getInstance().SwitchToDayViewToDay(\"{0}\", \"{1}\")'>",
            IdCalendar, Utilities.NormalizeDate(date));

         if (!ReadOnly && !(CurrentDate.Month != date.Month && ShowOtherMonth == false))
         {
            html.AppendFormat("<img src='{0}' {1} align='absmiddle'  border='0' alt='' width='16px' height='16px' />",
                              addEventIcon, clickAction);
         }

         html.Append(date.Day + "</a>");

         if (CurrentDate.Month != date.Month && ShowOtherMonth == false)
         {
            html.Append("</div>");
            writer.Write(html);
            return;
         }

         html.Append("</div>");

         writer.Write(html);
         int eventsCount = RenderEvents(writer, date);

         if (EventOverflowBehavior == EventsListOverflow.ShowEllipsis && EventListMgr.HasEvents(date) &&
             (EventLimit > 0 && EventLimit < eventsCount))
         {
            _overflowLink.OnClientClick =
               string.Format("ViewMode.getInstance().SwitchToDayViewToDay(\"{0}\", \"{1}\")", IdCalendar,
                             Utilities.NormalizeDate(date));
            _overflowLink.RenderControl(writer);
         }
      }

      protected virtual int RenderEvents(HtmlTextWriter writer, DateTime date, string eventAttributes,
                                         string dotAttributes, int threshold, string cssClassReadOnly,
                                         string cssClassCanWrite, bool isUseBackgroundColor)
      {
         int counter = 0;

         foreach (Event ev in EventListMgr.GetSortedEvents(date, threshold, new ComparerEventsByStartTime()))
         {
            if (ev.IsVisible)
            {
               if (threshold != 0 && counter == threshold)
               {
                  ++counter;
                  break;
               }

               EventList evList = new EventList(Utilities.GetCalendarByEvent(ev.GetItem()));

               RenderEventSection(writer, ev, evList, date, eventAttributes, dotAttributes, cssClassReadOnly,
                                  cssClassCanWrite, isUseBackgroundColor);
               counter++;
            }
         }

         return counter;
      }

      protected virtual int RenderEvents(HtmlTextWriter writer, DateTime date)
      {
         string clickAction;

         if (LinkBehaviour == LinkBehaviour.ShowWizard)
         {
            clickAction =
               string.Format("onclick='EventWizard.getInstance(\"{0}\").ExpandEvent(this, \"{1}\", \"{2}\");'",
                             IdCalendar, siteSettings, settings.ID);
         }
         else
         {
            clickAction = string.Format("onclick='MonthManager.getInstance(\"{0}\").Redirect(this);'", IdCalendar);
         }

         return
            RenderEvents(writer, date, "", clickAction,
                         EventOverflowBehavior != EventsListOverflow.ShowScrollbars ? EventLimit : int.MaxValue, "",
                         "eventInMonth", false);
      }

      protected virtual void RenderDays(HtmlTextWriter writer, DateTime visibleDate)
      {
         visibleDate = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);

         DateTime startDate = Utilities.FirstCalendarDay(visibleDate);

         int numOfWeeksInTheMonth = Utilities.NumberOfVisibleWeeks(visibleDate, startDate);

         int counter = 0;

         for (int weekNumber = 0; weekNumber < numOfWeeksInTheMonth; weekNumber++)
         {
            writer.Write("<tr>");

            for (int dayNumber = 0; dayNumber < 7; dayNumber++)
            {
               string today = "";
               DateTime currentDate = startDate.AddDays(counter);

               if (EventOverflowBehavior == EventsListOverflow.ShowScrollbars)
               {
                  if (DateTime.Today == currentDate)
                  {
                     today = " today='true' ";
                  }
               }

               writer.Write(
                  string.Format("<td style='height: 80px; width:14%' class='{0}' container date='{1}' {2}>",
                                GetCSSClass(currentDate, weekNumber), Utilities.NormalizeDate(currentDate), today));

               RenderDayCell(writer, startDate.AddDays(counter));

               writer.Write("</td>");
               counter++;
            }
         }

         writer.Write("</tr>");
      }

      protected virtual string GetCSSClass(DateTime date, int weekNumber)
      {
         string cssClass = "dayCellMonth";

         for (int dayNumber = 0; dayNumber < 7; dayNumber++)
         {
            if (DateTime.Today == date)
            {
               cssClass = "todayCellMonth";
            }
            else if (CurrentDate.Month != date.Month)
            {
               if (HighlightWeekends || (HighlightWeekends == false && (weekNumber % 2 == 0)))
               {
                  cssClass = "dayCellOtherMonth";
               }
               else
               {
                  cssClass = "dayCellMonth";
               }
            }
            else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
               if (HighlightWeekends)
               {
                  cssClass = "dayCellWeekEnd";
               }
               else
               {
                  if ((HighlightWeekends == false && (weekNumber % 2 == 0)))
                  {
                     cssClass = "dayCellOtherMonth";
                  }
                  else
                  {
                     cssClass = "dayCellMonth";
                  }
               }
            }
            else
            {
               if (HighlightWeekends == false && (weekNumber % 2 == 0))
               {
                  cssClass = "dayCellOtherMonth";
               }
               else
               {
                  cssClass = "dayCellMonth";
               }
            }

            if (date == CurrentDate)
            {
               cssClass = "dayCellSelected";
            }
         }

         return cssClass;
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

      protected override void Render(HtmlTextWriter writer)
      {
         string width = "100%";
         StringBuilder html = new StringBuilder();

         html.AppendFormat("<div class='monthGrid' style='height:{0}; margin:{1}; width:auto;position : relative;' >",
                           width, Margin);
         writer.Write(html);
         RenderSwitcher(writer);

         RenderMonthGrid(writer);

         html = new StringBuilder();

         if (isFullInit)
         {
            html.Append("<script type='text/javascript'>");
            html.AppendFormat("document.observe('dom:loaded', function() {1} MonthManager.getInstance(\"{0}\").Init(\"{0}\"); {2})",
               IdCalendar, "{", "}");
            html.Append("</script>");
         }
         else
         {
            html.AppendFormat("<img src='{0}' alt='' onload='MonthManager.getInstance(\"{1}\").Init(\"{1}\");' />",
                              StaticSettings.BlankGIF, IdCalendar);
         }

         writer.Write(html);

         writer.Write("</div>");
      }

      protected virtual string RenderDaysOfWeekHeader()
      {
         StringBuilder ctrlHTML = new StringBuilder();

         DateTime startDate = Utilities.FirstCalendarDay(new DateTime(CurrentDate.Year, CurrentDate.Month, 1));

         ctrlHTML.AppendFormat("<tr class='daysNames'><td colspan=7>{0}</td></tr>", CurrentDate.ToString("MMMM yyyy"));

         ctrlHTML.Append("<tr class='daysNames' id='idDaysNameHeader'>");

         for (int i = 0; i < 7; i++)
         {
            ctrlHTML.AppendFormat("<td style='width:14%'>{0}</td>", startDate.AddDays(i).ToString("dddd"));
         }

         ctrlHTML.Append("</tr>");

         return ctrlHTML.ToString();
      }

      protected virtual void RenderMonthGrid(HtmlTextWriter writer)
      {
         writer.Write(
            string.Format(
               "<table id='idGrid' mode='month' style='margin:0px;' {1} date='{0}' class='monthGrid' cellpadding='0' border='0' cellspacing='0' width='100%' height='100%' onselectstart='return false;'>",
               Utilities.NormalizeDate(CurrentDate), AdditionalAttributes));

         if (ShowHeader)
         {
            writer.Write(RenderDaysOfWeekHeader());
         }
         RenderDays(writer, CurrentDate);

         writer.Write("</table>");
      }

      protected override void RenderDataBeforeEvent(HtmlTextWriter writer, Event ev)
      {
      }

      protected override void RenderDataAfterEvent(HtmlTextWriter writer, Event ev)
      {
         if (ShowEventTime)
         {
            writer.Write(string.Format("<br/><span>{0}-{1}</span>", 
               Utilities.GetTime(ev.StartTime).ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern), 
               Utilities.GetTime(ev.EndTime).ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern)));
         }
      }

			protected override CalendarWebControl CreateControlInstance(Page page)
			{
				return null;
			}

      #endregion
   }
}