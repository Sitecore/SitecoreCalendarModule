/******************************************************
* CalendarEvent: represents a single calendar event
******************************************************/
function CalendarEvent(calendar, element)
{
   this._calendar  = calendar;
   this._element   = Element.extend(element);
   this._startTime = element.getAttribute("startTime");
   this._endTime   = element.getAttribute("endTime");
   this._startHour   = element.getAttribute("startHour");   

     
   if( IsIE() == true) {

      this._element.onmouseover = this.eventController;
   }
   else
   {
      this._element.addEventListener("mouseover",  this.eventController, false); 
   }
   
   
   this._element._eventEntity = this;
   this._element._eventEntity.height = null;
   
   this.dragging = false;
   this.resizing = false;
   this.expand = false;
}


CalendarEvent.prototype.align = function()
{
   if(this._startTime == null || this._endTime == null)
   {
      return;
   }
   
   var _helper = TimeHelper.getInstance(this._calendar);

   this._element.style.position = "absolute";
   
   var top = _helper.timeToPosY(this._startTime);
   this._element.style.top    = top + "px";

   this._element.style.height = Math.abs(_helper.timeToPosY(this._endTime) - _helper.timeToPosY(this._startTime)) + "px";
}


CalendarEvent.prototype.fitToTimeBounds = function(phantom, updatecalendar, updateOnlyTime, updateSeries) {
    if (phantom != null) {
        this._element.style.top = phantom.offsetTop + "px";
    }

    var _helper = TimeHelper.getInstance(this._calendar);
    var _topY = this._element.offsetTop;

    var _bottomY = _topY + Element.extend(this._element).getDimensions().height;

    var startTime = _helper.posToTime(_topY);
    var endTime = _helper.posToTime(_bottomY);

    this._element.style.top = _helper.timeToPosY(startTime) + "px";
    this._element.style.height = _helper.timeToPosY(endTime) - _helper.timeToPosY(startTime) + "px";
    this._element.style.left = _helper.timeToPosX(this._element) + "px";

    this._startTime = startTime;
    this._endTime = endTime;

    this._element.setAttribute('startTime', this._startTime);
    this._element.setAttribute('endTime', this._endTime);

    if (!updatecalendar) {
        ViewMode.getInstance()._calendar = this._calendar;
    } else {
        ViewMode.getInstance()._calendar = null;
    }

    if (!updateOnlyTime) {
        Proxy.getInstance().MoveEvent(this.getID(),
                                  _helper.getDate(this._element),
                                  this._startTime,
                                  this._endTime,
                                  ViewMode.getInstance().onUpdateCalendars);
    } else {
        Proxy.getInstance().UpdateTime(this.getID(),
                                       this._element._eventEntity._startTime,
                                       this._element._eventEntity._endTime, updateSeries,
                                       ViewMode.getInstance().onUpdateCalendars);
    }
}


CalendarEvent.prototype.adjustBoundaries = function(event, calendar, updatecalendar, updateSeries) {
    event = event || window.event;
    var _helper = TimeHelper.getInstance(calendar);

    var clientY = AbsToRelY(event.clientY, WeekManager.getInstance(calendar.id)._element) + getTotalScrollTop(this._element);

    var top = _helper.timeToPosY(this._element._eventEntity._startTime);

    var bottom = top + this._element.getDimensions().height;

    if (bottom <= top) {
        this._element._eventEntity._endTime = _helper.indexToTime(_helper.getIndex(this._startTime) + 1)
    } else {
        this._element._eventEntity._endTime = _helper.posToTime(bottom, true);
    }

    this._element.style.height = _helper.timeToPosY(this._element._eventEntity._endTime) - _helper.timeToPosY(this._element._eventEntity._startTime) + "px";

    if (!updatecalendar) {
        ViewMode.getInstance()._calendar = this._calendar;
    } else {
        ViewMode.getInstance()._calendar = null;
    }

    Proxy.getInstance().UpdateTime(this.getID(),
                                   this._element._eventEntity._startTime,
                                   this._element._eventEntity._endTime,
                                   updateSeries,
                                   ViewMode.getInstance().onUpdateCalendars);
}


CalendarEvent.prototype.getStartTime = function()
{
   return this._startTime;   
}


CalendarEvent.prototype.getEndTime = function()
{
   return this._endTime;
}

CalendarEvent.prototype.getID = function()
{
   return this._element.getAttribute("eventID");
}


CalendarEvent.prototype.getElement = function()
{
   return this._element;
}


CalendarEvent.prototype.eventController = function (event)
{
   event = event || window.event;

   var oDIV = (event.target) ? event.target : event.srcElement;
   
   oDIV.onmouseover  = onEventOver;
   oDIV.onmouseout   = onEventLeave;
   oDIV.onmousedown  = onEventMouseDown;
   
   var oldZ = null;
   
   onEventOver(event);
   
   function onEventOver(event)
   {  
      event = event || window.event;
      oDIV = (event.target) ? event.target : event.srcElement;

      oldZ = oDIV.style.zIndex;
      oDIV.style.zIndex = 10000;   
   }

   function onEventLeave(event)
   {
      event = event || window.event;
      oDIV = (event.target) ? event.target : event.srcElement;

      oDIV.style.zIndex = oldZ || 0; 
      oldZ = null;   
   }

   function onEventMouseDown(event)
   {  
      event = event || window.event;
      
      var grid = GetGridForEvent(event);     
      
      if(oDIV.tagName == "IMG") 
      {
         return;
      }

      if(oDIV.tagName == "A")
      { 
         Event.stop(event);
         return; 
      }
      
      if(oDIV._eventEntity == null)
      {
         return;
      }
      
      var currentElement = (event.target) ? event.target : event.srcElement;

      if( currentElement.tagName == "IMG")
      {
         var link = oDIV.getAttribute('link');

         if( link != "")
         {
            window.location = link;
         }
         else
         {
            oDIV._eventEntity.expand   = true;
            oDIV._eventEntity.resizing = false;
            oDIV._eventEntity.dragging = false;
         }
         return;
      }

      if( oDIV.getAttribute('readonly') == "True")
      {
         return;
      }
      
      var clientY = AbsToRelY( event.clientY, oDIV) + getTotalScrollTop(oDIV);

      if( clientY > ( oDIV.cumulativeOffset().top + oDIV.getDimensions().height - 10) )
      {
         if (!oDIV._eventEntity.resizing) {
            oDIV._eventEntity.height = oDIV.getDimensions().height;
         }
         oDIV._eventEntity.resizing = true; 
         oDIV._eventEntity.dragging = false;   
      }
      else
      {   
         oDIV._eventEntity.resizing = false;
         oDIV._eventEntity.dragging = true;         
      }
   }
}
