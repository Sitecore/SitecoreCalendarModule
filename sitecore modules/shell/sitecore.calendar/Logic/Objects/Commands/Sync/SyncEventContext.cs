using System.Collections.Generic;

using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class SyncEventContext : CommandContext
   {
      #region Methods

      public SyncEventContext(Event pattern, IEnumerable<Event> events, EventList calendar, EventListManager mgr)
      {
         Pattern = pattern;
         Events = events;
         EventList = calendar;
         EventListManager = mgr;
      }

      #endregion

      #region Properties

      public Event Pattern { get; set; }
      public IEnumerable<Event> Events { get; set; }
      public EventList EventList { get; set; }
      public EventListManager EventListManager { get; set; }

      #endregion
   }
}