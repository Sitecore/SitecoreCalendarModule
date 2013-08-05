using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Modules.EventCalendar.UI
{
	using sitecore_modules.shell.sitecore.calendar.Logic.UI;

	public class LazyMiniCalendar : AjaxControl
   {
      #region Fields

      protected System.Web.UI.WebControls.Calendar navCalendar = new System.Web.UI.WebControls.Calendar();
      protected UpdatePanel updatePanel = new UpdatePanel();
      #endregion

      #region Methods

			protected override void OnInit(EventArgs e)
			{
				base.OnInit(e);

         initAjax();

         navCalendar.Width = Unit.Pixel(152);
         navCalendar.Height = Unit.Pixel(89);
         navCalendar.DayNameFormat = DayNameFormat.FirstTwoLetters;

         navCalendar.BorderStyle = BorderStyle.Solid;
         navCalendar.BorderWidth = Unit.Pixel(1);
         navCalendar.CellPadding = 0;
         navCalendar.EnableTheming = true;
         navCalendar.EnableViewState = false;
         navCalendar.CssClass = "navCalendar";

         navCalendar.DayRender += navCalendar_DayRender;

         navCalendar.SelectedDayStyle.CssClass = "navDaySelected";

         navCalendar.DayStyle.CssClass = "navCalendarText";

         navCalendar.WeekendDayStyle.CssClass = "navWeekdays";

         navCalendar.OtherMonthDayStyle.CssClass = "navOtherMonth";

         navCalendar.NextPrevStyle.CssClass = "navNextPrev";
         navCalendar.NextPrevStyle.ForeColor = Color.White;

         navCalendar.DayHeaderStyle.CssClass = "navDayHeader";
         navCalendar.TitleStyle.CssClass = "navHeader";
         navCalendar.TitleStyle.BackColor = Color.White;
         navCalendar.ID = "calendar" + ID;
         updatePanel.ID = "UpdatePanel2";
         Controls.Add(updatePanel);

         updatePanel.ContentTemplateContainer.Controls.Add(navCalendar);

         base.OnInit(e);
      }

      protected void navCalendar_DayRender(object sender, DayRenderEventArgs e)
      {
         if (e.Day.Date == DateTime.Today)
         {
            e.Cell.Style.Add("border", "solid 1px red");
         }
      }

      protected override void Render(HtmlTextWriter writer)
      {
         updatePanel.RenderControl(writer);
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

			protected override CalendarWebControl CreateControlInstance(Page page)
			{
				return null;
			}
      #endregion
   }
}