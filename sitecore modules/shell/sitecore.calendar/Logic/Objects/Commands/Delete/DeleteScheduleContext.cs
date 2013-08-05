using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class DeleteScheduleContext : CommandContext
   {
      #region Methods

      public DeleteScheduleContext(Schedule schedule, bool deleteSeries)
      {
         Assert.ArgumentNotNull(schedule, "schedule");
         Schedule = schedule;
         DeleteSeries = deleteSeries;
      }

      #endregion

      #region Properties

      public Schedule Schedule { get; set; }
      public bool DeleteSeries { get; set; }

      #endregion
   }
}