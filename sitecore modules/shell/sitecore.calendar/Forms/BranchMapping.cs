using System.Collections.Generic;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Core;

namespace Sitecore.Modules.EventCalendar.Forms
{
    public class BranchMapping
    {
        #region Fields

        private static Dictionary<ID, ID> mapping = new Dictionary<ID, ID>();

        #endregion

        #region Methods

        static BranchMapping()
        {
            mapping.Add(CalendarIDs.ModuleSettingsTemplate, CalendarIDs.ModuleSettingsBranch);
            mapping.Add(CalendarIDs.AgendaViewTemplate, CalendarIDs.AgendaViewBranch);
            mapping.Add(CalendarIDs.CalendarSelectorTemplate, CalendarIDs.CalendarSelectorBranch);
            mapping.Add(CalendarIDs.DateSelectorTemplate, CalendarIDs.DateSelectorBranch);
            mapping.Add(CalendarIDs.DayViewTemplate, CalendarIDs.DayViewBranch);
            mapping.Add(CalendarIDs.MonthViewTemplate, CalendarIDs.MonthViewBranch);
            mapping.Add(CalendarIDs.WeekViewTemplate, CalendarIDs.WeekViewBranch);
            mapping.Add(CalendarIDs.MiniCalendarTemplate, CalendarIDs.MiniCalendarBranch);
            mapping.Add(CalendarIDs.ProgressBarTemplate, CalendarIDs.ProgressBarBranch);
        }

        public static ID GetBrunch(Item item)
        {
            ID id;
            if ((item.BranchId == ID.Null) && mapping.TryGetValue(item.TemplateID, out id))
            {
                return id;
            }
            return item.BranchId;
        }

        #endregion
    }
}