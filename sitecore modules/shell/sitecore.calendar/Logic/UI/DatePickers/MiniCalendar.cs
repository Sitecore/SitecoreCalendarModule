using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Web;

namespace Sitecore.Modules.EventCalendar.UI
{
	using System.IO;

	using sitecore_modules.shell.sitecore.calendar.Logic.UI;
	using sitecore_modules.shell.sitecore.calendar.Logic.Utils;

	public class MiniCalendar : BasePicker
   {
      #region Constants

      public static readonly string BehaviorRedirect = "Redirect";
      public static readonly string BehaviorRefresh = "Refresh Page";

      #endregion

      #region Attributes

      private EventListManager _eventListMgr = null;

      private string _linkBehavior = string.Empty;
      private string _redirectTo = string.Empty;
      private string _selectorSettingsPath = null;
      private string _siteSettingsPath = null;
      protected System.Web.UI.WebControls.Calendar navCalendar = new System.Web.UI.WebControls.Calendar();
      protected UpdatePanel updatePanel = new UpdatePanel();

      #endregion

      #region Accessor Methods

      public DateTime VisibleDate
      {
         get
         {
            return navCalendar.VisibleDate;
         }
         set
         {
            navCalendar.VisibleDate = value;
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
         }
      }

      public bool UseHasEventClass { get; set; }

      public string LinkBehavior
      {
         get
         {
            return _linkBehavior;
         }
         set
         {
            _linkBehavior = value;
         }
      }

      public string RedirectTo
      {
         get
         {
            return _redirectTo;
         }
         set
         {
            _redirectTo = value;
         }
      }

      public string SelectorSettingsPath
      {
         get
         {
            if (_selectorSettingsPath == null)
            {
               var moduleSettings = new ModuleSettings(SiteSettingsPath);
               _selectorSettingsPath = moduleSettings.MiniCalendarSettings.ID.ToString();
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

      #endregion

      #region Initialization Methods

      protected override void OnInit(EventArgs e)
      {
				base.OnInit(e);

				this.Initialize();
      }

			private void Initialize()
			{
				initAjax();

				if ((HttpContext.Current.Request.UserLanguages != null) &&
					 (!String.IsNullOrEmpty(HttpContext.Current.Request.UserLanguages[0])))
				{
					Utilities.SetCurrentCulture(HttpContext.Current.Request.UserLanguages[0]);
				}
				ModuleSettings module = new ModuleSettings(SiteSettingsPath);
				SiteSettingsPath = module.ModuleSettingsItemPath;
				Item settings_item = module.MiniCalendarSettings;
				MiniCalendarSettings settings = new MiniCalendarSettings(settings_item, SiteSettingsPath);

				EventListMgr = new EventListManager(settings.EventLists, SiteSettingsPath);
				LinkBehavior = settings.LinkBehavior;
				RedirectTo = settings.RedirectTo;
				Margin = settings.Margin;
				Title = settings.Title;

				navCalendar.Width = Unit.Pixel(152);
				navCalendar.Height = Unit.Pixel(89);
				navCalendar.DayNameFormat = DayNameFormat.FirstTwoLetters;

				navCalendar.BorderStyle = BorderStyle.Solid;
				navCalendar.BorderWidth = Unit.Pixel(1);
				navCalendar.CellPadding = 0;
				navCalendar.EnableTheming = true;
				navCalendar.EnableViewState = false;
				navCalendar.CssClass = "navCalendar";
				navCalendar.ID = "calendar" + ID;

				navCalendar.DayRender += navCalendar_DayRender;
				navCalendar.SelectionChanged += navCalendar_SelectionChanged;

				navCalendar.SelectedDayStyle.CssClass = "navDaySelected";

				navCalendar.DayStyle.CssClass = "navCalendarText";

				navCalendar.WeekendDayStyle.CssClass = "navWeekdays";

				navCalendar.OtherMonthDayStyle.CssClass = "navOtherMonth";

				navCalendar.NextPrevStyle.CssClass = "navNextPrev";
				navCalendar.NextPrevStyle.ForeColor = Color.White;

				navCalendar.DayHeaderStyle.CssClass = "navDayHeader";
				navCalendar.TitleStyle.CssClass = "navHeader";
				navCalendar.TitleStyle.BackColor = Color.White;

				updatePanel.ID = "UpdatePanel" + ID;

        if (!VirtualPreviewUtil.IsVirtualPreview(this.Page))
        {
          Controls.Add(updatePanel);
          updatePanel.ContentTemplateContainer.Controls.Add(navCalendar);
        }
        else
        {
          Controls.Add(navCalendar);
        }
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

					base.Render(writer);

					navCalendar.VisibleDate = CurrentDate;
					writer.Write(
						string.Format(
							"<div class='navCalendarDiv' id='{0}' siteSettings='{1}' selectorSettingPath='{2}' style='margin:{3}' mode='{4}' date='{5}'>",
							ID,
							SiteSettingsPath,
							SelectorSettingsPath,
							Margin,
							"minicalendar",
							Utilities.NormalizeDate(navCalendar.VisibleDate)));

					if (!String.IsNullOrEmpty(Title))
					{
						writer.Write(string.Format("<h2>{0}</h2>", Title));
					}

					foreach (Control control in Controls)
					{
						control.RenderControl(writer);
					}

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

      protected void navCalendar_DayRender(object sender, DayRenderEventArgs e)
      {
         if (UseHasEventClass && EventListMgr.HasEvents(e.Day.Date))
         {
            e.Cell.CssClass += "Events";
         }

         if (e.Day.IsToday)
         {
            e.Cell.Style.Add("border", "solid 1px red");
         }
         else if (e.Day.Date == CurrentDate)
         {
            e.Cell.CssClass = "navCalendarTextSelected";
         }
      }

			protected override CalendarWebControl CreateControlInstance(Page page)
			{
				var instance = new MiniCalendar
				{
					Page = page,
					SiteSettingsPath = this.SiteSettingsPath,
					CurrentDate = this.CurrentDate,
					VisibleDate = this.VisibleDate,
					EventListMgr = this.EventListMgr,
					UseHasEventClass = this.UseHasEventClass,
					LinkBehavior = this.LinkBehavior,
					RedirectTo = this.RedirectTo,
					SelectorSettingsPath = this.SelectorSettingsPath
				};

				instance.Initialize();

				return instance;
			}

      #endregion

      #region Event Handlers

      protected void navCalendar_SelectionChanged(object sender, EventArgs e)
      {
         navCalendar.VisibleDate = navCalendar.SelectedDate;

         CurrentDate = navCalendar.SelectedDate;

         if (LinkBehavior == BehaviorRedirect)
         {
					 	HttpContext.Current.Response.AddHeader("requestFrom", "CalendarModule");
         		Page.Response.Redirect(string.IsNullOrEmpty(RedirectTo) ? Page.Request.RawUrl : RedirectTo);
         }
         else if (LinkBehavior == BehaviorRefresh)
         {
            WebUtil.Redirect(LinkManager.GetItemUrl(Sitecore.Context.Item));
         }
      }

      #endregion
	 }
}