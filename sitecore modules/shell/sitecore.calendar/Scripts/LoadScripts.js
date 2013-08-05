function LoadScripts() {
  if ("undefined" == typeof $) // Check whether 'prototype.js' script has already been loaded
  {
    document.write('<script type="text/javascript" src="/sitecore/Shell/controls/lib/prototype/prototype.js"><\/script>');
  }

  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/Utils.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/ConfirmEx.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/DateUtils.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/EventPopups.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/Validator.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/Proxy.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/ViewMode.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/CalendarEvent.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/Managers/DayManager.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/Managers/MonthManager.js"><\/script>');
  document.write('<script type="text/javascript" src="/sitecore modules/Shell/Sitecore.Calendar/Scripts/Managers/WeekManager.js"><\/script>');
}

LoadScripts();