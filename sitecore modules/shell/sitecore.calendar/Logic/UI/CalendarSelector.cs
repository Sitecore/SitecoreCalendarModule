using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Web;

using Sitecore.Modules.EventCalendar.sitecore_modules.shell.sitecore.calendar.Logic.UI;

namespace Sitecore.Modules.EventCalendar.UI
{
	using System.IO;
	using sitecore_modules.shell.sitecore.calendar.Logic.Utils;

	public class CalendarSelector : CalendarWebControl
   {
      #region Constants

      public static readonly string FormatAsDropList = "Droplist";
      public static readonly string FormatAsToggleList = "Toggle List";

      #endregion

      #region Attributes

      private readonly List<string> _selectedList = new List<string>();
      private readonly LinkButton _submit = new LinkButton();
      private ListControl _calsCtrl;

      private string _format = string.Empty;
      private string _linkText = string.Empty;
      private string _selectorSettingsPath;
      private string _siteSettingsPath;
      private string _title = string.Empty;

      #endregion

      #region Accessor Methods

      public EventListManager EventListMgr { get; set; }

      public string Title
      {
         get
         {
            return _title;
         }
         set
         {
            _title = value;
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

      protected List<string> SelectedCalendars
      {
         get
         {
            List<string> _selectedList = null;
            if (HttpContext.Current.Session[StaticSettings.SessionStateUrl] != null &&
                (string) HttpContext.Current.Session[StaticSettings.SessionStateUrl] ==
                HttpContext.Current.Request.RawUrl)
            {
               _selectedList =
                  HttpContext.Current.Session[StaticSettings.SessionStateCalendarsList] as
                  List<string>;
            }
            else
            {
               _selectedList = null;
            }

            if (_selectedList != null)
            {
               return _selectedList;
            }

            _selectedList = new List<string>();

            int i = 0;

            foreach (var cal in EventListMgr.EventLists)
            {
               if (cal.Value.Selected)
               {
                  if ((ShowAsDropDownList && i == 0) || (ShowAsDropDownList == false))
                  {
                     _selectedList.Add(cal.Key);
										 //_selectedList.Add(cal.Key);
                  }

                  i++;
               }
            }

            HttpContext.Current.Session[StaticSettings.SessionStateUrl] =
               HttpContext.Current.Request.RawUrl;
            HttpContext.Current.Session[StaticSettings.SessionStateCalendarsList] = _selectedList;

            return _selectedList;
         }
      }

      public string SelectorSettingsPath
      {
         get
         {
            if (_selectorSettingsPath == null)
            {
               var moduleSettings = new ModuleSettings(SiteSettingsPath);
               _selectorSettingsPath = moduleSettings.CalendarSelectorSettings.ID.ToString();
            }

            return _selectorSettingsPath;
         }
         set
         {
            _selectorSettingsPath = value;
         }
      }

      public bool ShowAsDropDownList
      {
         get
         {
            return _format == FormatAsDropList;
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
				initList();

				int i = 0;

				foreach (var cal in EventListMgr.EventLists)
				{
					var item = new ListItem(cal.Value.Title, cal.Key);

					if (SelectedCalendars.Contains(cal.Key))
					{
						if ((ShowAsDropDownList && i == 0) || (ShowAsDropDownList == false))
						{
							item.Selected = true;
						}

						i++;
					}

					_calsCtrl.Items.Add(item);
				}

				Controls.Add(_calsCtrl);

				_submit.Click += onSubmit;
				_submit.Text = LinkText;
				_submit.CssClass = "selectorButton";

				Controls.Add(_submit);
			}

      private void initList()
      {
         var module = new ModuleSettings(SiteSettingsPath);
         SiteSettingsPath = module.ModuleSettingsItemPath;

         Item settings_item = null;
         if (!string.IsNullOrEmpty(SelectorSettingsPath))
         {
            settings_item = module.GetActiveDatabase().GetItem(SelectorSettingsPath);
         }

         var settings =
            new CalendarSelectorSettings(settings_item ?? module.CalendarSelectorSettings,
                                         SiteSettingsPath);

         EventListMgr = new EventListManager(settings.EventLists, SiteSettingsPath);
         Format = settings.Format;
         LinkText = settings.LinkText;
         Margin = settings.Margin;
         Title = settings.Title;

         if (ShowAsDropDownList)
         {
            _calsCtrl = new DropDownList();
         }
         else
         {
            _calsCtrl = new CheckBoxList();
         }

         _calsCtrl.EnableViewState = false;
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

					writer.Write("<div class='calendarSelector' style='margin:" + Margin + "'>");

					if (!String.IsNullOrEmpty(Title))
					{
						writer.Write(string.Format("<h2 class='calendarSelectorHeader'>{0}</h2>", Title));
					}

					_calsCtrl.RenderControl(writer);

					if (VirtualPreviewUtil.IsVirtualPreview(this.Page))
					{
						_submit.Enabled = false;
					}

					_submit.RenderControl(writer);
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
				var instance = new CalendarSelector
				{
					Page = page,
					SiteSettingsPath = this.SiteSettingsPath,
					Format = this.Format,
					LinkText = this.LinkText,
					SelectorSettingsPath = this.SelectorSettingsPath,
					EventListMgr = this.EventListMgr,
					Title = this.Title
				};

				instance.Initialize();

				return instance;
			}

      #endregion

      #region Event Handlers

      private void onSubmit(object sender, EventArgs args)
      {
         _selectedList.Clear();

         foreach (ListItem item in _calsCtrl.Items)
         {
            if (item.Selected)
            {
               _selectedList.Add(item.Value);
            }
         }

         Page.Session[StaticSettings.SessionStateCalendarsList] = _selectedList;

				 //WebUtil.Redirect(LinkManager.GetItemUrl(Sitecore.Context.Item));
				 WebUtil.Redirect(HttpContext.Current.Request.UrlReferrer.AbsoluteUri);
      }

      #endregion
   }
}