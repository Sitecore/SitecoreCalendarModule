using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
   public class DatePicker : BasePicker
   {
      protected System.Web.UI.WebControls.Calendar navCalendar = new System.Web.UI.WebControls.Calendar();

      protected override void OnInit(EventArgs e)
      {
         navCalendar.Width = Unit.Pixel(152);
         navCalendar.Height = Unit.Pixel(89);
         navCalendar.DayNameFormat = DayNameFormat.FirstTwoLetters;
         
         navCalendar.BorderStyle = BorderStyle.Solid;
         navCalendar.BorderWidth = Unit.Pixel(1);
         navCalendar.CellPadding = 0;
         navCalendar.EnableTheming = true;
         navCalendar.EnableViewState = false;        
         navCalendar.CssClass = "navCalendar";
         
         navCalendar.VisibleMonthChanged += navCalendar_VisibleMonthChanged;
         navCalendar.DayRender += navCalendar_DayRender;
         navCalendar.SelectionChanged += navCalendar_SelectionChanged;

         navCalendar.SelectedDayStyle.CssClass = "navDaySelected";

         navCalendar.DayStyle.CssClass = "navCalendarText";

         navCalendar.WeekendDayStyle.CssClass = "navWeekdays";

         navCalendar.OtherMonthDayStyle.CssClass = "navOtherMonth";

         navCalendar.NextPrevStyle.CssClass = "navNextPrev";
         navCalendar.NextPrevStyle.ForeColor = System.Drawing.Color.White;

         navCalendar.DayHeaderStyle.CssClass = "navDayHeader";

         navCalendar.TitleStyle.CssClass = "navHeader";
         navCalendar.TitleStyle.BackColor = System.Drawing.Color.White;

         navCalendar.Style.Add("margin", Margin);

         Controls.Add(navCalendar);
      }


      protected override void Render(HtmlTextWriter writer)
      {
         navCalendar.VisibleDate = CurrentDate;

         writer.Write(string.Format("<div class='navCalendarDiv' id='{0}'>", base.ID));
         navCalendar.RenderControl(writer);
         writer.Write("</div>");
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

      protected void navCalendar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
      {
         navCalendar.SelectedDate = navCalendar.VisibleDate;

         CurrentDate = navCalendar.SelectedDate;
      }


      protected void navCalendar_SelectionChanged(object sender, EventArgs e)
      {
         navCalendar.VisibleDate = navCalendar.SelectedDate;

         CurrentDate = navCalendar.SelectedDate;

         if (LinkBehaviour == LinkBehaviour.Redirect)
         {
            Page.Response.Redirect(RedirectTo);
         }
      }


      protected void navCalendar_DayRender(object sender, DayRenderEventArgs e)
      {
         if (CalendarManager.HasEvents(e.Day.Date))
         {
            e.Cell.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
         }

         if (e.Day.Date == DateTime.Today)
         {
            e.Cell.Style.Add("border", "solid 1px red");
         }
      }

   }
}