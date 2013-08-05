using System;
using Sitecore.Web.UI;
using System.Web.UI;
using System.Text;

namespace Sitecore.Modules.EventCalendar.sitecore_modules.shell.sitecore.calendar.Logic.UI
{
	using System.IO;
	using System.Web.Compilation;
	using System.Web.UI.HtmlControls;

	using EventCalendar.Utils;
	using Sitecore.Modules.EventCalendar.UI;
	using Utils;

	public abstract class CalendarWebControl: WebControl
	{
		protected abstract CalendarWebControl CreateControlInstance(Page page);

		protected override void OnInit(EventArgs e)
		{
			if (Sitecore.Context.PageMode.IsPageEditor && !VirtualPreviewUtil.IsVirtualPreview(this.Page))
			{
				this.Page.ClientScript.RegisterClientScriptInclude(
					"sc_c_init", "/sitecore modules/Shell/Sitecore.Calendar/Scripts/Init.js");
			}

			if (this.Page != null && !Page.IsPostBack)
			{
        if (!Sitecore.Context.PageMode.IsPageEditor)
				{
					this.Page.ClientScript.RegisterClientScriptInclude(
						Page.GetType(), "sc_c_LoadScripts", "/sitecore modules/Shell/Sitecore.Calendar/Scripts/LoadScripts.js");
				}

			  this.Page.AddCssFile("/sitecore modules/shell/sitecore.calendar/Themes/Calendar.css");
        this.Page.AddCssFile("/sitecore modules/shell/sitecore.calendar/Themes/Colors.css");
        this.Page.AddCssFile(StaticSettings.Theme);
			}
		}

		protected override void Render(HtmlTextWriter output)
		{
			if (this.Page == null)
			{
				this.PreviewVirtualControl(output);
			}
			else
			{
				if (Sitecore.Context.PageMode.IsPageEditor && !VirtualPreviewUtil.IsVirtualPreview(this.Page))
				{
					var html = new StringBuilder();

					html.Append("<link href=\"/sitecore modules/shell/sitecore.calendar/Themes/Calendar.css\" rel=\"stylesheet\"/>");
					html.Append("<link href=\"/sitecore modules/shell/sitecore.calendar/Themes/Colors.css\" rel=\"stylesheet\"/>");
					html.Append(String.Format("<link href=\"{0}\" rel=\"stylesheet\"/>", StaticSettings.Theme));

					html.Append("<script src=\"/sitecore modules/Shell/Sitecore.Calendar/Scripts/LoadScripts.js\"></script>");

					output.Write(html);
				}
			}
		}

		protected override void DoRender(HtmlTextWriter output)
		{
			this.Render(output);
		}

		#region Private methods

		private void PreviewVirtualControl(TextWriter output)
		{
			var page =
					BuildManager.CreateInstanceFromVirtualPath(@"~/sitecore/shell/VirtualPreviewCalendar.aspx", typeof(Page)) as Page;

			if (page != null)
			{
				CalendarWebControl render = this.CreateControlInstance(page);

				if (render != null)
				{
					var _form = new HtmlForm { ID = Guid.NewGuid().ToString() };

					var LoadScriptGenericControl = new HtmlGenericControl("script");
					LoadScriptGenericControl.Attributes.Add("type", "text/javascript");
					LoadScriptGenericControl.Attributes.Add("src", "\"/sitecore modules/Shell/Sitecore.Calendar/Scripts/LoadScripts.js\"");

					_form.Controls.Add(CreateHtmlLink("/sitecore modules/shell/sitecore.calendar/Themes/Calendar.css"));
					_form.Controls.Add(CreateHtmlLink("/sitecore modules/shell/sitecore.calendar/Themes/Colors.css"));
					_form.Controls.Add(CreateHtmlLink(StaticSettings.Theme));
					_form.Controls.Add(LoadScriptGenericControl);
					_form.Controls.Add(render);

					page.Controls.Add(_form);

					page.EnableEventValidation = false;

					var writer = new HtmlTextWriter(new StringWriter());
					page.RenderControl(writer);

					string pageHtml = writer.InnerWriter.ToString();

					output.Write(HtmlParserUtil.RemoveFormTagById(pageHtml, _form.ID).Trim('\n', '\t', '\r'));
				}
			}
		}

		private bool IsExistCssInHeader(string cssLocation)
		{
			foreach (Control headerControl in this.Page.Header.Controls)
			{
				if (headerControl is HtmlLink)
				{
					var link = headerControl as HtmlLink;
					if (link.Href == cssLocation) return true;
				}
			}

			return false;
		}

		private static HtmlLink CreateHtmlLink(string url)
		{
			var link = new HtmlLink { Href = url };
			link.Attributes.Add("type", "text/css");
			link.Attributes.Add("rel", "stylesheet");

			return link;
		}

		#endregion
	}
}
