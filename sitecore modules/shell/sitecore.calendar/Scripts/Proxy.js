/*********************************************************
* Proxy: responsible for interoperability with server 
**********************************************************/

function Proxy()
{
   this._service  = Sitecore.Modules.EventCalendar.Services.WebService;
   this._calendar = null;  
   
   Sys.Net.WebRequestManager.add_invokingRequest(_startProgress);
   Sys.Net.WebRequestManager.add_completedRequest(_stopProgress);
   _progress = $('idProgressBar');
}

Proxy._instance = null;

Proxy.getInstance = function()
{
   if(Proxy._instance == null)
   {
      Proxy._instance = new Proxy();
   }
   
   return Proxy._instance;
}

Proxy.prototype.MoveEvent = function( eventID, newDate, startTime, endTime, OnCallBack)
{
   this._service.MoveEvent( eventID, newDate, startTime, endTime, OnCallBack);   
}

Proxy.prototype.UpdateTime = function(eventID, startTime, endTime, updateSeries, OnCallBack)
{
   this._service.UpdateTime(eventID, startTime, endTime, updateSeries, OnCallBack);
}

Proxy.prototype.CreateEvent = function( evnt, schedule, calendarID, siteSettings, viewSettings)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.CalendarID = calendarID;
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   
   //this.startProgress();
   this._service.CreateEvent( evnt, schedule, options, this.UpdateView);
}


Proxy.prototype.DeleteEvent = function(eventID, deleteSeries)
{
   //this.startProgress();
   this._service.DeleteEvent(eventID, deleteSeries, this.UpdateView);
}

Proxy.prototype.GetEventInfo = function(eventID, callbackFunction)
{
   this._service.GetEventInfo(eventID, callbackFunction);  
}

Proxy.prototype.GetScheduleInfo = function(scheduleID, callbackFunction)
{
   this._service.GetScheduleInfo(scheduleID, callbackFunction);
}

Proxy.prototype.GetCalendarsList = function(canwrite, siteSettings, viewSettings, callbackFunction)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   
   this._service.GetCalendarsList(canwrite, options, callbackFunction);
}


Proxy.prototype.GetCalendar = function(eventID, siteSettings, viewSettings, callbackFunction)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();   
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   
   this._service.GetCalendar( eventID, options, callbackFunction);
}


Proxy.prototype.SaveEventInfo = function(evnt, schedule, calendar, updateSeries, siteSettings, viewSettings)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.CalendarID = calendar;
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   
   this._service.SaveEventInfo(evnt, schedule, updateSeries, options, this.UpdateView);
}

Proxy.prototype.UpdateView = function()
{
   if ( _$('idGrid') != null)
   {      
       ViewMode.getInstance()._calendar = null;       
       ViewMode.getInstance().onUpdateCalendars();              
   } 
   else if( document.frames == null)
   {
      if((self.frames != null) && (self.frames[0] != null))
      {
         self.frames[0].window.location.reload(false);      
      }
      else
      {
         window.location.reload(false);   
      }
   }
   else
   {
      if( document.frames[0] == null)
      {
         window.location.reload(false);
      }
      else 
      {
         document.frames[0].window.location.reload(false);
      }
   }
}

Proxy.prototype.SwitchToDayView = function(idCalendar, date, siteSettings, viewSettings, hour, callBack)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.CalendarID = idCalendar;
   options.Date = date;
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   var tableGrid = _$(idCalendar).firstChild.lastChild.previousSibling;
     
   //this.startProgress();
   Proxy.getInstance()._service.GetViewHTML(                
                -1,
                tableGrid.getAttribute('startHour'),                
                "day", 
                tableGrid.getAttribute('mode'), 
                hour,
                options,
                callBack);
}

Proxy.prototype.SwitchToWeekView = function(idCalendar, date, siteSettings, viewSettings, hour, callBack)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.CalendarID = idCalendar;
   options.Date = date;
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   var tableGrid = _$(idCalendar).firstChild.lastChild.previousSibling;
      
   //this.startProgress();
   Proxy.getInstance()._service.GetViewHTML(
                -1,
                tableGrid.getAttribute('startHour'),                
                "week", 
                tableGrid.getAttribute('mode'), 
                hour, 
                options,              
                callBack);
}

Proxy.prototype.SwitchToMonthView = function(idCalendar, date, siteSettings, viewSettings, hours, callBack)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.CalendarID = idCalendar;
   options.Date = date;
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   
   var tableGrid = _$(idCalendar).firstChild.lastChild.previousSibling;
   
   //this.startProgress();
   Proxy.getInstance()._service.GetViewHTML(
                -1,
                tableGrid.getAttribute('startHour'),
                "month", 
                tableGrid.getAttribute('mode'), 
                hours,
                options,
                callBack);
}

Proxy.prototype.GetHTMLDayInMonthView = function(idCalendar, curdate, date, siteSettings, viewSettings, startHour, hours, callBack)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.CalendarID = idCalendar;
   options.Date = curdate;
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   
    Proxy.getInstance()._service.GetHTMLDayInMonthView(date, options, startHour, hours, callBack)
}

Proxy.prototype.GetHTMLMiniCalendar = function(num, date, siteSettings, viewSettings, callBack)
{
   var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
   options.Date = curdate;
   options.SiteSettingsPath = siteSettings;
   options.ControlSettingsPath = viewSettings;
   
    Proxy.getInstance()._service.GetHTMLMiniCalendar(num, options, callBack)
}

Proxy.prototype.IsRecurrent = function(eventID, callback)
{
   return this._service.IsRecurrent(eventID, callback);
}


var _progress;

_startProgress = function(executor, eventArgs)
{
   if(_progress == null)
   {
      return;
   }

   _progress.style.top = document.body.scrollTop || document.documentElement.scrollTop + 'px';

   _progress.style.display    = "block";
   _progress.style.visibility = "visible";
}

_stopProgress = function(executor, eventArgs)
{
   if(_progress == null)
   {
      return;
   }

   _progress.style.display    = "none";
   _progress.style.visibility = "hidden";
}
