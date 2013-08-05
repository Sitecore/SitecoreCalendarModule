using System;
using System.Web.UI;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Modules.EventCalendar.sitecore_modules.shell.sitecore.calendar.Logic.UI;

namespace Sitecore.Modules.EventCalendar.UI
{
  using System.IO;
  using sitecore_modules.shell.sitecore.calendar.Logic.Utils;

  public class ProgressBar : CalendarWebControl
  {
    private string _progressBarSettingsPath = null;
    private string _siteSettingsPath = null;
    private string image = "";
    private ProgressBarSettings settings = null;
    private string text = "";

    public string Text
    {
      get
      {
        return text;
      }
      set
      {
        text = value;
      }
    }

    public string Image
    {
      get
      {
        return image;
      }
      set
      {
        image = value;
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

    public string ProgressBarSettingsPath
    {
      get
      {
        if (_progressBarSettingsPath == null)
        {
          var moduleSettings = new ModuleSettings(SiteSettingsPath);
          _progressBarSettingsPath = moduleSettings.ProgressBarSettings.ID.ToString();
        }

        return _progressBarSettingsPath;
      }
      set
      {
        _progressBarSettingsPath = value;
      }
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

        base.Render(output);

        if (!Sitecore.Context.PageMode.IsPageEditor)
        {
          writer.Write("<div id='idProgressBar' class='progressBar' style='display:block; visibility:hidden'>");
        }

        writer.Write("<img align=top alt='' src='{0}'/>", Image);
        writer.Write("<span class='progressText'>{0}</span>", Text);

        if (!Sitecore.Context.PageMode.IsPageEditor)
        {
          writer.Write("</div>");
        }

        string pageHtml = writer.InnerWriter.ToString();

        if (!Sitecore.Context.PageMode.IsPageEditor)
        {
          pageHtml = HtmlParserUtil.RemoveAllOnFunctions(pageHtml);
          pageHtml = HtmlParserUtil.RemoveAllHrefAttributesFromLinks(pageHtml);
        }

        output.Write(pageHtml);
      }
    }

    protected override void DoRender(HtmlTextWriter output)
    {
      this.Render(output);
    }

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);

      this.Initialize();
    }

    private void Initialize()
    {
      var moduleSettings = new ModuleSettings(SiteSettingsPath);
      SiteSettingsPath = moduleSettings.ModuleSettingsItemPath;
      Item settings_item = moduleSettings.ProgressBarSettings;
      settings = new ProgressBarSettings(settings_item, SiteSettingsPath);
      Image = settings.Image;
      Text = settings.Text;
    }

    protected override CalendarWebControl CreateControlInstance(Page page)
    {
      var instance = new ProgressBar
      {
        Page = page,
        SiteSettingsPath = this.SiteSettingsPath,
        ProgressBarSettingsPath = this.ProgressBarSettingsPath,
        Text = this.Text
      };

      instance.Initialize();

      return instance;
    }
  }
}