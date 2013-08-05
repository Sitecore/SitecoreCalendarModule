using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class MoveEventTimeContext : CommandContext
   {
      #region Methods

      public MoveEventTimeContext(string eventID, string startTime, string endTime, bool updateSeries)
      {
         EventID = eventID;
         StartTime = startTime;
         EndTime = endTime;
         UpdateSeries = updateSeries;
         Assert.ArgumentNotNullOrEmpty(eventID, "eventID");
         Assert.IsTrue(ID.IsID(eventID), "eventID is not an ID");
      }

      #endregion

      #region Properties

      public string EventID { get; set; }
      public string StartTime { get; set; }
      public string EndTime { get; set; }
      public bool UpdateSeries { get; set; }

      #endregion
   }
}