using System;

namespace Sitecore.Modules.EventCalendar.Utils
{
   [Flags]
   public enum DaysOfWeek
   {
      None = 0,
      Monday = 1,
      Tuesday = 2,
      Wednesday = 4,
      Thursday = 8,
      Friday = 16,
      Saturday = 32,
      Sunday = 64
   }
}