using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
	using sitecore_modules.shell.sitecore.calendar.Logic.UI;
	using sitecore_modules.shell.sitecore.calendar.Logic.Utils;

	public class DayView : ExtendBaseView
   {
      #region Private Variables

      private string _headerFormat;
      private int _numHoursDisplayed;
      private int _startHour;
      private DayViewSettings settings;
      private string siteSettings;
      private bool isFullInit = false;

      #endregion

      #region Field Access Methods

      public string HeaderFormat
      {
         get
         {
            return _headerFormat;
         }
         set
         {
            _headerFormat = value;
         }
      }

      public int NumHoursDisplayed
      {
         get
         {
            return _numHoursDisplayed;
         }
         set
         {
            _numHoursDisplayed = value;
         }
      }

      public int StartHour
      {
         get
         {
            return _startHour;
         }
         set
         {
            _startHour = value;
            if (_startHour + _numHoursDisplayed > 24)
            {
               _startHour = 24 - _numHoursDisplayed;
               _startHour = _startHour >= 0 ? _startHour : 0;
            }
         }
      }

      #endregion

      #region Initialization Methods

      public DayView(string idCalendar, Item day_settings, string site_settings_path)
      {
         init(idCalendar, day_settings, site_settings_path);
      }

      protected override void OnInit(EventArgs e)
      {
         base.OnInit(e);
         isFullInit = true;
      }

      private void init(string idCalendar, Item day_settings, string site_settings_path)
      {
         settings = new DayViewSettings(day_settings, site_settings_path);
         siteSettings = site_settings_path;

         IdCalendar = idCalendar;

         HeaderFormat = settings.HeaderFormat;
         Margin = settings.Margin;
         NumHoursDisplayed = settings.NumberHoursDisplayed;
         StartHour = settings.StartHour;

         init(settings, site_settings_path);
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

      #endregion

      #region Rendering Methods

      public override Control RenderHeader()
      {
         HtmlGenericControl header = new HtmlGenericControl();
         if (ShowHeader)
         {
            header.InnerHtml = renderDaysOfWeekHeader();
         }
         return header;
      }

      protected virtual void RenderEvents(HtmlTextWriter writer, DateTime date)
      {
         date = new DateTime(date.Year, date.Month, date.Day, _startHour, 0, 0);

				 if (!VirtualPreviewUtil.IsVirtualPreview(this.Page))
				 {
				 	base.RenderEvents(
				 		writer,
				 		date,
				 		"calEvent='true'",
				 		string.Format(
				 			"onclick='EventWizard.getInstance(\"{0}\").ExpandEvent(this, \"{1}\", \"{2}\");'",
				 			IdCalendar,
				 			siteSettings,
				 			settings.ID),
				 		"eventInline",
				 		"eventInline",
				 		true);
				 }
      }

      protected virtual void RenderDays(HtmlTextWriter writer, DateTime visibleDate)
      {
         writer.Write(
            "<tr valign='top' height='100%'><td colspan='2' width='100%'><div style='width: 100%; height: 400px; overflow-y: scroll; overflow-x: hidden; position:relative;'><table class='dayGrid' width='100%' cellpadding='0' border='0' cellspacing='0' onselectstart='return false;' style='position:relative;'><tr><td>");
         RenderTimingColumn(writer);
         writer.Write("</td><td width='100%' class='dayCellWeek'>");

         RenderTimingGrid(writer, CurrentDate, RenderEvents);
         writer.Write("</td></tr></table></div></td></tr>");
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

      protected override void Render(HtmlTextWriter writer)
      {
         string width = "100%";

         var html = new StringBuilder();
         html.AppendFormat("<div class='dayGrid' style=' height:{0}; margin:{1}; width:auto;' >",
                           width, Margin);
         writer.Write(html);
         RenderSwitcher(writer);
            
         if (!ReadOnly)  
         {
            RenderEventToolbar(writer);
         }
         renderDayGrid(writer);
         if (IsReinitNeed)
         {
            if (isFullInit)
            {
               writer.Write("<script type='text/javascript'>");
               writer.Write(string.Format("document.observe('dom:loaded', function() {1} WeekManager.getInstance(\"{0}\").Init(\"{0}\"); {2})",
                  IdCalendar, "{", "}"));
               writer.Write("</script>");
               
            }
            else
            {
               writer.Write(
                     string.Format(
                           "<img src='{0}' alt='' onload='WeekManager.getInstance(\"{1}\").Init(\"{1}\");'/>",
                           StaticSettings.BlankGIF,
                           IdCalendar));
            }
         }
         else
         {
            writer.Write("<img src='" + StaticSettings.BlankGIF + "' alt=''/>");
         }

         writer.Write("</div>");
      }

      private void renderDayGrid(HtmlTextWriter writer)
      {
         writer.Write(
            string.Format(
               "<table id='idGrid' width='100%' mode='day' startHour='{0}' date='{1}' numHours='{2}' onselectstart='return false;' class='dayGrid' cellpadding='0' border='0' cellspacing='0' onselectstart='return false;'",
               _startHour, Utilities.NormalizeDate(CurrentDate), _numHoursDisplayed), Margin);

         if (!ReadOnly)
         {
            writer.Write(
               string.Format(" onmousedown='WeekManager.getInstance(\"{0}\").eventController(event);' ", IdCalendar));
         }

         writer.Write(">");

         if (ShowHeader)
         {
            writer.Write(renderDaysOfWeekHeader());
         }

         RenderDays(writer, CurrentDate);

         writer.Write("</table>");
      }

      private string renderDaysOfWeekHeader()
      {
         DateTime startDate = CurrentDate;

         StringBuilder ctrlHTML = new StringBuilder();
         string mode = HttpContext.Current.Session[IdCalendar] as string;
         StringBuilder htmlReturnLink = new StringBuilder();
         if (mode != null)
         {
            if (mode == "month")
            {
               htmlReturnLink.AppendFormat(
                  "<a link='' class='aLink' onclick='ViewMode.getInstance().SwitchToMonthViewToDay(\"{0}\", \"{1}\");'>{2}</a>",
                  IdCalendar, Utilities.NormalizeDate(CurrentDate), settings.ReturnToMonthViewText);
            }
            else if (mode == "week")
            {
               htmlReturnLink.AppendFormat(
                  "<a link='' class='aLink' onclick='ViewMode.getInstance().SwitchToWeekViewToDay(\"{0}\", \"{1}\");'>{2}</a>",
                  IdCalendar, Utilities.NormalizeDate(CurrentDate), settings.ReturnToWeekViewText);
            }
         }

         ctrlHTML.Append("<tr class='daysNames'>");
         ctrlHTML.AppendFormat(
            "<td><img src='{2}' height='1px' width='41px' alt=''/></td><td><table width='100%'><tr><td width='60%' align='right'>{0}</td><td align='right'>{1}</td></tr></table></td>",
            startDate.ToString(HeaderFormat), htmlReturnLink, StaticSettings.BlankGIF);

         ctrlHTML.Append("</tr>");
         return ctrlHTML.ToString();
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