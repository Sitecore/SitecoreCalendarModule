using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class SyncEvent : Command
   {
      #region

      public override void Execute(CommandContext context)
      {
         if (context is SyncEventContext)
         {
            Execute((SyncEventContext) context);
         }
      }

      protected virtual void Execute(SyncEventContext context)
      {
         foreach (Event ev in context.Events)
         {
            ev.Attributes = context.Pattern.Attributes;
            ev.Description = context.Pattern.Description;
            ev.EndTime = context.Pattern.EndTime;
            ev.Location = context.Pattern.Location;
            ev.Name = context.Pattern.Name;
            ev.ScheduleID = context.Pattern.ScheduleID;
            ev.StartTime = context.Pattern.StartTime;
            ev.Title = context.Pattern.Title;
           
            ev.Save();

            if (context.EventList.ID != context.EventListManager.GetCalendar(ev).ID)
            {
               context.EventList.MoveEvent(ev);
            }

            PublishUtil.Publishing(ev.GetTargetItem(), true, false);
         }
      }

      #endregion
   }
}