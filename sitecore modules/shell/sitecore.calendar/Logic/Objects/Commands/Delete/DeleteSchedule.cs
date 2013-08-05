using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class DeleteSchedule : Command
   {
      public override void Execute(CommandContext context)
      {
         if (context is DeleteScheduleContext)
         {
            Execute((DeleteScheduleContext) context);
         }
      }

      protected virtual void Execute(DeleteScheduleContext context)
      {
         if (!string.IsNullOrEmpty(context.Schedule.ID) && !(context.Schedule.ID == ID.Null.ToString()))
         {
            Item schedule = context.Schedule.GetTargetItem();

            if (schedule != null)
            {
               if (context.DeleteSeries)
               {
                  var links = Globals.LinkDatabase.GetReferrers(schedule);

                  Item source = null;
                  foreach (var link in links)
                  {
                     source = link.GetSourceItem();
                     source.Delete();

                     PublishUtil.Publishing(source, false, false);
                  }
               }

               if (Globals.LinkDatabase.GetReferrerCount(schedule) == 0)
               {
                  schedule.Delete();
                  PublishUtil.Publishing(schedule, false, false);
               }
            }
         }
      }
   }
}