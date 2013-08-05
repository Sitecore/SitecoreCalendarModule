using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Web;

namespace Sitecore.Modules.EventCalendar.UI
{
	using System.IO;
	using sitecore_modules.shell.sitecore.calendar.Logic.Utils;

	using sitecore_modules.shell.sitecore.calendar.Logic.UI;

	public class DateSelector : BasePicker
   {                                                                       
      #region Constants

      private const string DayMonthYear = "Day Month Year";
      private const int maxYearsToShow = 100;
      private const int minYear = 1;
      private const int minYearsToShow = 0;
      private const string MonthDayYear = "Month Day Year";
      private const string MonthYear = "Month Year";

      private const string YearMonth = "Year Month";
      private const string YearMonthDay = "Year Month Day";

      #endregion

      #region Attributes

      private readonly LinkButton _selectDate = new LinkButton();
      private readonly DropDownList _selectorDay = new DropDownList();
      private readonly DropDownList _selectorMonth = new DropDownList();
      private readonly DropDownList _selectorYear = new DropDownList();

      private readonly LinkButton _setTodayDate = new LinkButton();

      private string _format = null;
      private string _linkText = null;
      private string _selectorSettingsPath = null;
      private string _siteSettingsPath = null;
      private int _startYear = DateTime.Now.Year;
      private string _todayLinkText = null;
      private int _yearsToShow = 5;

      #endregion

      #region Accessor Methods

      public string Format
      {
         get
         {
            return _format;
         }
         set
         {
            _format = value;
         }
      }

      public string LinkText
      {
         get
         {
            return _linkText;
         }
         set
         {
            _linkText = value;
         }
      }

      public string SelectorSettingsPath
      {
         get
         {
            if (_selectorSettingsPath == null)
            {
               var moduleSettings = new ModuleSettings(SiteSettingsPath);
               _selectorSettingsPath = moduleSettings.DateSelectorSettings.ID.ToString();
            }

            return _selectorSettingsPath;
         }
         set
         {
            _selectorSettingsPath = value;
         }
      }

      public string SiteSettingsPath
      {
         get
         {
            return _siteSettingsPath;
         }
         set
         {
            _siteSettingsPath = new ModuleSettings(value).ModuleSettingsItemPath;
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

      public string TodayLinkText
      {
         get
         {
            return _todayLinkText;
         }
         set
         {
            _todayLinkText = value;
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

      #endregion

      #region Initialization Methods

			protected override void OnInit(EventArgs e)
			{
				base.OnInit(e);

				this.Initialize();
      }

			private void Initialize()
			{
				ModuleSettings module = new ModuleSettings(SiteSettingsPath);

				SiteSettingsPath = module.ModuleSettingsItemPath;
				Item settings_item = module.DateSelectorSettings;
				DateSelectorSettings settings = new DateSelectorSettings(settings_item, SiteSettingsPath);

				Format = settings.Format;
				LinkText = settings.LinkText;
				Margin = settings.Margin;
				Title = settings.Title;
				TodayLinkText = settings.TodayLinkText;

				_selectorYear.EnableViewState = false;
				_selectorMonth.EnableViewState = false;
				_selectorDay.EnableViewState = false;

				_selectorYear.Attributes["id"] = "idSelectorYear";
				_selectorMonth.Attributes["id"] = "idSelectorMonth";
				_selectorDay.Attributes["id"] = "idSelectorDay";
				_selectorYear.Attributes["onclick"] = "UpdateDateSelector();";
				_selectorMonth.Attributes["onclick"] = "UpdateDateSelector();";

				initYearSelector();
				initMonthSelector();
				initDaySelector();

				setSelectedDate(CurrentDate);

				_selectDate.Text = LinkText;
				_selectDate.CssClass = "selectorButton";
				_selectDate.Click += onSelectDate;
				Controls.Add(_selectDate);

				_setTodayDate.Text = TodayLinkText;
				_setTodayDate.CssClass = "selectorButton";
				_setTodayDate.Click += onSetToday;

				Controls.Add(_setTodayDate);
			}

			private void initYearSelector()
      {
         int endDate = _startYear + _yearsToShow;

         for (int i = _startYear - _yearsToShow; i <= endDate; i++)
         {
            string year = i.ToString();
            ListItem item = new ListItem(year, year);

            _selectorYear.Items.Add(item);
         }

         Controls.Add(_selectorYear);
      }

      private void initMonthSelector()
      {
         DateTime dt = new DateTime(2000, 1, 1, 0, 0, 0);

         for (int i = 1; i <= 12; i++)
         {
            ListItem item = new ListItem(dt.ToString("MMM"), i.ToString());

            _selectorMonth.Items.Add(item);

            dt = dt.AddMonths(1);
         }

         Controls.Add(_selectorMonth);
      }

      private void initDaySelector()
      {
         for (int i = 1; i <= CultureInfo.InvariantCulture.Calendar.GetDaysInMonth(CurrentDate.Year, CurrentDate.Month);
              ++i)
         {
            string day = i.ToString();

            _selectorDay.Items.Add(new ListItem(day, day));
         }

         Controls.Add(_selectorDay);
      }

      #endregion

      #region Render Methods

      protected override void Render(HtmlTextWriter output)
      {
				if (this.Page == null)
				{
					base.Render(output);
				}
				else
				{
					var writer = new HtmlTextWriter(new StringWriter());

					writer.Write("<div>");

					RenderControl(writer);

					writer.Write("</div>");

					string pageHtml = writer.InnerWriter.ToString();

					if (VirtualPreviewUtil.IsVirtualPreview(this.Page))
					{
						pageHtml = HtmlParserUtil.RemoveAllOnFunctions(pageHtml);
						pageHtml = HtmlParserUtil.RemoveAllHrefAttributesFromLinks(pageHtml);
					}

					output.Write(pageHtml);
				}
      }

      private void setSelectedDate(DateTime date)
      {
         int endDate = _startYear + _yearsToShow;

         if (_startYear - _yearsToShow <= date.Year && date.Year <= endDate)
         {
            _selectorYear.SelectedIndex = date.Year - _startYear + _yearsToShow;
            _selectorDay.SelectedIndex = date.Day - 1;
            _selectorMonth.SelectedIndex = date.Month - 1;
         }
         else
         {
            _selectorYear.SelectedIndex = _yearsToShow;
            _selectorMonth.SelectedIndex = 0;
            _selectorDay.SelectedIndex = 0;
         }
      }

      public override void RenderControl(HtmlTextWriter output)
      {
				if (this.Page == null)
				{
					base.Render(output);
				}
				else
				{
					var writer = new HtmlTextWriter(new StringWriter());

					Control first;
					Control second;
					Control third;

					switch (Format)
					{
						case DayMonthYear:
							first = _selectorDay;
							second = _selectorMonth;
							third = _selectorYear;
							break;
						case MonthDayYear:
							first = _selectorMonth;
							second = _selectorDay;
							third = _selectorYear;
							break;
						case YearMonth:
							first = _selectorYear;
							second = _selectorMonth;
							third = null;
							break;
						case MonthYear:
							first = _selectorMonth;
							second = _selectorYear;
							third = null;
							break;
						case YearMonthDay:
							first = _selectorYear;
							second = _selectorMonth;
							third = _selectorDay;
							break;
						default:
							first = _selectorYear;
							second = _selectorMonth;
							third = _selectorDay;
							break;
					}

					writer.Write("<div class='quickDatePicker' style='margin:" + Margin + "'>");

					if (String.IsNullOrEmpty(Title) != true)
					{
						writer.Write(string.Format("<h2 class='quickDatePickerHeader'>{0}</h2>", Title));
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

					if (VirtualPreviewUtil.IsVirtualPreview(this.Page))
					{
						_selectDate.Enabled = false;
						_setTodayDate.Enabled = false;
					}

					_selectDate.RenderControl(writer);

					writer.Write("</td></tr>");

					writer.Write("<tr><td colspan='3'>");

					_setTodayDate.RenderControl(writer);

					writer.Write("</td></tr></table>");
					writer.Write("</div>");

					string pageHtml = writer.InnerWriter.ToString();

					if (VirtualPreviewUtil.IsVirtualPreview(this.Page))
					{
						pageHtml = HtmlParserUtil.RemoveAllOnFunctions(pageHtml);
						pageHtml = HtmlParserUtil.RemoveAllHrefAttributesFromLinks(pageHtml);
					}

					output.Write(pageHtml);
				}
      }

      protected override void DoRender(HtmlTextWriter writer)
      {
         Render(writer);
      }

			protected override CalendarWebControl CreateControlInstance(Page page)
			{
				var instance = new DateSelector
				{
					Page = page,
					SiteSettingsPath = this.SiteSettingsPath,
					Format = this.Format,
					LinkText = this.LinkText,
					SelectorSettingsPath = this.SelectorSettingsPath,
					StartYear = this.StartYear,
					TodayLinkText = this.TodayLinkText,
					YearsToShow = this.YearsToShow,
				};

				instance.Initialize();

				return instance;
			}

      #endregion

      #region Event Handlers

      private void onSelectDate(object sender, EventArgs args)
      {
         DateTime date;

         if (Format == MonthYear || Format == YearMonth)
         {
            _selectorDay.SelectedValue = "1";
         }

         try
         {
            date =
               new DateTime(int.Parse(_selectorYear.SelectedValue), int.Parse(_selectorMonth.SelectedValue),
                            int.Parse(_selectorDay.SelectedValue));
         }
         catch (ArgumentOutOfRangeException)
         {
            int month = int.Parse(_selectorMonth.SelectedValue);
            int year = int.Parse(_selectorYear.SelectedValue);
            date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
         }

         CurrentDate = date;

         WebUtil.Redirect(LinkManager.GetItemUrl(Sitecore.Context.Item));
      }

      private void onSetToday(object sender, EventArgs args)
      {
         CurrentDate = DateTime.Now;

         WebUtil.Redirect(LinkManager.GetItemUrl(Sitecore.Context.Item));
      }

      #endregion
   }
}