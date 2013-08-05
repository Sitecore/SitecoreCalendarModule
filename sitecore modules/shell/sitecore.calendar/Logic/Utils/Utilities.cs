using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Sites;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Utils
{
   public static class Utilities
   {
      private static Calendar threadCalendar
      {
         get
         {
            return DateTimeFormatInfo.CurrentInfo.Calendar;
         }
      }

      public static string ShellDomainName
      {
         get
         {
            return SiteContextFactory.GetSiteContext("shell").Domain.Name;
         }
      }

      public static string ProperCase(string originalString)
      {
         string strPrev = String.Empty;
         string strProper = String.Empty;

         if (originalString != null && originalString != String.Empty && originalString.Length > 1)
         {
            strProper = originalString.Substring(0, 1).ToUpper();
            originalString = originalString.Substring(1).ToLower();

            for (int i = 0; i < originalString.Length; i++)
            {
               if (i > 1)
               {
                  strPrev = originalString.Substring(i - 1, 1);
               }
               if (strPrev.Equals(" ") || strPrev.Equals("\t") || strPrev.Equals("\n") || strPrev.Equals("."))
               {
                  strProper += originalString.Substring(i, 1).ToUpper();
               }
               else
               {
                  strProper += originalString.Substring(i, 1);
               }
            }
         }

         return strProper;
      }

      public static string Sanitize(string original)
      {
         string results = String.Empty;

         if (original == null || original.Length == 0)
         {
            throw new Exception("Parameter \"original\" was null or empty");
         }

         char[] chars = original.Trim(' ').ToCharArray();

         for (int i = 0; i < chars.Length; i++)
         {
            if ((chars[i] >= 'A' && chars[i] <= 'Z') || (chars[i] >= 'a' && chars[i] <= 'z') ||
                (chars[i] >= '0' && chars[i] <= '9') || chars[i] == ' ' || chars[i] == '-')
            {
               results += chars[i].ToString();
            }
         }

         return results;
      }

      /// <summary>
      /// Get a List of Sitecore items collected from a Sitecore root item
      /// </summary>
      /// <param name="rootItem">The root Item to collect items from</param>
      /// <param name="template">Defines which Template items should be created from</param>
      /// <returns>List with items</returns>
      public static List<Item> GetChildItemsRecursive(Item rootItem, TemplateItem template)
      {
         Assert.ArgumentNotNull(rootItem, "rootItem");
         Assert.ArgumentNotNull(template, "rootItem");

         var items = new List<Item>();

         foreach (Item item in rootItem.Children)
         {
            if (item.TemplateID == template.ID)
            {
               items.Add(item);
            }

            if (item.HasChildren)
            {
               items.AddRange(GetChildItemsRecursive(item, template));
            }
         }

         return items;
      }

      public static DateTime FirstCalendarDay(DateTime visibleDate)
      {
         int delta = threadCalendar.GetDayOfWeek(visibleDate) - DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;

         if (delta < 0)
         {
            delta += 7;
         }

         return threadCalendar.AddDays(visibleDate, -delta);
      }

      public static DateTime FirstMonthDay(DateTime date)
      {
         return new DateTime(date.Year, date.Month, 1);
      }

      public static DateTime GetTime(string time)
      {
         int hours = 0;
         int minutes = 0;

         var parts = time.Split(':');

         int.TryParse(parts[0], out hours);

         if (parts.Length > 1)
         {
            int.TryParse(parts[1], out minutes);
         }

         return new DateTime(DateTime.Now.Year, 1, 1, hours, minutes, 0); 
      }

      public static int NumberOfVisibleWeeks(DateTime visibleDate, DateTime startDate)
      {
         Assert.IsTrue(visibleDate >= startDate, "Visible Date should be later than Start Date");

         TimeSpan span = visibleDate - startDate;
         int numOfDays = threadCalendar.GetDaysInMonth(visibleDate.Year, visibleDate.Month);

         return (int) Math.Ceiling((numOfDays + span.Days)/7.0);
      }

      public static string NormalizeDate(DateTime date)
      {
         string month = date.Month < 10 ? "0" + date.Month : date.Month.ToString();
         string day = date.Day < 10 ? "0" + date.Day : date.Day.ToString();

         return string.Format("{0}/{1}/{2}", date.Year, month, day);
      }

      public static string NormalizeDate(string year, string month, string day)
      {
         DateTime date = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));

         return NormalizeDate(date);
      }

      public static string NormalizeTime(string time)
      {
         string[] arrTime = time.Split(':');
         if (arrTime.Length < 2)
         {
            return "00:00";
         }
         string hh = arrTime[0].Length == 1 ? "0" + arrTime[0] : arrTime[0];
         string mm = arrTime[1].Length == 1 ? "0" + arrTime[1] : arrTime[1];

         return string.Format("{0}:{1}", hh, mm);
      }

      public static Item GetCalendarByEvent(Item ev)
      {
         if (ValidateEventStructure(ev) != true)
         {
            return null;
         }

         return ev.Parent.Parent.Parent.Parent;
      }

      public static bool ValidateEventStructure(Item ev)
      {
         if (ev == null)
         {
            return false;
         }

         Regex dayValidator = new Regex("^([0-3][0-9])$");
         Item day = ev.Parent;
         if (day == null || !dayValidator.IsMatch(day.Name))
         {
            return false;
         }

         Regex monthValidator = new Regex("^([0-1][0-9])$");
         Item month = day.Parent;
         if (month == null || !monthValidator.IsMatch(month.Name))
         {
            return false;
         }

         Item year = month.Parent;
         Regex yearValidator = new Regex("^([0-9][0-9][0-9][0-9])$");
         if (year == null || !yearValidator.IsMatch(year.Name))
         {
            return false;
         }

         Item calendar = year.Parent;
         if (calendar == null)
         {
            return false;
         }

         return true;
      }

      public static DateTime GetDateForEvent(Item ev)
      {
         if (ValidateEventStructure(ev) != true)
         {
            return DateTime.Today;
         }
         int day = int.Parse(ev.Parent.Name);
         int month = int.Parse(ev.Parent.Parent.Name);
         int year = int.Parse(ev.Parent.Parent.Parent.Name);

         return new DateTime(year, month, day);
      }

      public static string GetActualContentPath(Item ev)
      {
         if (ValidateEventStructure(ev) != true)
         {
            return string.Empty;
         }

         return GetCalendarByEvent(ev).Paths.FullPath + "/" + GetActualDatePath(ev);
      }

      public static string GetActualDatePath(Item ev)
      {
         DateTime date = DateUtil.IsoDateToDateTime(ev.Fields[Event.StartDateField].Value);

         return NormalizeDate(date);
      }

      public static string GetDatePath(Item ev)
      {
         string day = ev.Parent.Name;
         string month = ev.Parent.Parent.Name;
         string year = ev.Parent.Parent.Parent.Name;

         return NormalizeDate(year, month, day);
      }

      public static bool IsDateMappedToStruct(Item ev)
      {
         if (ValidateEventStructure(ev) != true)
         {
            return false;
         }

         return GetDatePath(ev) == GetActualDatePath(ev);
      }

      public static string IsoDateToNormal(string isoDate)
      {
         DateTime date = DateUtil.IsoDateToDateTime(isoDate);
         return NormalizeDate(date);
      }

      public static DateTime IsoStringToDate(string date)
      {
         return StringToDate(IsoDateToNormal(date));
      }

      public static DateTime StringToDate(string date)
      {
         string[] arrDate = date.Split('/');

         if (arrDate == null)
         {
            return DateTime.Today;
         }

         int year;
         if (int.TryParse(arrDate[0], out year) != true)
         {
            return DateTime.Today;
         }

         int month;
         if (int.TryParse(arrDate[1], out month) != true)
         {
            return DateTime.Today;
         }

         int day;
         if (int.TryParse(arrDate[2], out day) != true)
         {
            return DateTime.Today;
         }

         return new DateTime(year, month, day);
      }

      public static string StringToIsoDate(string date)
      {
         return DateUtil.ToIsoDate(StringToDate(date));
      }

      public static void SetCurrentCulture(string langauge)
      {
         Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(langauge);
      }

      public static string GetDefaultEndTime(string startTime)
      {
         string[] arrTime = startTime.Split(':');
         DateTime dtStart = new DateTime(2006, 10, 10, int.Parse(arrTime[0]), int.Parse(arrTime[1]), 0);
         DateTime dtEnd = dtStart.AddHours(StaticSettings.HoursSpan);
         dtEnd = dtEnd.AddMinutes(StaticSettings.MinutesSpan);

         string endTime;
         if (dtStart.Hour > dtEnd.Hour)
         {
            endTime = "24:00";
         }
         else
         {
            endTime = NormalizeTime(string.Format("{0}:{1}", dtEnd.Hour, dtEnd.Minute));
         }
         return endTime;
      }

      public static bool IsIE()
      {
         return HttpContext.Current.Request.Browser.Type.ToLower().Contains("ie");
      }

      public static string GetFriendlyURL(Item item)
      {
         if (item == null)
         {
            return string.Empty;
         }

         string path = item.Paths.ContentPath;

         int idx = path.IndexOf(StaticSettings.SitecoreRoot, 0, StringComparison.CurrentCultureIgnoreCase);
         if (idx != -1)
         {
            path =
               path.Substring(idx + StaticSettings.SitecoreRoot.Length,
                              path.Length - (idx + StaticSettings.SitecoreRoot.Length));
         }

         return path + ".aspx";
      }

      public static Item CreateItemPath(Item parent, string itemName, BranchItem branch)
      {
         Item child = parent.Axes.GetChild(itemName);
         if (child == null)
         {
            return parent.Add(itemName, branch);
         }
         return child;
      }

      public static Item CreateDatePath(Item parent, string datePath)
      {
         string[] folders = datePath.Split(StaticSettings.DateSeparator);
         if (folders.Length == 3)
         {
            parent = CreateItemPath(parent, folders[0], StaticSettings.YearBranch);
            parent = CreateItemPath(parent, folders[1], StaticSettings.MonthBranch);
            parent = CreateItemPath(parent, folders[2], StaticSettings.DayBranch);
         }
         else
         {
            parent =
               StaticSettings.EventTargetDatabase.Database.CreateItemPath(parent.Paths.ContentPath + "/" + datePath);
         }
         return parent;
      }

      public static Item[] GetEventListForView(Item settingsItem, string siteSettings)
      {
         ModuleSettings moduleSettings = new ModuleSettings(siteSettings);

         if (MainUtil.GetBool(settingsItem[StandardViewSettings.ShowAllCalendarsField], false))
         {
            return new Item[1] {moduleSettings.CalendarEventsRoot};
         }

         MultilistField calendarListFld = settingsItem.Fields[StandardViewSettings.CalendarListField];
         return calendarListFld.GetItems();
      }

      public static bool IsTemplateSettingsEqual(Item settings, Item settingsSite, string settingsField)
      {
         return settings.TemplateID == StaticSettings.ContextDatabase.GetItem(settingsSite[settingsField]).TemplateID;
      }

      public static ItemList GetChildrenUnsorted(Item item)
      {
         return item.Database.Engines.DataEngine.GetChildren(item);
      }
   }
}