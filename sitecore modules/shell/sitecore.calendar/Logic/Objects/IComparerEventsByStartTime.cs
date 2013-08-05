using System;
using System.Collections.Generic;

namespace Sitecore.Modules.EventCalendar.Objects
{
   public class ComparerEventsByStartTime : IComparer<Event>
   {
      #region IComparer<Event> Members

      public int Compare(Event x, Event y)
      {
         if (x == null)
         {
            if (y == null)
            {
               return 0;
            }
            else
            {
               return -1;
            }
         }
         else
         {
            if (y == null)
            {
               return 1;
            }
            else
            {
               return TimeSpan.Parse(x.StartTime).CompareTo(TimeSpan.Parse(y.StartTime));
            }
         }
      }

      #endregion
   }
}