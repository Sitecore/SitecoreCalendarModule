using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
	using sitecore_modules.shell.sitecore.calendar.Logic.UI;
	using Sitecore.Modules.EventCalendar.sitecore_modules.shell.sitecore.calendar.Logic.Utils;

	public class WeekView : ExtendBaseView
   {
      #region Private Variables

      private string _headerFormat;
      private int _numHoursDisplayed;
      private int _startHour;
      private WeekViewSettings settings;
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
               _startHour = _startHour > 0 ? _startHour : 0;
            }
         }
      }

      #endregion

      #region Initialization Methods

      public WeekView(string idCalendar, Item week_settings, string site_settings_path)
      {
         init(idCalendar, week_settings, site_settings_path);
      }

      protected override void OnInit(EventArgs e)
      {
         base.OnInit(e);
         isFullInit = true;
      }

      private void init(string idCalendar, Item week_settings, string site_settings_path)
      {
         settings = new WeekViewSettings(week_settings, site_settings_path);
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

      protected void RenderEvents(HtmlTextWriter writer, DateTime date)
      {
         date = new DateTime(date.Year, date.Month, date.Day, _startHour, 0, 0);

				 if (!VirtualPreviewUtil.IsVirtualPreview(this.Page))
				 {
				 	base.RenderEvents(
				 		writer,
				 		date,
				 		"calEvent='true'",
				 		string.Format(
				 			"onclick='EventWizard.getInstance().ExpandEvent(this, \"{1}\", \"{2}\");'",
				 			IdCalendar,
				 			siteSettings,
				 			settings.ID),
				 		"eventInline",
				 		"eventInline",
				 		true);
				 }
      }

      protected void RenderDays(HtmlTextWriter writer, DateTime visibleDate)
      {
         DateTime startDate = Utilities.FirstCalendarDay(visibleDate);

				if (VirtualPreviewUtil.IsVirtualPreview(this.Page))
				{
					writer.Write(
						 "<tr valign='top' height='100%'><td colspan='2'><div style='height: 270px; overflow-y: scroll; overflow-x: hidden; position:relative;'><table class='weekGrid' width='100%' cellpadding='0' border='0' cellspacing='0' onselectstart='return false;' style='position:relative;'><tr><td>");
				}
				else
				{
					writer.Write(
						"<tr valign='top' height='100%'><td colspan='2'><div style='height: 0px; overflow-y: scroll; overflow-x: hidden; position:relative;'><table class='weekGrid' width='100%' cellpadding='0' border='0' cellspacing='0' onselectstart='return false;' style='position:relative;'><tr><td>");
				}

				RenderTimingColumn(writer);
         writer.Write("</td>");

         for (int dayNumber = 0; dayNumber < 7; dayNumber++)
         {
            writer.Write("<td class='dayCellWeek' width='14%'>");

            RenderTimingGrid(writer, startDate.AddDays(dayNumber), RenderEvents);

            writer.Write("</td>");
         }

         writer.Write("</tr></table></div></td></tr>");
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

      protected override void Render(HtmlTextWriter writer)
      {
         string width = "100%";

         var html = new StringBuilder();
         html.AppendFormat("<div class='weekGrid' style='height:{0}; margin:{1}; width:auto;'>",
                           width, Margin);
         writer.Write(html);
         RenderSwitcher(writer);

         if (!ReadOnly)
         {
            RenderEventToolbar(writer);
         }

         renderWeekGrid(writer);

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

      private void renderWeekGrid(HtmlTextWriter writer)
      {
         StringBuilder html = new StringBuilder();
         html.AppendFormat(
            "<table id='idGrid' mode='week' startHour='{0}' date='{1}' numHours='{2}' class='weekGrid' width='100%'  cellpadding='0' border='0' cellspacing='0' onselectstart='return false;'",
            _startHour, Utilities.NormalizeDate(CurrentDate), _numHoursDisplayed);

         if (ReadOnly != true)
         {
            html.AppendFormat(" onmousedown='WeekManager.getInstance(\"{0}\").eventController(event);' ", IdCalendar);
         }

         html.Append(">");

         if (ShowHeader)
         {
            html.Append(renderDaysOfWeekHeader());
         }

         writer.Write(html);

         RenderDays(writer, CurrentDate);

         writer.Write("</table>");
      }

      private string renderDaysOfWeekHeader()
      {
         StringBuilder html = new StringBuilder();

         DateTime startDate = Utilities.FirstCalendarDay(CurrentDate);

         html.Append("<tr class='daysNames'>");

         html.AppendFormat(
            "<td id='idemptycell{0}' class='emptycell'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td><td align='left' class='daycolumn'><table cellpadding='0' border='0' cellspacing='0' onselectstart='return false;' width='100%'><tr>",
            IdCalendar);

         for (int i = 0; i < 7; i++)
         {
            html.AppendFormat(
               "<td style='width:{3}; height:10px;' align='center'><a link='' class='aLink' onclick='ViewMode.getInstance().SwitchToDayViewToDay(\"{0}\", \"{1}\");'>{2}</a></td>",
               IdCalendar, Utilities.NormalizeDate(startDate.AddDays(i)), startDate.AddDays(i).ToString(HeaderFormat),
               i != 6 ? "14%" : "16%");
         }

         html.Append("</tr></table></td></tr>");

         return html.ToString();
      }

			protected override void RenderDataBeforeEvent(HtmlTextWriter writer, Event ev) { }

			protected override void RenderDataAfterEvent(HtmlTextWriter writer, Event ev) { }

			protected override CalendarWebControl CreateControlInstance(Page page)
			{
				return null;
			}

      #endregion
   }
}