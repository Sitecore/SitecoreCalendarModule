using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class MoveEventContext : CommandContext
   {
      #region Methods

      public MoveEventContext(string eventID, string newDate)
      {
         Assert.ArgumentNotNullOrEmpty(eventID, "eventID");
         Assert.ArgumentNotNullOrEmpty(newDate, "newDate");
         Assert.IsTrue(ID.IsID(eventID), "eventID is not an ID");

         EventID = eventID;
         NewDate = newDate;
      }

      #endregion

      #region Properties

      public string EventID { get; set; }
      public string NewDate { get; set; }

      #endregion
   }
}