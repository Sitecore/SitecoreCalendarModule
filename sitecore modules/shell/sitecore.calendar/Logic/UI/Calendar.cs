using System;
using System.Text;
using System.Web;
using System.Web.UI;

using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Core;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Reflection;
using System.IO;

namespace Sitecore.Modules.EventCalendar.UI
{
  using sitecore_modules.shell.sitecore.calendar.Logic.UI;
  using sitecore_modules.shell.sitecore.calendar.Logic.Utils;

  public class Calendar : AjaxControl, ICalendarView
  {
    #region Private Fields

    private readonly string _idCalendar = Guid.NewGuid().ToString();

    private DateTime _currentDate = DateTime.Now;
    private BaseView _currentView = null;
    private bool _isWizardRender = false;
    private string _siteSettingsPath = null;

    private Control _wizard;
    private BaseView view = null;

    #endregion Private Fields

    #region Accessor Methods

    public string CustomView { get; set; }

    public bool SwitchViewMode { get; set; }

    public string ViewSettingsPath { get; set; }

    public ViewModeOption ViewMode { get; set; }

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

    public DateTime CurrentDate
    {
      get
      {
        return _currentDate;
      }
      set
      {
        _currentDate = value;

        if (_currentView != null)
        {
          _currentView.CurrentDate = _currentDate;
        }
      }
    }

    #endregion Accessor Methods

    #region Initialization Methods

    public Calendar()
    {
      ViewMode = ViewModeOption.Month;
      SecurityManager.Init();
      SiteSettingsPath = new ModuleSettings(SiteSettingsPath).ModuleSettingsItemPath;

      if ((HttpContext.Current.Request.UserLanguages != null) &&
          (!String.IsNullOrEmpty(HttpContext.Current.Request.UserLanguages[0])))
      {
        Utilities.SetCurrentCulture(HttpContext.Current.Request.UserLanguages[0]);
      }
    }

    protected override void OnInit(EventArgs e)
    {
      Page.RegisterRequiresControlState(this);

      _wizard = Page.LoadControl(StaticSettings.EventWizardPath);
    }

    protected override void OnLoad(EventArgs e)
    {
      if (!Page.IsPostBack && !Page.IsCallback && Page.Items["isWizardAdded"] == null)
      {
        _isWizardRender = true;
        Page.Items["isWizardAdded"] = true;
      }

      initAjax();
      InitView();
    }

    protected virtual void InitView()
    {
      Item settingsItem = null;

      var moduleSettings = new ModuleSettings(SiteSettingsPath);

      if (!string.IsNullOrEmpty(ViewSettingsPath))
      {
        settingsItem = moduleSettings.GetActiveDatabase().GetItem(ViewSettingsPath);
      }

      if (!string.IsNullOrEmpty(CustomView))
      {
        view = (BaseView)ReflectionUtil.CreateObject(CustomView,
                                       new object[] { _idCalendar, settingsItem, SiteSettingsPath });
      }

      if (view == null)
      {

        if (settingsItem != null)
        {
          if (settingsItem.TemplateID == CalendarIDs.AgendaViewTemplate)
          {
            ViewMode = ViewModeOption.Agenda;
            view = new AgendaView(_idCalendar, settingsItem, SiteSettingsPath);
          }
          else if (settingsItem.TemplateID == CalendarIDs.DayViewTemplate)
          {
            ViewMode = ViewModeOption.Day;
            view = new DayView(_idCalendar, settingsItem, SiteSettingsPath);
          }
          else if (settingsItem.TemplateID == CalendarIDs.MonthViewTemplate)
          {
            ViewMode = ViewModeOption.Month;
            view = new MonthView(_idCalendar, settingsItem, SiteSettingsPath);
          }
          else if (settingsItem.TemplateID == CalendarIDs.WeekViewTemplate)
          {
            ViewMode = ViewModeOption.Week;
            view = new WeekView(_idCalendar, settingsItem, SiteSettingsPath);
          }
        }
        else
        {
          switch (ViewMode)
          {
            case ViewModeOption.Week:
              settingsItem = moduleSettings.WeekViewSettings;
              view = new WeekView(_idCalendar, settingsItem, SiteSettingsPath);
              break;
            case ViewModeOption.Day:
              settingsItem = moduleSettings.DayViewSettings;
              view = new DayView(_idCalendar, settingsItem, SiteSettingsPath);
              break;
            case ViewModeOption.Agenda:
              settingsItem = moduleSettings.AgendaViewSettings;
              view = new AgendaView(_idCalendar, settingsItem, SiteSettingsPath);
              break;
            default:
              settingsItem = moduleSettings.MonthViewSettings;
              view = new MonthView(_idCalendar, settingsItem, SiteSettingsPath);
              break;
          }
        }
      }

      if (view != null)
      {
        view.CurrentDate = _currentDate;
        Controls.Add(view);
        _currentView = view;
      }
      else
      {
        Log.Error("Calendar: CalendarView provided settings item of incorrect type!", this);
        Log.Error("Calendar: Settings item path: " + settingsItem.Paths.Path, this);
        Log.Error("Calendar: Settings item type: " + settingsItem.TemplateName, this);
      }
    }

    #endregion Initialization Methods

    #region Control State Methods

    protected override object SaveControlState()
    {
      return new object[] { _currentDate, ViewMode };
    }

    protected override void LoadControlState(object state)
    {
      if (state != null)
      {
        object[] tmpState = (object[])state;
        _currentDate = (DateTime)tmpState[0];
        ViewMode = (ViewModeOption)tmpState[1];
      }
    }

    #endregion Control State Methods

    #region Render Methods

    public Control RenderHeader()
    {
      if (_currentView != null)
      {
        return _currentView.RenderHeader();
      }

      return null;
    }

    protected override void OnPreRender(EventArgs e)
    {
      if (HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate] != null)
      {
        CurrentDate = (DateTime)HttpContext.Current.Session[StaticSettings.SessionStateCurrentDate];
      }

      base.OnPreRender(e);
    }

    protected override void Render(HtmlTextWriter output)
    {
      if (this.Page == null)
      {
        base.Render(output);
      }
      else
      {
        var writer = new HtmlTextWriter(new StringWriter());

        if (_isWizardRender)
        {
          writer.Write("<div>");
          _wizard.RenderControl(writer);
          writer.Write("</div>");
        }

        var html = new StringBuilder();

        if (_currentView != null)
        {
          base.Render(writer);

          if (_currentView is MonthView)
          {
            html.AppendFormat(
              "<div name='calendar' id='{0}' siteSettings='{1}' viewSettingPath='{2}' class='calendarMonth'>",
              _idCalendar,
              SiteSettingsPath,
              ViewSettingsPath);
          }
          else
          {
            html.AppendFormat(
              "<div name='calendar' id='{0}' siteSettings='{1}' viewSettingPath='{2}' class='calendar'>",
              _idCalendar,
              SiteSettingsPath,
              ViewSettingsPath);
          }

          writer.Write(html);
          _currentView.RenderControl(writer);
          writer.Write("</div>");
        }

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
      var instance = new Calendar
      {
        Page = page,
        ViewMode = this.ViewMode,
        ViewSettingsPath = this.ViewSettingsPath,
        SiteSettingsPath = this.SiteSettingsPath,
        CustomView = this.CustomView,
        SwitchViewMode = this.SwitchViewMode,
        CurrentDate = this.CurrentDate
      };

      instance.OnInit(new EventArgs());
      instance.OnLoad(new EventArgs());
      instance.OnPreRender(new EventArgs());

      return instance;
    }

    #endregion Render Methods
  }
}