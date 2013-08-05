/******************************************************
* WeekManager: handles day tables
******************************************************/
function WeekManager() {
    this._dayManagersList = null;
    this._element = null;
}

WeekManager._instance = new Array();
WeekManager._current = null;

WeekManager.getInstance = function(idCalendar) {
    if (WeekManager._instance[idCalendar] == null) {
        WeekManager._instance[idCalendar] = new WeekManager();
    }

    return WeekManager._instance[idCalendar];
}

window.onresize = resize;
var gloabalwidth = 0;
function resize() {
    if (gloabalwidth != document.forms[0].offsetWidth) {
        for (var item in WeekManager._instance) {
            if (item != null && item != "[object]" && WeekManager.getInstance(item).Resize != null) {
                WeekManager.getInstance(item).Resize(item);
            }
        }
        gloabalwidth = document.forms[0].offsetWidth;
    }
}

WeekManager.prototype.UpdateResizeEventData = function(_element, evnt, calendar, event) {

    var grid = GetGridForEvent(event);
    if (_element.readAttribute('isrecur') == '1') {
        grid.onmousemove = null;
        grid.onmouseup = null;
        grid.onmouseleave = null;
        new ConfirmEx().show("Update", "",
                function(res, mode) {
                    if (res == 'yes') {
                        evnt.adjustBoundaries(event, calendar, true, mode == "all");
                        WeekManager.getInstance(calendar.id).Update(calendar.id);
                    } else {
                        _element.style.height = _element._eventEntity.height - 2 + "px";
                    }
                });
    }
    else {
        evnt.adjustBoundaries(event, calendar, false, false);
        WeekManager.getInstance(calendar.id).Update(calendar.id);
    }

    if (evnt != null) {
        evnt.resizing = false;
    }
    evnt = null;
}

WeekManager.prototype.Resize = function(idCalendar) {
  calendarElement = WeekManager.getInstance(idCalendar)._element;
  var calendar = _$(idCalendar);
  if (calendarElement != null && calendarElement.rows && calendarElement.rows.length > 0) {
    var divDays = calendarElement.rows[calendarElement.rows.length - 1].cells[0].firstChild;
    var helper = TimeHelper.getInstance(calendar);

    //    if (IsIE7()) {
    //      element.style.width = calendar.offsetWidth - 16 + "px";
    //    }

    var numHours = parseInt(calendarElement.getAttribute('numHours'), 10);
    var startHour = parseInt(calendarElement.getAttribute('startHour'), 10);
    divDays.style.height = 2 * numHours * parseInt(helper._height, 10) + numHours + 4 + "px";
    divDays.scrollTop = 2 * startHour * parseInt(helper._height, 10) + startHour + 2;

    var tdcell = document.getElementById('idemptycell' + idCalendar);

    if (IsIE7()) {
      if (tdcell != null) {
        divDays.firstChild.style.width = calendar.offsetWidth - 50 + "px";
        tdcell.nextSibling.style.width = tdcell.nextSibling.clientWidth - divDays.firstChild.rows[0].cells[0].clientWidth + 'px';
      }
      divDays.firstChild.style.width = divDays.clientWidth;
    }
  }
  WeekManager.getInstance(idCalendar).Update(idCalendar);
}

WeekManager.prototype.Init = function(idCalendar) {
    this._dayManagersList = new Array();
    this._element = _$(idCalendar).firstChild.lastChild.previousSibling;

    var tables = this._element.getElementsByTagName("TABLE");

    WeekManager.getInstance(_$(idCalendar)).Resize(idCalendar);


    var j = 0;
    for (var i = 0; i < tables.length; i++) {
        if (tables[i].getAttribute('DayTable') != null) {
            this._dayManagersList[j++] = new DayManager(_$(idCalendar), tables[i]);
        }
    }
}

WeekManager.prototype.Update = function(calendar) {
    var dayManager = WeekManager.getInstance(calendar)._dayManagersList;
    if (dayManager != null) {
        for (var i = 0; i < dayManager.length; i++) {
            var manager = dayManager[i];
            if (manager.wrap == null && manager._calendar != null &&
               (manager._calendar.firstChild.className == "weekGrid" ||
                manager._calendar.firstChild.className == "dayGrid")) {
                dayManager[i].AdjustOverlappingEvents();
            }
        }
    }
}

WeekManager.prototype.onResize = function(event) {
    event = event || window.event;

    var target = (event.target) ? event.target : event.srcElement;

    var grid = GetGridForEvent(event);
    if (grid == null) {
        return;
    }
    var calendar = grid.parentNode.parentNode;
    WeekManager.getInstance(calendar.id).Update(calendar.id);
}

WeekManager.prototype.eventController = function(event) {
    event = event || window.event;

    var target = Event.element(event);

    if (target != null) {
        if (target.tagName == "IMG") {
            return;
        }
    }

    var grid = GetGridForEvent(event);
    if (grid == null) {
        return;
    }
    var calendar = grid.parentNode.parentNode;

    if (target == null || (target._eventEntity == null && target.tagName == "TD" && target.className != "")) {
        if (WeekManager.getInstance(calendar.id)._element.getAttribute("readonly") != "True") {

            EventToolbar.getInstance(calendar.id).show(calendar.id,
                             AbsToRelX(event.clientX, WeekManager.getInstance(calendar.id)._element),
                             AbsToRelY(event.clientY, WeekManager.getInstance(calendar.id)._element) + getTotalScrollTop(WeekManager.getInstance(calendar.id)._element),
                             target.parentNode.rowIndex,
                             findAbsPosX(WeekManager.getInstance(calendar.id)._element));
        }
        return;
    }

    var evnt = target._eventEntity || target.parentNode._eventEntity;

    if (evnt == null || evnt.expand == null) {
        return
    }
    if (evnt.expand == true) {
        EventWizard.getInstance().show(evnt.getID(), false);
        evnt.expand = false;
        return;
    }

    this._element = grid;

    this._element.onmousemove = onMouseMove;
    this._element.onmouseup = onMouseUp;
    this._element.onmouseleave = onMouseLeave;

    var _element = evnt.getElement();

    var phantom = null;
    var phantom2 = null;
    var oldItem = _element;

    if (evnt.dragging == true) {
        phantom = createPhantom(_element);

        phantom.style.left = findAbsPosX(_element) - findAbsPosX(WeekManager.getInstance(calendar.id)._element);

        var eventGrid = GetEventGrid(this._element);

        if (IsIE8()) {
            phantom2 = phantom.cloneNode(true);
            phantom2.style.display = "block";
            eventGrid.appendChild(phantom2);
            phantom2.childNodes[1].nodeValue = '.';
            phantom2.addClassName("smalletPhantom");
        }
        eventGrid.appendChild(phantom);
        phantom.style.display = "block";
    }

    var _dragging = false;
    var _delta = AbsToRelY(event.clientY, WeekManager.getInstance(calendar.id)._element) + getTotalScrollTop(_element) - parseInt(_element.style.top, 10);

    function onMouseMove(event) {

        event = event || window.event;

        if (evnt == null) {
            return;
        }

        var grid = GetGridForEvent(event);
        if (grid == null) {
            return;
        }
        var calendar = grid.parentNode.parentNode;

        var clientY = AbsToRelY(event.clientY, _element) + getTotalScrollTop(_element);

        var _helper = TimeHelper.getInstance(calendar);


        if (evnt.resizing == true) {
            var newH = clientY - _element.cumulativeOffset().top;
            _element.style.height = (newH > _helper.getMinHeight() ? newH : _helper.getMinHeight()) + "px";

            if (Element.extend(_element).getDimensions().height + _element.cumulativeOffset().top > _helper.getBottomLimit()) {
                _element.style.height = _helper.getBottomLimit() - _element.cumulativeOffset().top + "px";
            }
            return;
        }

        if (evnt.dragging == true && phantom != null) {

            clientY = AbsToRelY(event.clientY, WeekManager.getInstance(calendar.id)._element) + getTotalScrollTop(_element);
            phantom.style.top = clientY - _delta + "px";
            phantom.style.display = "block";

            var bottomY = parseInt(phantom.style.top, 10) + parseInt(phantom.style.height, 10);
            if (clientY > _helper.getBottomLimit() + parseInt(phantom.style.height, 10) + 150) {
                evnt.dragging == false;
                onMouseLeave(event);

                evnt = null;

                grid.onmousemove = null;
                grid.onmouseup = null;
                grid.onmouseleave = null;
                grid.onmouseout = null;
                return;
                //phantom.style.top = _helper.getBottomLimit() - parseInt(phantom.style.height, 10) + "px";
            }

            if (parseInt(phantom.style.top, 10) < 0) {
                phantom.style.top = "0px";
            }

            var newPosX = AbsToRelX(Event.pointerX(event), WeekManager.getInstance(calendar.id)._element);

            if (IsIE() || IsIE7()) {
                newPosX -= 20;
            }

            if (newPosX > (parseInt(phantom.style.left, 10) + phantom.getDimensions().width)) {
                phantom.style.left = newPosX - phantom.offsetWidth + "px";
            }
            else if (newPosX < parseInt(phantom.style.left, 10)) {
                phantom.style.left = newPosX + "px";
            }

            phantom.style.left = newPosX;

            _dragging = true;
        }
    }

    function onMouseLeave(event) {
        if (phantom != null && phantom.style != null) {
            phantom.style.display = "none";
            phantom.remove();
        }

        if (phantom2 != null) {
            phantom2.remove();
        }

    }

    function onMouseUp(event) {
        event = event || window.event;

        if (evnt == null) {
            return;
        }

        var grid = GetGridForEvent(event);
        if (grid == null) {
            return;
        }

        var calendar = grid.parentNode.parentNode;

        if (evnt.resizing == true) {
            WeekManager.getInstance(calendar.id).UpdateResizeEventData(_element, evnt, calendar, event);
            return;
        }

        if (evnt.dragging == false && evnt.resizing == false) {
            return;
        }

        if (_dragging == false) // handling single/double clicks 
        {
            evnt.dragging = false;
            evnt.resizing = false;

            if (phantom != null) {
                phantom.remove();
            }

            if (phantom2 != null) {
                phantom2.remove();
            }
            phantom = null;
            phantom2 = null;

            return;
        }

        var _helper = TimeHelper.getInstance(calendar);

        var newParent = _helper.getDayTblFromX(AbsToRelX(event.clientX, WeekManager.getInstance(calendar.id)._element) - 20);

        if (newParent != null && newParent._DayEntity != null) {
            postDragAction(this, newParent, evnt._element.parentNode.offsetParent);
        }
    }

    function postDragAction(element, newParent, oldParent) {

        if (evnt == null) {
            return;
        }

        var grid = GetGridForEvent(event);
        if (grid == null) {
            return;
        }
        var calendar = grid.parentNode.parentNode;

        if (evnt.resizing == true) {
            WeekManager.getInstance(calendar.id).UpdateResizeEventData(_element, evnt, calendar, event);
            return;
        }

        if (_element.readAttribute('isrecur') == '1') {
            if (oldParent._DayEntity != newParent._DayEntity) {
                if (window.confirm("The recurring event will be removed from the series. Would you like to go on moving of this event?") == true) {
                    newParent._DayEntity.addEvent(evnt, oldItem);
                    evnt.fitToTimeBounds(phantom, true);
                    evnt._element.style.display = "none";
                }
            }
            else {
                grid.onmousemove = null;
                grid.onmouseup = null;
                grid.onmouseleave = null;
                new ConfirmEx().show("Update", "",
                    function(res, mode) {
                        if (res == 'yes') {
                            newParent._DayEntity.addEvent(evnt, oldItem);
                            evnt.fitToTimeBounds(phantom, true, true, mode == "all");
                            evnt._element.style.display = "none";
                        }
                    });
            }
        } else {
            newParent._DayEntity.addEvent(evnt, oldItem);
            evnt.fitToTimeBounds(phantom);
            newParent._DayEntity.update();

            WeekManager.getInstance(calendar.id).Update(calendar.id);
        }

        var deletePhantom = evnt.dragging;
        if (evnt != null) {
            evnt.dragging = false;
        }

        evnt = null;

        grid.onmousemove = null;
        grid.onmouseup = null;
        grid.onmouseleave = null;
        grid.onmouseout = null;

        if (phantom != null && deletePhantom) {
            phantom.remove();

            if (phantom2 != null) {
                phantom2.remove();
            }
        }
    }
}



