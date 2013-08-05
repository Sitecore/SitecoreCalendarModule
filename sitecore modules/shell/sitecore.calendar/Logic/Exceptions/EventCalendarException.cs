using System;

using Sitecore.Diagnostics;

namespace Sitecore.Modules.EventCalendar.Exceptions
{
   /// <summary>
   /// Event Calendar Exceptions
   /// </summary>
   public class EventCalendarException : ApplicationException
   {
      /// <summary>
      /// Throws a exception while logging an error in the Sitecore error log
      /// </summary>
      /// <param name="message">A custom message for the exception</param>
      public EventCalendarException(string message)
         : base(message)
      {
         Log.Error(String.Concat("Sitecore.Modules.EventCalendar Exception: ", message), this);
      }
   }
}