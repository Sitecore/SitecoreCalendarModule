using System;
using System.Text;
using System.Web;
using System.Web.UI;

using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.UI
{
   public partial class CalendarPopup : Page
   {
      private DateTime baseDate = new DateTime(0x7d0, 1, 1);
      private DateTime selDate;

      protected void Page_Load(object sender, EventArgs e)
      {
				 if ((HttpContext.Current.Request.UserLanguages != null) &&
						 (!String.IsNullOrEmpty(HttpContext.Current.Request.UserLanguages[0])))
				 {
					 Utilities.SetCurrentCulture(HttpContext.Current.Request.UserLanguages[0]);
				 }

         string date = Request.QueryString["date"];

         try
         {
            selDate = Utilities.StringToDate(date);
         }
         catch (Exception)
         {
            selDate = DateTime.Today;
         }

         idCalendarPopup.SelectedDate = selDate;
         idCalendarPopup.VisibleDate = selDate;

         if (IsPostBack)
         {
            if (!string.IsNullOrEmpty(Request.Form["__EVENTARGUMENT"]) && Request.Form["__EVENTARGUMENT"][0] != 'V')
            {
               int days = idCalendarPopup.SelectedDate.Subtract(baseDate).Days;
               if (days == int.Parse(Request.Form["__EVENTARGUMENT"]))
               {
                  CalendarPopup_SelectionChanged(this, null);
               }
            }
         }
      }

      protected void CalendarPopup_SelectionChanged(object sender, EventArgs e)
      {
         StringBuilder sb = new StringBuilder();

         sb.Append("<script>");

         if (Utilities.IsIE())
         {
            sb.Append("if (window.dialogArguments) { window.opener = window.dialogArguments;");
         }

         string month = idCalendarPopup.SelectedDate.Month < 10
                           ? "0" + idCalendarPopup.SelectedDate.Month : idCalendarPopup.SelectedDate.Month.ToString();

         string day = idCalendarPopup.SelectedDate.Day < 10
                         ? "0" + idCalendarPopup.SelectedDate.Day : idCalendarPopup.SelectedDate.Day.ToString();

         sb.AppendFormat("window.opener.document.getElementById('{0}').value='{1}';", Request.QueryString["controlID"],
                         string.Format("{0}/{1}/{2}", idCalendarPopup.SelectedDate.Year, month, day));

         sb.Append("self.close();");

         if (Utilities.IsIE())
         {
            sb.Append("}");
         }

         sb.Append("</script>");

         ClientScript.RegisterClientScriptBlock(sb.GetType(), "mainScript", sb.ToString());
      }
   }
}