namespace Sitecore.Modules.EventCalendar.UI
{
  using System.IO;
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using Sitecore.Diagnostics;

  public static class PageExtensions
  {
    /// <summary>
    /// Adds the CSS file.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="cssFile">The CSS file.</param>
    public static void AddCssFile(this Page page, string cssFile)
    {
      Assert.ArgumentNotNull(page, "page");
      Assert.ArgumentNotNullOrEmpty(cssFile, "cssFile");

      HtmlLink link = new HtmlLink();
      link.Href = cssFile;

      link.Attributes.Add("rel", "stylesheet");
      link.Attributes.Add("type", "text/css");

      if (page.Header != null)
      {
        try
        {
          if (!IsExistCssInHeader(page, cssFile))
          {
            page.Header.Controls.Add(link);            
          }
        }
        catch 
        {
          AddToClientScriptManager(page, link);
        }
      }
      else
      {
        AddToClientScriptManager(page, link);
      }
    }

    /// <summary>
    /// Adds to client script manager.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="link">The link.</param>
    private static void AddToClientScriptManager(Page page, HtmlLink link)
    {
      HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(new StringBuilder()));
      link.RenderControl(writer);
      page.ClientScript.RegisterClientScriptBlock(typeof(Page),
        writer.InnerWriter.ToString(), writer.InnerWriter.ToString());
    }

    /// <summary>
    /// Determines whether [is exist CSS in header] [the specified page].
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="cssLocation">The CSS location.</param>
    /// <returns>
    /// <c>true</c> if [is exist CSS in header] [the specified page]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsExistCssInHeader(Page page, string cssLocation)
    {
      foreach (Control headerControl in page.Header.Controls)
      {
        if (headerControl is HtmlLink)
        {
          var link = headerControl as HtmlLink;
          if (link.Href == cssLocation)
          {
            return true;
          }
        }
      }

      return false;
    }

  }
}