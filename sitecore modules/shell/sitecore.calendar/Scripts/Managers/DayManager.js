/******************************************************
* DayManager: manages a table for a single day
******************************************************/
function DayManager(calendar, element)
{
   this._calendar = calendar;
   this._isDayViewMode = (calendar.firstChild.className == "dayGrid");
   this._tblElement = element;
   this._evElements = this._tblElement.getElementsByTagName("div");
   this._events = new Array();
   this.initEvents();
   this._tblElement._DayEntity = this;
}
   
DayManager.prototype.addEvent = function(evnt, oldItem)
{
   if (this._isDayViewMode) 
   {
       if (IsIE7())
       {
            this.getRow().replaceChild(evnt._element, oldItem);    
       }
       else
       {
            this.getRow().appendChild(evnt._element);    
       }
   }
   else
   {
       this.getRow().appendChild(evnt._element);    
   }
}

DayManager.prototype.getElement = function()
{
   return this._tblElement;
}

DayManager.prototype.initEvents = function()
{
   for(var i = 0; i< this._evElements.length; i++)
   {
      if(this._evElements[i].getAttribute("calEvent")==null)
      {
         continue;
      }
      
      this._events[i] = new CalendarEvent(this._calendar, this._evElements[i]);
      this._events[i].align();
   }
   this.AdjustOverlappingEvents();
}


DayManager.prototype.getRow = function()
{
   var row = null;

   if( IsIE() == true )
   {
      row = this._tblElement.cells[0]
   }
   else
   {
      row = this._tblElement.rows[0].cells[0];
   }
   
   return row;
}


DayManager.prototype.update = function()
{
   this._events = new Array();
   
   for(var i=0; i< this._evElements.length; i++)
   {
      if(this._evElements[i].getAttribute("calEvent")==null)
      {
         continue;
      }

      this._events[i] = this._evElements[i]._eventEntity;
   }
  
   this.AdjustOverlappingEvents();
}


DayManager.prototype.AdjustOverlappingEvents = function()
{  
   var _time = new Array();

   for(var i = 0; i < this._events.length; ++i)
   {

      var evnt = this._events[i];
      
      if(evnt == null)
      {
         continue;
      }
      
      var startIndex = getFormatTime(evnt._startTime);
      var endIndex = getFormatTime(evnt._endTime);
      
      var position = -1;
      var isPresent = _time[startIndex] == null;
      
      if (this._isDayViewMode)
      {
          position = FindColision(_time, startIndex, endIndex);      
          isPresent = position == -1;
      }
      
      if(isPresent)
      {
          var ar = new Array();
          ar[0]  = evnt;
         _time[startIndex] =  ar;
      }
      else
      {
         if (!this._isDayViewMode)
         {
            position = startIndex;
         }
         _time[position][_time[position].length] = evnt;
      }
   }
   
   var firstElem = null;
   for (firstElem in _time) {
        if(_time[firstElem] != null && _time[firstElem].wrap == null)	{
	   break;
	}	
   };
       
   if  (_time[firstElem] != null && _time[firstElem].wrap == null)
   {
       var widthLimit = 0;            
       var eventDIV = _time[firstElem][0].getElement();
       var table = eventDIV.offsetParent.offsetParent;
       widthLimit = (table.offsetWidth - table.rows[0].cells[0].offsetWidth) / (table.rows[0].cells.length - 1);             
       var xLimit  = findPosX(eventDIV.parentNode);      
       var space = xLimit + widthLimit;
       
       for(var y in _time)
       {       
          if(_time[y] == null || _time[y].wrap != null)
          {
             continue;
          }   
          
          var delta = 0;            
          var length = _time[y].length;                   

          var elementWidth = widthLimit / length;                  
          
	  if (_time[y].sort != null){
              _time[y].sort(SortItem);                 
	  }else{
              _time[y].sortBy(SortItem);                 
	  }
                 
          for( var j = 0; j < length; ++j)  
          {
             eventDIV = _time[y][j].getElement();        
             
             var newLeft = xLimit + delta; 
             
             eventDIV.style.left = newLeft + "px";
             
             var width = elementWidth;
             if (!this._isDayViewMode)
             {
               width = space - newLeft;
             }
             
             width = width < 0 ? 0 : width;
             
             eventDIV.style.width  = width + "px";

             eventDIV.style.zIndex = Math.abs(parseInt(eventDIV.style.top, 10)) * 10 + Math.abs(parseInt(eventDIV.style.left, 10));

             delta += elementWidth ;
          }
      }
   } 
}

function FindColision(_time, startIndex, endIndex)
{     

     for(var i  in _time)
     {
        if (_time[i] != null && _time[i].wrap == null)
        {
            for (j = 0; j < _time[i].length; ++j)
            {
                var evnt = _time[i][j];
                if (evnt != null && evnt.wrap == null)
                {              
                    var start = getFormatTime(evnt._startTime);
                    var end = getFormatTime(evnt._endTime);
                    
	           if (!(end <= startIndex || start >= endIndex)){

                        return getFormatTime(_time[i][0]._startTime);			
		   }		
                }
            }            
        }
     }
     
     return -1;
}


function SortItem(a, b)
{
    return a._startTime > b._startTime;
}