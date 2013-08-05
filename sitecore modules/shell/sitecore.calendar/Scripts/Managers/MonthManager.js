/*************************************************
* Draggable: represents draggable copy of event
**************************************************/
function Draggable()
{
   this.Init(null, null);
}

Draggable.prototype.Dispose = function()
{
   this.element = null;
   this.phantom = null;
}

Draggable.prototype.Init = function(element, phantom)
{
   this.element = element;
   this.phantom = phantom;
}

Draggable.prototype.IsDragging = function()
{
   return this.phantom != null;
}

/*************************************************
* MonthManager: handles days grid
**************************************************/
function MonthManager()
{
   this._draggable = null;
   this._element   = null;   
   this._sourceDay = null; 
   this._targetDay = null; 
}

MonthManager._instance = new Array();
MonthManager._current  = null;

MonthManager.getInstance = function (idCalendar) {
    if (MonthManager._instance[idCalendar] == null) {
        MonthManager._instance[idCalendar] = new MonthManager();
    }

    $$('idEventWizard').each(Element.hide);

    return MonthManager._instance[idCalendar];
}

MonthManager.prototype.Init = function(idCalendar)
{
   this._draggable = new Draggable();
   this._element   =  _$(idCalendar).firstChild.lastChild.previousSibling;
   
   this._sourceDay = null; 
   this._targetDay = null; 
   
   if( IsIE() == true)
   {
      this._element.onmouseover  = MonthManager.getInstance(_$(idCalendar).id).onMouseOver;
      this._element.onmouseleave = MonthManager.getInstance(_$(idCalendar).id).onMouseLeave;
      this._element.onmousedown  = MonthManager.getInstance(_$(idCalendar).id).onMouseDown; 
      this._element.onmousemove  = MonthManager.getInstance(_$(idCalendar).id).onMouseMove;
      this._element.onmouseup    = MonthManager.getInstance(_$(idCalendar).id).onMouseUp;
   }
   else
   {  
      this._element.addEventListener("mouseup",    this.onMouseUp, true); 
      this._element.addEventListener("mousedown",  this.onMouseDown, true); 
      this._element.addEventListener("mouseout",   this.onMouseLeave, true); 
      this._element.addEventListener("mousemove",  this.onMouseMove, false); 
      this._element.addEventListener("mouseover",  this.onMouseOver, false); 
   }
}

MonthManager.prototype.CreateEvent = function(event, date, siteSettings, viewSettings)
{  
   EventWizard.getInstance().CreateEvent(event, date, null, null, siteSettings, viewSettings);
}

MonthManager.prototype.Redirect = function(element)
{
   var link = element.parentNode.getAttribute('link');  
   if( link != "")
   {
      window.location = link;
   }
}

MonthManager.prototype.onMouseOver = function(event)
{
   return;
//   event = event || window.event;

//   var oDIV = (event.target) ? event.target : event.srcElement;  
//   if( oDIV.tagName != "DIV" || 
//       oDIV.getAttribute("container") == null )
//   {
//      return; 
//   }

//   if(MonthManager.getInstance()._draggable.IsDragging() == true)
//   {
//      oDIV.className = "dayCellMonthHOVER";
//   }
//   else
//   {
//      oDIV.style.overflowY = "auto";
//   }
//      
//   if( oDIV.onmouseout == null)
//   { 
//      oDIV.onmouseout = onDayCellMouseOut;
//   }
}
 

MonthManager.prototype.onMouseLeave = function(event)
{  
    event = event || window.event;
    var grid = GetGridForEvent(event);    
    if (grid == null)
    {
        return;
    }
    var calendar = grid.parentNode.parentNode;
       
    if(MonthManager.getInstance(calendar.id)._draggable == null || 
       MonthManager.getInstance(calendar.id)._draggable.IsDragging() == false)
    {
      return;
    }   

    var oldParent = MonthManager.getInstance(calendar.id)._draggable.phantom.parentNode;
    if( oldParent != null )
    {
      if( IsIE() != true )
      {
         var target = event.relatedTarget;
         if( target == null ||
           ( target.tagName != "HTML" && target.tagName != "BODY" &&
             target.tagName != "TBODY" ))
         {
            return;
         }
      }

      oldParent.removeChild(MonthManager.getInstance(calendar.id)._draggable.phantom);
    }   

    MonthManager.getInstance(calendar.id)._draggable.Dispose();
}

MonthManager.prototype.onMouseDown = function(event)
{
    event = event || window.event; 
    var grid = GetGridForEvent(event);    
    if (grid == null)
    {
        return;
    }
    var calendar = grid.parentNode.parentNode;
    
    if (MonthManager.getInstance(calendar.id)._draggable != null)
    {
        MonthManager.getInstance(calendar.id)._draggable.Dispose();
    }

    if(event.button > 1)
    {
       return;
    }

    var oDIV = (event.target) ? event.target : event.srcElement;

    if( oDIV.tagName != "DIV" || oDIV.getAttribute('eventID') == null )
    {
     return;
    }

    if( oDIV.getAttribute("readonly") == "True" )
    {
     return;
    }

    var phantom = createPhantom(oDIV);

    grid.rows[0].cells[0].appendChild(phantom);       
    MonthManager.getInstance(calendar.id)._draggable.Init(oDIV, phantom);

    Event.stop(event);
}

MonthManager.prototype.onMouseMove = function(event)
{
    event = event || window.event;
    var grid = GetGridForEvent(event);    
    if (grid == null)
    {
        return;
    }
    var calendar = grid.parentNode.parentNode;
    
    if(MonthManager.getInstance(calendar.id)._draggable == null ||
        MonthManager.getInstance(calendar.id)._draggable.IsDragging() == false)
    {
        return;
    }

    var oTable = MonthManager.getInstance(calendar.id)._element;
    var deltaOffset = -100;

    var relX =  event.clientX;
    var relY =  event.clientY;

    if (MonthManager.getInstance(calendar.id)._draggable.phantom == null){
        alert('null');
    }

    MonthManager.getInstance(calendar.id)._draggable.phantom.style.display = "block";
    MonthManager.getInstance(calendar.id)._draggable.phantom.style.top     = AbsToRelY(event.clientY, oTable) + "px"; 
    MonthManager.getInstance(calendar.id)._draggable.phantom.style.left    = AbsToRelX(event.clientX, oTable) + getTotalScrollLeft( MonthManager.getInstance(calendar.id)._draggable.phantom) + "px";
}

MonthManager.prototype.onMouseUp = function(event)
{   
    event = event || window.event;
    var grid = GetGridForEvent(event); 
    if (grid == null)
    {
        return;
    }       
    var calendar = grid.parentNode.parentNode;
    
    if( MonthManager.getInstance(calendar.id)._draggable == null || 
        MonthManager.getInstance(calendar.id)._draggable.element == null)
    {
        return;
    }
    
    var oldParent = MonthManager.getInstance(calendar.id)._draggable.element.parentNode;
    
    if( oldParent == null || 
        oldParent.getAttribute("container") == null )
    {     
        MonthManager.getInstance(calendar.id)._draggable.phantom.parentNode.removeChild(
                                MonthManager.getInstance(calendar.id)._draggable.phantom);
        return;
    }

    var container = (event.target) ? event.target : event.srcElement; 

    if( container.tagName != "TD" )
    {
         if( container.parentNode == null && 
             container.parentNode.tagNode != "TD")
         {
             oldParent.removeChild(MonthManager.getInstance(calendar.id)._draggable.phantom);   
             return;
         }
         else
         {
            container = container.parentNode;
         }
    }

    if( container.getAttribute("container") == null )
    {
         if( container.parentNode != null && 
             container.parentNode.tagName == "TD" &&
             container.parentNode.getAttribute("container") != null)
         {
            container = container.parentNode;
         }
         else
         {
            var phantom = MonthManager.getInstance(calendar.id)._draggable.phantom;
            if( phantom!= null || phantom.parentNode != null)
            {
               phantom.parentNode.removeChild(phantom);
            }
            return;
         }
    }

    if (oldParent == container){
            var phantom = MonthManager.getInstance(calendar.id)._draggable.phantom;
            if( phantom!= null || phantom.parentNode != null)
            {
               phantom.parentNode.removeChild(phantom);
            }
            return;
    }

    grid.rows[0].cells[0].removeChild(MonthManager.getInstance(calendar.id)._draggable.phantom);

    MonthManager.getInstance(calendar.id)._sourceDay = oldParent;            
    MonthManager.getInstance(calendar.id)._targetDay = container;      
        
    MonthManager._current = calendar.id;

    var element = MonthManager.getInstance(calendar.id)._draggable.element;    
    if (MonthManager.getInstance(calendar.id)._draggable.element.readAttribute('isrecur') == '1'){
       if(window.confirm("The event will be removed from the series. Would you like to go on moving of the recurring event?") == true) {
           Proxy.getInstance().MoveEvent( element.getAttribute("eventID"),
            container.getAttribute("date"), null, null, MonthManager.getInstance(calendar.id).onUpdateCellSource);
       }       
    }
    else
    {
        Proxy.getInstance().MoveEvent( element.getAttribute("eventID"),
         container.getAttribute("date"), null, null, MonthManager.getInstance(calendar.id).onUpdateCellSource);
    }   
              
    MonthManager.getInstance(calendar.id)._draggable.Dispose();

    Event.stop(event);
}

MonthManager.prototype.onUpdateCellSource = function()   
{
    if (MonthManager.getInstance(MonthManager._current)._sourceDay != null)
    {
       var table = _$(MonthManager._current).firstChild.lastChild.previousSibling;
       Proxy.prototype.GetHTMLDayInMonthView(
                    MonthManager._current,
                    table.getAttribute('date'),
                    MonthManager.getInstance(MonthManager._current)._sourceDay.getAttribute("date"), 
                    _$(MonthManager._current).getAttribute('siteSettings'),
                    _$(MonthManager._current).getAttribute('viewSettingPath'),
                    table.getAttribute('startHour'),
                    table.getAttribute('numHours'),
                    function(html)
                    {
                        if (MonthManager.getInstance(MonthManager._current)._sourceDay != null)
                        {
                            MonthManager.getInstance(MonthManager._current)._sourceDay.innerHTML = html;
                            MonthManager.getInstance(MonthManager._current)._sourceDay = null;
                        }
                        
                        if (MonthManager.getInstance(MonthManager._current)._targetDay != null)
                        {
                            Proxy.prototype.GetHTMLDayInMonthView(
                                MonthManager._current,        
                                table.getAttribute('date'),                            
                                MonthManager.getInstance(MonthManager._current)._targetDay.getAttribute("date"), 
                                _$(MonthManager._current).getAttribute('siteSettings'),    
                                _$(MonthManager._current).getAttribute('viewSettingPath'),     
                                table.getAttribute('startHour'),
                                table.getAttribute('numHours'),                                                                   
                                function(html)
                                {
                                    if (MonthManager.getInstance(MonthManager._current)._targetDay != null)
                                    {
                                        MonthManager.getInstance(MonthManager._current)._targetDay.innerHTML = html;
                                        MonthManager.getInstance(MonthManager._current)._targetDay = null;
                                        ViewMode.getInstance()._calendar = _$(MonthManager._current);
                                        ViewMode.getInstance().onUpdateCalendars();
                                    }
                                });              
                        }
                    });
    }
}

function onDayCellMouseOut(event)
{
    event = event || window.event;
    var oDIV = (event.target) ? event.target : event.srcElement; 

    if( oDIV.tagName == "DIV" && 
      oDIV.getAttribute("container") != null )
    {
     if(oDIV.getAttribute("today") && (oDIV.getAttribute("today")=="true"))
     {
        oDIV.className = "todayCellMonth";
     }
     else
     {
        oDIV.className ="dayCellMonth";
     }

     oDIV.style.overflowY = "hidden";
    }
}
   