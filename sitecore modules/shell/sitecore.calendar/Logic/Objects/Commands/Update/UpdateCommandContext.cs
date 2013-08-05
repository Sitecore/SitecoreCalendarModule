using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Logic.Utils;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class UpdateCommandContext : EventCommandContext
   {
      #region Methods

      public UpdateCommandContext(Event evt, Schedule schedule, Options options, bool updateSeries)
         : base(evt, schedule, options, null)
      {
         UpdateSeries = updateSeries;
      }

      #endregion

      #region Properties

      public bool UpdateSeries { get; set; }

      #endregion
   }
}