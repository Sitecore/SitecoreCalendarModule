using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
   public class QuickDatePicker :  BasePicker
   {
      private DropDownList _selectorYear = new DropDownList();
      private DropDownList _selectorMonth = new DropDownList();
      private DropDownList _selectorDay = new DropDownList();

      private LinkButton _setTodayDate = new LinkButton();
      private LinkButton _selectDate = new LinkButton();

      private int _startYear = DateTime.Now.Year;
      private int _yearsToShow = 5;

      private const int minYear = 1;
      private const int maxYearsToShow = 100;
      private const int minYearsToShow = 0;

      private bool _dateFormatAssignment = false;
      private DateFormat _dateFormat = DateFormat.YearMonthDay;

      protected override void OnInit(EventArgs e)
      {
         initYearSelector();

         initMonthSelector();

         initDaySelector();

         _selectDate.Text = LinkText;
         _selectDate.CssClass = "selectorButton";
         _selectDate.Click += onSelectDate;
         Controls.Add(_selectDate);

         _setTodayDate.Text = TodayLinkText;
         _setTodayDate.CssClass = "selectorButton";
         _setTodayDate.Click += onSetToday;
         Controls.Add(_setTodayDate);
      }

      protected override void Render(HtmlTextWriter writer)
      {
         if (HttpContext.Current.Session[Settings.SessionStateCurrentDate] != null)
         {
            setSelectedDate((DateTime)HttpContext.Current.Session[Settings.SessionStateCurrentDate]);
         }
         else
         {
            setSelectedDate(DateTime.Now);
         }

         writer.Write("<div>");

         RenderControl(writer);

         writer.Write("</div>");
      }

      public override void RenderControl(HtmlTextWriter writer)
      {
         Control first;
         Control second;
         Control third;

         switch (DateFormat)
         {
            case DateFormat.DayMonthYear:
               first = _selectorDay;
               second = _selectorMonth;
               third = _selectorYear;
               break;
            case DateFormat.MonthDayYear:
               first = _selectorMonth;
               second = _selectorDay;
               third = _selectorYear;
               break;
            case DateFormat.YearDayMonth:
               first = _selectorYear;
               second = _selectorDay;
               third = _selectorMonth;
               break;
            case DateFormat.YearMonth:
               first = _selectorYear;
               second = _selectorMonth;
               third = null;
               break;
            case DateFormat.MonthYear:
               first = _selectorMonth;
               second =  _selectorYear;
               third = null;
               break;
            default:              
               first = _selectorYear;
               second = _selectorMonth;
               third = _selectorDay;
               break;
         }

         writer.Write("<div class='quickDatePicker'>");

         if (String.IsNullOrEmpty(Title) != true)
         {
            writer.Write(
               string.Format("<span class='quickDatePickerHeader'>{0}</span>", Title)
               );
         }

         writer.Write("<table><tr>");

         writer.Write("<td>");

         first.RenderControl(writer);

         writer.Write("</td><td>");

         second.RenderControl(writer);

         writer.Write("</td>");
         
         if (third != null)
         {
            writer.Write("<td>");
            third.RenderControl(writer);
            writer.Write("</td>");
         }

         writer.Write("<td>");

         _selectDate.RenderControl(writer);
         
         writer.Write("</td></tr>");

         
         writer.Write("<tr><td>");

         _setTodayDate.RenderControl(writer);
         
         writer.Write("</td></tr></table>");
         writer.Write("</div>");
      }

      public DateFormat DateFormat
      {
         get
         {
            if (!_dateFormatAssignment)
            {
               return Settings.DateFormat;
            }

            return _dateFormat;
         }
         set
         {
            _dateFormatAssignment = true;

            _dateFormat = value;
         }
      }


      public int StartYear
      {
         get
         {
            return _startYear;
         }
         set
         {
            _startYear = value;

            if (_startYear < minYear)
            {
               _startYear = minYear;
            }
         }
      }

      public int YearsToShow
      {
         get
         {
            return _yearsToShow;
         }
         set
         {
            _yearsToShow = value;

            if (_yearsToShow < minYearsToShow)
            {
               _yearsToShow = minYearsToShow;
            }
            else if (_yearsToShow > maxYearsToShow)
            {
               _yearsToShow = maxYearsToShow;
            }

         }
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

      private void initYearSelector()
      {
         int endDate = _startYear + _yearsToShow;

         for (int i = _startYear; i <= endDate; i++)
         {
            string year = i.ToString();
            ListItem item = new ListItem(year, year);

            _selectorYear.Items.Add(item);
         }

         Controls.Add(_selectorYear);
      }

      private void initMonthSelector()
      {
         DateTime dt = new DateTime(2000,1,1,0,0,0);
         
         for (int i = 1; i <= 12; i++)
         {
            ListItem item = new ListItem( dt.ToString("MMM"), i.ToString() );

            _selectorMonth.Items.Add(item);

            dt = dt.AddMonths(1);
         }

         Controls.Add(_selectorMonth);
      }

      private void initDaySelector()
      {
         for(int i=1; i<=31; i++)
         {
            string day = i.ToString();

            _selectorDay.Items.Add(new ListItem( day, day));
         }

      
         Controls.Add(_selectorDay);
      }

      private void setSelectedDate(DateTime date)
      {
         int endDate = _startYear + _yearsToShow;

         if (_startYear <= date.Year && date.Year <= endDate)
         {
            _selectorYear.SelectedIndex = date.Year - _startYear;
            _selectorDay.SelectedIndex = date.Day - 1;
            _selectorMonth.SelectedIndex = date.Month - 1;
         }
         else
         {
            _selectorYear.SelectedIndex = 0;
            _selectorMonth.SelectedIndex= 0;
            _selectorDay.SelectedIndex = 0;
         }
      }

      private void onSelectDate(object sender, EventArgs args)
      {
         DateTime date;

         try
         {
            date = new DateTime( int.Parse(_selectorYear.SelectedValue),
                                 int.Parse(_selectorMonth.SelectedValue),
                                 int.Parse(_selectorDay.SelectedValue));
         }
         catch (ArgumentOutOfRangeException)
         {
            int month = int.Parse(_selectorMonth.SelectedValue);
            int year = int.Parse(_selectorYear.SelectedValue);
            date = new DateTime( year,
                                 month,
                                 DateTime.DaysInMonth(year, month)
                                 );
         }

         CurrentDate = date;

         Sitecore.Web.WebUtil.Redirect(Sitecore.Context.Item.Paths.GetFriendlyUrl());
      }

      private void onSetToday(object sender, EventArgs args)
      {
         CurrentDate = DateTime.Now;

         Sitecore.Web.WebUtil.Redirect(Sitecore.Context.Item.Paths.GetFriendlyUrl());
      }
   }
}
