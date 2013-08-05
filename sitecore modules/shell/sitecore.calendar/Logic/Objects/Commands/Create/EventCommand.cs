using System;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public abstract class EventCommand : Command
   {
      #region Methods

      public override void Execute(CommandContext context)
      {
         if (context is EventCommandContext)
         {
            Execute((EventCommandContext)context);
         }
      }

      protected abstract void Execute(EventCommandContext context);

			protected static void AddEvent(DateTime date, EventCommandContext context)
			{
				if ((date > Utilities.StringToDate(context.Schedule.EndDate)) || date < Utilities.StringToDate(context.Schedule.StartDate))
				{
					return;
				}

				context.Event.StartDate = Utilities.NormalizeDate(date);
				context.Event.EndDate = Utilities.NormalizeDate(date);
				context.Event.ScheduleID = context.Schedule.ID;
				context.EventList.AddEvent(context.Event, context.Branch);
			}

      #endregion
   }
}