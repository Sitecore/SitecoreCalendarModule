using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class DeleteEventContext : CommandContext
   {
      #region Methods

      public DeleteEventContext(string eventID, bool deleteSeries)
      {
         Assert.ArgumentNotNullOrEmpty(eventID, eventID); 

         EventID = eventID;
         DeleteSeries = deleteSeries;
      }

      #endregion

      #region Properties

      public string EventID { get; set; }
      public bool DeleteSeries { get; set; }

      #endregion
   }
}