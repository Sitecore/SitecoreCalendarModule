using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Commands;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class DeleteEvent : Command
   {
      #region Methods

      public override void Execute(CommandContext context)
      {
         if (context is DeleteEventContext)
         {
            Execute((DeleteEventContext) context);
         }
      }

      protected virtual void Execute(DeleteEventContext context)
      {
         if (!string.IsNullOrEmpty(context.EventID))
         {
            Item item = StaticSettings.EventTargetDatabase.GetItem(ID.Parse(context.EventID));
            if (item == null)
            {
               Log.Error("Calendar Module: cannot find event", null);
               return;
            }

            ID schedule = null;
            bool parsed = ID.TryParse(item.Fields[Event.ScheduleIDField].Value, out schedule);

            Event.Delete(item);
            PublishUtil.Publishing(item, false, true);

            if (parsed && !schedule.IsNull)
            {
               CalendarActions.DeleteScheduler(new Schedule(schedule), context.DeleteSeries);
            }
         }
      }

      #endregion
   }
}