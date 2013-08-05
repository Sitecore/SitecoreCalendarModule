/******************************************************
* EventToolbar
******************************************************/
function EventToolbar(element)
{  
    this._calendar = null;
    this._element = null;
    this._startTime = null;
    this._endTime = null;
    this._date = null;
}

EventToolbar._instance = new Array();

EventToolbar.getInstance = function(idCalendar)
{
   if (EventToolbar._instance[idCalendar] == null)
   {
       EventToolbar._instance[idCalendar] = new EventToolbar();
   }
   return EventToolbar._instance[idCalendar];
}

EventToolbar.prototype.Init = function(idCalendar) {
    this._calendar = idCalendar;
    this._element = _$('idEventToolbar' + idCalendar);
    if (this._element != null) {
        EventToolbar.getInstance(idCalendar)._element._popupEntity = this;
    }
}

EventToolbar.prototype.show = function(idCalendar, x, y, rowindex, offsetParentX) {
    EventToolbar.getInstance(idCalendar).Init(idCalendar);
    if (EventToolbar.getInstance(idCalendar)._element.style.display == 'block') {
        return;
    }
    Proxy.getInstance().GetCalendarsList(true, _$(idCalendar).getAttribute('siteSettings'), _$(idCalendar).getAttribute('viewSettingPath'),
        function(list) {
            if (list != null && list.length > 0) {
                EventToolbar.getInstance().fillCalendarsList(list, idCalendar);
                var _helper = TimeHelper.getInstance(_$(idCalendar));

                var startTime = getFormatDate(addHours(_helper.indexToTime(rowindex), 0));
                var endTime = getFormatDate(addHours(_helper.indexToTime(rowindex), 1));

                EventToolbar.getInstance(idCalendar)._date = _helper.getDayTblFromX(x).getAttribute('date');

                if (rowindex == 46) {
                    startTime = "22:30";
                    endTime = "23:30";
                }

                EventToolbar.getInstance(idCalendar)._startTime = startTime;
                EventToolbar.getInstance(idCalendar)._endTime = endTime;

                var tmpTbl = _helper.getDayTblFromX(x);
                var coor = _helper.timeToPosY(startTime) - _helper._scroll.scrollTop;

                var bottom = _helper._scroll.parentNode.offsetHeight;
                if (coor < _helper._scroll.offsetTop) {
                    coor = 0;
                } else if (coor > bottom - _helper._scroll.parentNode.offsetTop - 45) {
                    coor = bottom - 45;
                }

                EventToolbar.getInstance(idCalendar)._element.style.top = coor + "px";

                coor = findAbsPosX(tmpTbl) - offsetParentX;
                if (coor > _helper._scroll.offsetWidth - 150) {
                    coor = _helper._scroll.offsetWidth - 150;
                }

                EventToolbar.getInstance(idCalendar)._element.style.left = coor + "px";
                EventToolbar.getInstance(idCalendar)._element.style.zIndex = 100000;
                EventToolbar.getInstance(idCalendar)._element.style.display = "block";

                if (EventWizard.getInstance()._element == null) {
                    _$('idExpandButton' + idCalendar).style.display = 'none';
                }

            }
        })
}

EventToolbar.prototype.fillCalendarsList = function(list, idCalendar)
{
   var calsList = _$('idCalendarsList' + idCalendar);   
   calsList.innerHTML = "";
   for(var i = 0; i < list.length - 1; i = i + 2)
   {
      var option = document.createElement("OPTION");
      option.value = list[i+1]; 
      option.text = list[i];
      if (calsList.options.add)
      {
         calsList.options.add(option);
      }
      else
      {
         calsList.add(option);
      }
   }
}

EventToolbar.prototype.dispatchMessage = function(idCalendar, message)
{
   if( message == "event:cancel" )
   {
      EventToolbar.getInstance(idCalendar)._element.style.display = 'none';
      return;
   }
   
   var evnt = new Sitecore.Modules.EventCalendar.Objects.Event(); 
   evnt.Title       =  Validator.getInstance().Sanitize(_$('idEventTitle' + idCalendar).value); 
   evnt.StartDate   =  EventToolbar.getInstance(idCalendar)._date;
   evnt.EndDate     =  evnt.StartDate;
   evnt.StartTime   =  getFormatDate(EventToolbar.getInstance(idCalendar)._startTime);
   evnt.EndTime     =  getFormatDate(EventToolbar.getInstance(idCalendar)._endTime);
 
   if( message == "event:create" && Validator.getInstance().IsEmpty(_$('idEventTitle' + idCalendar)))
   {
      EventToolbar.getInstance(idCalendar)._element.style.display = 'none';
      Proxy.getInstance().CreateEvent( evnt, null, _$('idCalendarsList' + idCalendar).value, 
                                       _$(idCalendar).getAttribute('siteSettings'), 
                                       _$(idCalendar).getAttribute('viewSettingPath'));
   }
   
   if( message == "event:expand" )
   { 
      EventToolbar.getInstance(idCalendar)._element.style.display = 'none';
      EventWizard.getInstance().CreateEvent(null, EventToolbar.getInstance(idCalendar)._date, evnt, 
                                            _$('idCalendarsList' + idCalendar).value,
                                            _$(idCalendar).getAttribute('siteSettings'), 
                                            _$(idCalendar).getAttribute('viewSettingPath'));  
   }
}

/******************************************************
* EventWizard
******************************************************/
function EventWizard() {
   this.init();

   if( this._element != null) {
      this._element._eventWizardEntity = this;
   }

   this._forceReadOnly = false;
}

EventWizard._instance = null;
EventWizard._schedule = null;

EventWizard.getInstance = function() {
   if(EventWizard._instance == null) {  
      EventWizard._instance = new EventWizard();
   }
   
   return EventWizard._instance;
}

EventWizard.prototype.init = function() {
   this._evManagerID = null;
   this._element = _$('idEventWizard');
   this._panel   = _$('idEventWizardPanel');
   this._modalBackground = _$('idModalBackground');
   this._eventID = _$('idEventID_hidden');
   this._siteSettings = null;
   this._viewSettings = null;
}

EventWizard.prototype.ExpandEvent = function(element, siteSettings, viewSettings) {  
   _$('idBtnDelete').style.visibility = '';
   var forceReadOnly = element.parentNode.getAttribute('readonly');
   this._siteSettings = siteSettings;
   this._viewSettings = viewSettings;
   this.show(element.getAttribute('eventID'), false, forceReadOnly);
}

EventWizard.prototype.CreateEvent = function(event, date, evnt, id, siteSettings, viewSettings) {
    var btnDel = _$('idBtnDelete');
    _$('idBtnDelete').style.visibility = "hidden";

    EventWizard.getInstance()._evManagerID = null;
    if (evnt != null && id != null) {
        EventWizard.getInstance()._evManagerID = id;
    }

    this._siteSettings = siteSettings;
    this._viewSettings = viewSettings;
    
    var context = {sender : this};
    var onCalsListLoadedNew = Function.createCallback(this.onCalsListLoadedNew, context);
    Proxy.getInstance().GetCalendarsList(true, siteSettings, viewSettings, onCalsListLoadedNew);

    this.setDefaults(date);
    if (evnt != null) {
        if (evnt.Title != null) {
            _$('idSubject').value = evnt.Title;
        }

        _$('idStartTime').value = evnt.StartTime;

        var i = evnt.StartTime.indexOf(':');

        if (evnt.StartTime == null || evnt.StartTime == "") {
            _$('idStartTime').value = "09:00";
            _$('idEndTime').value = "09:30";
        } else {
            _$('idStartTime').value = getFormatDate(evnt.StartTime);
            if (_$('idStartTime').value == "") {
                _$('idStartTime').value = "09:00";
            }
            _$('idEndTime').value = getFormatDate(evnt.EndTime);
        }

        var hours = parseInt(evnt.StartTime.substring(0, i), 10);
        if (hours > 23) {
            evnt.StartTime = "23:00";
            _$('idStartTime').value = evnt.StartTime;
            _$('idEndTime').value = "23:30"
        }
    }

    event = event || window.event;

    if (event != null) {
        if (event.stopPropagation) {
            event.stopPropagation();
        } else {
            event.cancelBubble = true;
        }
    }
}

EventWizard.prototype.Save = function() {
    if (_$('idBtnSave').disabled == true || _$('idBtnSave').disabled == "true") {
        return;
    }

    if (this.validate() != true) {
        return;
    }

    var evnt = new Sitecore.Modules.EventCalendar.Objects.Event();

    evnt.Title = _$('idSubject').value;
    evnt.Description = _$('idDescription').value;
    evnt.StartDate = _$('idStartDate').value;
    evnt.EndDate = _$('idEndDate').value;
    evnt.StartTime = _$('idStartTime').value;
    evnt.EndTime = _$('idEndTime').value;
    evnt.Location = _$('idLocation').value;

    var schedule = null;
    var updateSeries = false;

    if (_$('idEventID_hidden').value.length > 0) {
        evnt.ID = _$('idEventID_hidden').value;
    }

    if (EventWizard.getInstance().IsRecurrent() == true) {           
        
                
        if (_$('idScheduleID_hidden').value.length > 0) {

            schedule = eval('(' + _$('idScheduleID_hidden').value + ')');
            evnt.Schedule = schedule.ID;
                       

            var tempSchedule = Object.clone(schedule);
            EventWizard.getInstance().saveRecurrence(tempSchedule);
                
            if (Sys.Serialization.JavaScriptSerializer.serialize(tempSchedule) == _$('idScheduleID_hidden').value) {            
                
                var context = {siteSettings : this._siteSettings, viewSettings :  this._viewSettings};
                new ConfirmEx().show("Update", "'" + evnt.Title + "'", 
                    function(res, mode){
                        if (res == 'yes'){
                            EventWizard.getInstance().saveRecurrence(schedule);
                            Proxy.getInstance().SaveEventInfo(evnt, schedule, _$('idCalendar').value, mode == 'all',
                                                                context.siteSettings, context.viewSettings);
                                                                
                            EventWizard.getInstance().doBlink();
                            EventWizard.getInstance().Hide();
                        }
                    });
                return;
            }else{
                updateSeries = true;
                EventWizard.getInstance().saveRecurrence(schedule);
            }            
        }
        else {
            updateSeries = true;
            schedule = new Sitecore.Modules.EventCalendar.Objects.Schedule();
            EventWizard.getInstance().saveRecurrence(schedule);
        }
   }     

   if (evnt.ID != null){              
       Proxy.getInstance().SaveEventInfo(evnt, schedule, _$('idCalendar').value, updateSeries,
                                         this._siteSettings, this._viewSettings);
    }else{
       Proxy.getInstance().CreateEvent(evnt, schedule, _$('idCalendar').value,
                                       this._siteSettings, this._viewSettings);
    }
   EventWizard.getInstance().doBlink();
   EventWizard.getInstance().Hide();
}

EventWizard.prototype.getSchedule = function(schedule) {
    EventWizard._schedule = schedule;
}


EventWizard.prototype.Clear = function() {
   if( _$('idBtnClear').disabled == true || _$('idBtnClear').disabled == "true" ) {
      return;
   }

   if(window.confirm("Do you want to restore Event's fields?") == true) {
      this.doBlink();
      this.setDefaults();
   }   
}

EventWizard.prototype.DeleteEvent = function() {
   if( _$('idBtnDelete').disabled == true || _$('idBtnDelete').disabled == "true") {
      return;
   }

   if( EventWizard.getInstance().IsRecurrent() == true) {
   
      var context = {eventId : this._eventID.value};
      new ConfirmEx().show("Delete", "'" + _$('idSubject').value + "'", 
            function(res, mode){
                if (res == 'yes'){
                    Proxy.getInstance().DeleteEvent(context.eventId, mode=='all');
                    
                    EventWizard.getInstance().doBlink();
                    EventWizard.getInstance().Hide();
                }               
            });      
      return;
   }   

   if( window.confirm("Do you want to delete Event?") == true) {
      Proxy.getInstance().DeleteEvent( this._eventID.value, false);

      this.Hide();
   }
}

EventWizard.prototype.IsRecurrent = function() {
   if( _$('idScheduleID_hidden').value.length > 0) {
      return true;
   }

   return _$('idRecurOnce').checked != true;
}

EventWizard.prototype.Hide = function() { 
   this.init();

   this._element.style.display = 'none'; 
   this._intervalID = null;  
   this._modalBackground.style.display = 'none';
   _$('idScheduleID_hidden').value = '';
   _$('idCalendarID_hidden').value = '';
   
   this.setDefaults();
}

EventWizard.prototype.showAnimated = function() {   
   if( this._element.style.display == 'block') {
      return;
   }

   if( IsIE() != true) {
      this._panel.style.oldBackgroundColor = this._panel.style.backgroundColor;      
      this._element.style.display = 'block';
      this._element.style.top =  this._modalBackground.ownerDocument.documentElement.scrollTop + 50+ "px";
      this._element.style.left = (this._modalBackground.ownerDocument.documentElement.clientWidth - this._panel.offsetWidth) / 2 + "px";

      this.makeModal();
      return;
   }

   this._element.style.top = 70 + document.documentElement.scrollTop + "px";
   this._element.style.left = "0px";
   this._element.style.display = 'block';
   
   var centerX = ( document.documentElement.clientWidth - this._panel.offsetWidth) / 2;
   var centerY =   document.documentElement.scrollTop + 50;
  
   var func = "EventWizard.getInstance().animate(" + centerX + "," 
                                                   + centerY + ")";

   this._panel.style.oldBackgroundColor = this._panel.style.backgroundColor;
    
   this._intevalID = window.setInterval( func , 1); 
}


EventWizard.prototype.show = function(eventID, isNew, forceReadOnly) {
   this._eventID.value = eventID;

   if(forceReadOnly == "True") {
      this._forceReadOnly = forceReadOnly;
  }

  EventWizard.getInstance().switchTab('idDetails');

   if(isNew == true) {
      EventWizard.getInstance().showAnimated();
   } else {
      this.load();
   }
}

EventWizard.prototype.load = function() {    
   var context = {sender : this}
   var onEventLoadComplete = Function.createCallback(this.onEventLoadComplete, context);
   Proxy.getInstance().GetEventInfo(this._eventID.value, onEventLoadComplete);
}

EventWizard.prototype.onEventLoadComplete = function(evnt, p1, method, context) {

    _$('idSubject').value = evnt.Title;
    _$('idStartDate').value = evnt.StartDate;
    _$('idStartTime').value = evnt.StartTime;
    _$('idEndDate').value = evnt.EndDate = null ? evnt.StartDate : evnt.EndDate;
    _$('idEndTime').value = evnt.EndTime = null ? evnt.StartTime : evnt.EndTime;
    _$('idDescription').value = evnt.Description;
    _$('idLocation').value = evnt.Location;

    if (evnt.ReadOnly == true || EventWizard.getInstance()._forceReadOnly == "True") {
        _$('idBtnSave').style.visibility = 'hidden';
        _$('idBtnClear').style.visibility = 'hidden';
        _$('idBtnDelete').style.visibility = 'hidden';
        _$('idDescription').readOnly = "true";
        _$('idSubject').readOnly = "true";
        _$('idLocation').readOnly = "true";
        _$('idStartDate').readOnly = "true";
        _$('idEndDate').readOnly = "true";
        _$('idEndTime').disabled = "true";
        _$('idStartTime').disabled = "true";
        _$('idCalendar').disabled = "true";
        _$('idImgCalPopupStart').disabled = "true";
        _$('idImgCalPopupEnd').disabled = "true";
    }


    if (evnt.IsRecurrentEvent == true) {
        var onScheduleLoadComplete = Function.createCallback(context.sender.onScheduleLoadComplete, context);
        Proxy.getInstance().GetScheduleInfo(evnt.ScheduleID, onScheduleLoadComplete);
    } else {
        _$('idRecurOnce').checked = true;
    }

    var onCalsListLoadedExpand = Function.createCallback(context.sender.onCalsListLoadedExpand, context);
    Proxy.getInstance().GetCalendarsList(!evnt.ReadOnly, context.sender._siteSettings, context.sender._viewSettings,
                                         onCalsListLoadedExpand);
}

EventWizard.prototype.onCalsListLoadedExpand = function(list, p1, method, context) {  
   context.sender.fillCalendarsList(list);

   var onCalendarLoadComplete = Function.createCallback(context.sender.onCalendarLoadComplete, context);
   Proxy.getInstance().GetCalendar( _$('idEventID_hidden').value, 
                                    context.sender._siteSettings, context.sender._viewSettings,
                                    onCalendarLoadComplete);
}

EventWizard.prototype.onCalendarLoadComplete = function(calendar, p1, method, context) { 
   if( _$('idCalendar') == null) {
      return;
   }

   if ( _$('idCalendar').options.length > 0) {
      _$('idCalendar').value = calendar.IDKey;
   } 
   
   _$('idCalendarID_hidden').value = calendar.IDKey;      

   context.sender.showAnimated();
}

EventWizard.prototype.onScheduleLoadComplete = function(schedule, p1, method, context) {
   if( schedule == null) {
       return;
   }

   _$('idScheduleID_hidden').value = Sys.Serialization.JavaScriptSerializer.serialize(schedule); 

   context.sender.loadRecurrence(schedule);
}

EventWizard.prototype.onCalsListLoadedNew = function(list, p1, method, context) {
   if( list != null && list.length > 0) {
      context.sender.fillCalendarsList(list);
      context.sender.show("",true);
   }
}

EventWizard.prototype.fillCalendarsList = function(list) {
   _$('idCalendar').innerHTML = "";
   for(var i = 0; i < list.length - 1; i = i + 2) {
      var option = document.createElement("OPTION");
      option.value = list[i+1];
      option.text = list[i];
      
      if (_$('idCalendar').options.add) {
         _$('idCalendar').options.add(option);
      } else {
         _$('idCalendar').add(option);
      }
   }

   if (EventWizard.getInstance()._evManagerID != null) {
       _$('idCalendar').value = EventWizard.getInstance()._evManagerID;
       _$('idCalendarID_hidden').value = EventWizard.getInstance()._evManagerID;
       EventWizard.getInstance()._evManagerID = null;
   }    
}

EventWizard.prototype.animate = function(limitX, limitY) {
   var elY = parseInt(this._element.style.top, 10);
   var elX = parseInt(this._element.style.left, 10);
   
   var step = 30;
   
   if( elX < limitX) {
      this._element.style.left = elX + step;
   } else {
      window.clearInterval( this._intevalID );
      this.makeModal();
      this._intervalID = null;
      this._panel.style.backgroundColor = this._panel.style.oldBackgroundColor;
   }
}


EventWizard.prototype.makeModal = function() {
   if( this._modalBackground != null) {
      this._modalBackground.style.display = 'block'; 

      var width = 0;
      var height = 0;
      if( this._modalBackground.ownerDocument != null) {
         if(this._modalBackground.ownerDocument.clientWidth) {
            width = this._modalBackground.ownerDocument.clientWidth + "px";
         } else {
            width = this._modalBackground.ownerDocument.documentElement.clientWidth + "px";
         }
      } else {
         width = this._modalBackground.document.documentElement.clientWidth + "px";
      }

      this._modalBackground.style.width = width;
      this._modalBackground.style.height = document.documentElement.scrollHeight + "px";
   }
}


EventWizard.prototype.doBlink = function() {
   this._panel.style.oldBackgroundColor = this._panel.style.backgroundColor;
   this._panel.style.backgroundColor = "#ffe7bb";
   this._intervalID = window.setTimeout("EventWizard.getInstance().blinkEffect()",150);
}


EventWizard.prototype.blinkEffect = function() {
   window.clearTimeout( this._intevalID );
   this._intervalID = null;
   this._panel.style.backgroundColor = this._panel.style.oldBackgroundColor;
}


EventWizard.prototype.showCalendar = function(control) {
  var dateCtrl = eval("_$('" + control + "')");

  var url = '/sitecore%20modules/shell/sitecore.calendar/controls/calendarpopup.aspx';

  var date = dateCtrl.value;

  var queryString = '?controlID=' + control + '&date=' + date;

  var features;
  if (IsIE7()) {
    features = 'help:no;dialogWidth:175px;dialogHeight:180px;status:no;scrolling:no;resizable:no;center:yes';
  } else {
    features = 'help:no; dialogWidth:175px;dialogHeight:180px; status: no;scrolling:no; resizable:no;center:yes';
  }

  if (window.showModalDialog) {
    window.showModalDialog(url + queryString, self, features);
  } else {
    var left = (screen.availWidth - 150) / 2 + 'px';
    var top = (screen.availHeight - 50) / 2 + 'px';
        window.open(url + queryString, 'Date_Picker', 'width=165,height=165,top=' + top + ',left=' + left +
                        'toolbar=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no,modal=yes');
  }
}


EventWizard.prototype.switchTab = function(active) {
   _$('idReoccur').className='eventWizardTab'; 
   _$('idDetails').className='eventWizardTab';
   
   eval( '_$("' + active +'").className = "eventWizardTabActive"');
   
   switch(active) {
      case 'idDetails':
         _$('idDetailsTable').style.display = 'block';
         _$('idReoccurTable').style.display = 'none';
         break;
      case 'idOptions':
         _$('idDetailsTable').style.display = 'none';
         _$('idReoccurTable').style.display = 'none';
         break;
      case 'idReoccur':
         _$('idDetailsTable').style.display = 'none';
         _$('idReoccurTable').style.display = 'block';
         break;
      default:
         break;
   }
}


EventWizard.prototype.onClickRecur = function(element) {
   switch(element.id) {
      case 'idRecurOnce':
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'none';
         break;
      case 'idRecurDaily':
         _$('idDayRecur').style.display   = 'block';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'none';
         break;
      case 'idRecurWeekly':
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'block';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'none';
         break;      
      case 'idRecurMonthly':
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'block';
         _$('idYearRecur').style.display  = 'none';
         break;
      case 'idRecurYearly':
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'block';
         break;
      default:
         break;
   }
}


EventWizard.prototype.setDefaults = function(date) {
   _$('idSubject').value      = "New Event";
   _$('idLocation').value     = "";

   if(date != null && date.length > 0) {
      _$('idStartDate').value = date;
      _$('idEndDate').value   = date;  
   } else {  
      _$('idStartDate').value = getTodayDate();
      _$('idEndDate').value   = getTodayDate();
   }
 
   _$('idStartTime').value    = "09:00";   
   
   _$('idEndTime').value      = "09:30";
   _$('idDescription').value  = "";
   
   _$('idWeekRecurNum').value     = "1";
   _$('idDayRecurNum').value      = "1";
   _$('idDayOfWeekOrdinal').value = "0";
   _$('idPartOfWeek').value       = "1";
   _$('idMonthRecurWeekDayOrdinal').value  = "1";
   _$('idYearOccurMonth').value            = "1";
   _$('idYearOccurDay').value              = "1";
   
   _$('idRecurOnce').checked        = "checked";
   _$('idDayRecur').style.display   = 'none';
   _$('idWeekRecur').style.display  = 'none';
   _$('idMonthRecur').style.display = 'none';
   _$('idYearRecur').style.display  = 'none';

   _$('idBtnSave').style.visibility   = '';
   _$('idBtnClear').style.visibility  = '';
   _$('idDescription').readOnly = '';
   _$('idSubject').readOnly     = "";
   _$('idLocation').readOnly    = "";
   _$('idStartDate').readOnly   = "";   
   _$('idEndDate').readOnly     = "";   
   _$('idEndTime').disabled   = "";
   _$('idStartTime').disabled = "";
   _$('idCalendar').disabled  = "";
   _$('idImgCalPopupStart').disabled = "";
   _$('idImgCalPopupEnd').disabled   = "";

   EventWizard.getInstance()._forceReadOnly = "False";
}


EventWizard.prototype.validate = function() {  
   return this.validateDetails() == true && this.validateRecurrence() == true;   
}


EventWizard.prototype.validateDetails = function() {
   var validator = Validator.getInstance();

   if( validator.IsEmpty(_$('idSubject')) != true ||
       validator.IsEmpty(_$('idStartDate')) != true || 
       validator.IsEmpty(_$('idEndDate')) != true ) {
      return false;
   }

   if( validator.IsValidDate(_$('idStartDate')) != true || 
       validator.IsValidDate(_$('idEndDate')) != true) {
      return false;
   }
   
   if( validator.validateDatesRange( _$('idStartDate'), _$('idEndDate')) != true) {
      return false;
   }
   
   if( validator.validateTimeRange( _$('idStartDate'), _$('idStartTime'), _$('idEndDate'), _$('idEndTime')) != true) {
      return false;
   }

   if(validator.validateFieldLength( _$('idSubject'), 200) != true) {
      return false;
   }

   if(validator.validateFieldLength( _$('idLocation'), 150) != true) {
      return false;
   }
   
   return true;
}


EventWizard.prototype.validateRecurrence = function() {
   var validator = Validator.getInstance();

   if( _$('idRecurOnce').checked == true) {
      return true;
   }
   
   if( _$('idRecurDaily').checked == true) {
      if(validator.validate( _$('idDayRecurNum'), /^([1-9]|[1-9][0-9])$/, "Recurrence: Invalid format or 0!") != true) {      
         EventWizard.getInstance().switchTab('idReoccur');
         return false;
      }

      return true;
   }
   
   if( _$('idRecurWeekly').checked == true) {
       if (validator.validate(_$('idWeekRecurNum'), /^([1-9]|[1-9][0-9])$/, "Recurrence: Invalid format or 0!") != true) {
      
         EventWizard.getInstance().switchTab('idReoccur');
         return false;
      }
   
      if( _$('idCheckMonday').checked != true    && _$('idCheckTuesday').checked != true &&
          _$('idCheckWednesday').checked != true && _$('idCheckThursday').checked != true &&
          _$('idCheckFriday').checked != true    && _$('idCheckSunday').checked != true &&
          _$('idCheckSaturday').checked != true) {
          
         alert('At least one day of week should be selected, e.g. Monday');
         EventWizard.getInstance().switchTab('idReoccur');
         return false;
      }
      
      return true;
   }

   if( _$('idRecurMonthly').checked == true) {
       if (!validator.validate(_$('idMonthRecurWeekDayOrdinal'), /^([1-9]|[1-9][0-9])$/, "Invalid format or 0!")) {
       
           EventWizard.getInstance().switchTab('idReoccur');
           return false;
      }
   
      return true;
   }
   
   if( _$('idRecurYearly').checked == true) {
       if (!validator.validate(_$('idYearOccurDay'), /^([1-9]|[1-9][0-9])$/, "Invalid format or 0!") ||
           !validator.isValidDayNumber(_$('idYearOccurMonth').selectedIndex + 1,
                                       _$('idYearOccurDay').value,
                                       _$('idYearOccurDay'), 
                                       "The day number is incorrect.")) {
         
         EventWizard.getInstance().switchTab('idReoccur');
         return false;
      }

      return true;
   }

   return true;
}

EventWizard.prototype.loadRecurrence = function(schedule) {
   if(schedule == null) {
      return;
   }

   switch(schedule.Recurrence) {
      case Recurrence.Daily:
         _$('idDayRecur').style.display   = 'block';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'none';

         _$('idDayRecurNum').value = schedule.Frequency;
         _$('idRecurDaily').checked = true;

         break;
      case Recurrence.Weekly:
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'block';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'none';

         _$('idWeekRecurNum').value = schedule.Frequency;
         _$('idRecurWeekly').checked = true;

         EventWizard.getInstance().loadDaysOfWeek(schedule);

         break;
      case Recurrence.Monthly:
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'block';
         _$('idYearRecur').style.display  = 'none';
        
         _$('idRecurMonthly').checked = true;

         EventWizard.getInstance().loadSequence(schedule);

         break;
      case Recurrence.Yearly:
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'block';

         _$('idRecurYearly').checked = true;

         _$('idYearOccurMonth').value = schedule.Month;
         _$('idYearOccurDay').value = schedule.Frequency;

         break;
      default:
         _$('idDayRecur').style.display   = 'none';
         _$('idWeekRecur').style.display  = 'none';
         _$('idMonthRecur').style.display = 'none';
         _$('idYearRecur').style.display  = 'none';
         _$('idYearOccurDay').value = schedule.Frequency ;
         _$('idRecurOnce').checked = true;
         break;
   } 
}

EventWizard.prototype.saveRecurrence = function(schedule) {
   if(schedule == null) {
      return;
   }
   
   if( _$('idRecurOnce').checked == true) {
      schedule.Recurrence = Recurrence.None;
      return;
   }
   
   if( _$('idRecurDaily').checked == true) {
      schedule.Recurrence = Recurrence.Daily;
      schedule.Frequency = parseInt(_$('idDayRecurNum').value, 10);
      return;
   }
     
   if( _$('idRecurWeekly').checked == true) {
      schedule.Recurrence = Recurrence.Weekly;
      schedule.Frequency = parseInt(_$('idWeekRecurNum').value, 10);

      EventWizard.getInstance().saveDaysOfWeek(schedule);
     
      return;
   }
   
   if( _$('idRecurMonthly').checked == true) {
      schedule.Recurrence = Recurrence.Monthly;      
      EventWizard.getInstance().saveSequence(schedule);

      return;
   }
   
   if( _$('idRecurYearly').checked == true) {
      schedule.Recurrence = Recurrence.Yearly;    
      schedule.Month = parseInt(_$('idYearOccurMonth').value, 10);
      schedule.Frequency = parseInt(_$('idYearOccurDay').value, 10);

      return;
   }
}

EventWizard.prototype.loadDaysOfWeek = function(schedule) {
   if(schedule == null) {
      return;
   }

   if((schedule.DaysOfWeek & DaysOfWeek.Monday) == DaysOfWeek.Monday) {
      _$('idCheckMonday').checked = true;
  }  
   
   if( (schedule.DaysOfWeek & DaysOfWeek.Tuesday) == DaysOfWeek.Tuesday) {
      _$('idCheckTuesday').checked = true;
   }

   if( (schedule.DaysOfWeek & DaysOfWeek.Wednesday) == DaysOfWeek.Wednesday) {
      _$('idCheckWednesday').checked = true;
   }

   if( (schedule.DaysOfWeek & DaysOfWeek.Thursday) == DaysOfWeek.Thursday) {
      _$('idCheckThursday').checked = true;
   }   

   if( (schedule.DaysOfWeek & DaysOfWeek.Friday) == DaysOfWeek.Friday) {
      _$('idCheckFriday').checked = true;
   }   

   if( (schedule.DaysOfWeek & DaysOfWeek.Saturday) == DaysOfWeek.Saturday) {
      _$('idCheckSaturday').checked = true;
   }     

   if( (schedule.DaysOfWeek & DaysOfWeek.Sunday) == DaysOfWeek.Sunday) {
      _$('idCheckSunday').checked = true;
   }        
}

EventWizard.prototype.saveDaysOfWeek = function(schedule) {
   if(schedule == null) {
      return;
   }

   schedule.DaysOfWeek = 0;

   if( _$('idCheckMonday').checked == true) {
      schedule.DaysOfWeek |= DaysOfWeek.Monday;
   }  
   
   if( _$('idCheckTuesday').checked == true) {
      schedule.DaysOfWeek |= DaysOfWeek.Tuesday;
   }

   if( _$('idCheckWednesday').checked == true) {
      schedule.DaysOfWeek |= DaysOfWeek.Wednesday;
   }

   if( _$('idCheckThursday').checked == true) {
      schedule.DaysOfWeek |= DaysOfWeek.Thursday;
   }   

   if( _$('idCheckFriday').checked == true) {
      schedule.DaysOfWeek |= DaysOfWeek.Friday;
   }   

   if( _$('idCheckSaturday').checked == true) {
      schedule.DaysOfWeek |= DaysOfWeek.Saturday;
   }     

   if( _$('idCheckSunday').checked == true) {
      schedule.DaysOfWeek |= DaysOfWeek.Sunday;
   }        
}

EventWizard.prototype.loadSequence = function(schedule) {
   if( schedule == null) {
      return;
   }

   _$('idDayOfWeekOrdinal').value = schedule.Sequence;

   _$('idPartOfWeek').value = schedule.DaysOfWeek;

   _$('idMonthRecurWeekDayOrdinal').value = schedule.Frequency;
}

EventWizard.prototype.saveSequence = function(schedule) {
   if (schedule == null) {
      return;
   }

   schedule.Sequence = parseInt(_$('idDayOfWeekOrdinal').value, 10);
   
   schedule.DaysOfWeek = parseInt(_$('idPartOfWeek').value, 10);

   schedule.Frequency = parseInt(_$('idMonthRecurWeekDayOrdinal').value, 10);
}