/***********************************************
* TimeHelper: maps coordinates to time cells
*
************************************************/
function TimeHelper(calendar) {

    var tableGrid = calendar.firstChild.lastChild.previousSibling;
    this._tables = $$('#' + calendar.id + ' table[dayCellTable]');

    this._timeRuller = $("timeRullerTbl" + calendar.id);
    this._tds = $$('#' + this._tables[0].id + " td");

    this._topY = findPosY(this._tds[0].style.top);
    this._height = this._tds[this._tds.length - 1].offsetHeight;

    this._bottomY = this._tables[0].cumulativeOffset().top + this._tables[0].getDimensions().height;
    var tableRows = tableGrid.rows;
    this._scroll = tableRows[tableRows.length - 1].cells[0].firstChild;
    this._offsetHours = tableRows.length == 1 ? 0 : -1;
}

TimeHelper._instance = new Array();

TimeHelper.getInstance = function(calendar) {
    if (TimeHelper._instance[calendar.id] == null ||
      TimeHelper._instance[calendar.id]._scroll.parentNode == null ||
      !(IsIE7() || IsIE())) {
        TimeHelper._instance[calendar.id] = new TimeHelper(calendar);
    }

    return new TimeHelper(calendar);
}

TimeHelper.Clear = function(calendar) {
    TimeHelper._instance = new Array();
}

TimeHelper.prototype.ReInit = function(calendar) {
    TimeHelper._instance[calendar.id] = new TimeHelper(calendar.id);
}


TimeHelper.prototype.getBottomLimit = function() {
    return this._bottomY;
}

TimeHelper.prototype.posToTime = function(y, includeHeight) {
    var i = 0;
    do {
        ++i;
    } while (this._tds.length > i && (this._tds[i].offsetTop + this._tds[i].height) <= y)

    var time = this.indexToTime(includeHeight ? i : i - 1);

    if (time == "24:00" || i >= this._tds.length) {
        return "23:30";
    }
    return time;
}

TimeHelper.prototype.indexToTime = function(y) {
    var idx = y / 2 | 0;
    if (idx >= 24) {
        idx = 23;
    }

    var m = y % 2 > 0 ? ":30" : ":00";


    return idx + m;
}

TimeHelper.prototype.timeToPosY = function(time, includeHeight) {
    var idx = this.getIndex(time);

    var top = this._tds[idx].offsetTop;

    if (includeHeight) {
        return top + this._tds[idx].getDimensions().height;
    }

    return top;
}

TimeHelper.prototype.timeToPosX = function(parentNode) {
    return findPosX(parentNode);
}

TimeHelper.prototype.getDate = function(eventElement) {
    var parent = eventElement.parentNode;

    while (parent != null) {
        if (parent.getAttribute("DayTable") != null) {
            return parent.getAttribute("date");
        }

        parent = parent.parentNode;
    }

    return null;
}


TimeHelper.prototype.getMinHeight = function(parentNode) {
    return this._tds[0].offsetHeight;
}


TimeHelper.prototype.getIndex = function(time) {
    var i = time.indexOf(':');

    var minutes = time.substring(i + 1, time.length);

    var minIndex = 0;
    if (minutes.length > 1) {
        if (minutes >= 30) {
            minIndex = 1;
        }
    }
    var hours = time.substring(0, i);

    if (hours.indexOf("0") == 0 && hours != "0") {
        hours = time.substring(1, 2);
    }

    return hours * 2 + minIndex;
}


TimeHelper.prototype.getDayTblFromX = function(x) {
    var rulerTblWidth = this._timeRuller == null ? 0 : this._timeRuller.getDimensions().width;
    var dayTblWidth = this._tables[0].getDimensions().width;

    var idx = Math.ceil((x - rulerTblWidth) / dayTblWidth);

    if (idx > 7) {
        return this._tables[6];
    }
    else if (idx <= 0) {
        return this._tables[0];
    }

    return this._tables[idx - 1];
}

function createPhantom(element) {
    var phantom = element.cloneNode(true);

    phantom.id = element.id + "_phanton";
    phantom.style.zIndex = 100;

    if (element.style.width) {
        phantom.style.width = element.style.width;
    }
    else {
        phantom.style.width = element.offsetWidth + "px";
    }

    convertPosition(element, phantom);

    if (IsIE() == true) {
        phantom.runtimeStyle.filter = "alpha(opacity=20);";
    }
    else {
        phantom.style.MozOpacity = 1 / 5;
    }

    phantom.style.display = "none";

    return Element.extend(phantom);
}

function adaptToBrowser() {
    // NOTE(IE BUG) Using css property overflow-y:scroll on iframes in IE7 causes it 
    // to display an empty gap on the right side of the frame. 
    if (IsIE7() == true) {
        var spacer = _$('ie7gapFix');
        if (spacer != null) {
            spacer.style.width = "36px";
        }
    }

    var monthSpacer = _$('idDummySpacer');
    if (IsIE() != true && monthSpacer != null) {
        monthSpacer.style.display = 'none';
    }

    var frame = document.getElementById('calendarFrame');

    if (monthSpacer != null && frame != null &&
       frame.contentWindow.document.documentElement.clientWidth < frame.offsetWidth) {
        if (IsIE7() == true) {
            monthSpacer.style.width = "36px";
        }
        if (IsIE() != true) {
            monthSpacer.style.display = 'inline';
        }

    }
}

function setWeekScrollPos(element) {
    adaptToBrowser();

    if (element == null) {
        return;
    }

    var frame = _$(element.id);
    if (frame != null) {
        frame.contentWindow.scrollBy(0, 250);
    }
}

function convertPosition(element, phantom) {
    if (element == null) {
        return;
    }

    phantom.style.top = findAbsPosY(element) + "px";
    phantom.style.left = findAbsPosX(element) + "px";
    phantom.style.position = "absolute";

}


function findPosX(element) {
    var curleft = -2;

    if (element != null && element.x) {
        curleft += element.x;
    }
    return curleft;
}


function findPosY(element) {
    var curtop = 0;

    if (element != null && element.y) {
        curtop += element.y;
    }
    return curtop;
}


function findAbsPosX(element) {
    return Element.extend(element).cumulativeOffset().left;

    //   if(element.offsetParent != null)
    //   {
    //      while(true) 
    //      {
    //         curleft += element.offsetLeft;

    //         if(element.offsetParent == null)
    //         {
    //            break;
    //         }

    //         element = element.offsetParent;
    //      }
    //    }
    //    else if(element.x)
    //    {
    //       curleft += element.x;
    //    }

    //   return curleft;
}


function findAbsPosY(element) {
    return Element.extend(element).viewportOffset().top;
    //   var curtop = 0;// element.cells[2].firstChild.firstChild.cells[0].firstChild.cells[0].clientHeight;
    //      
    //   if(element.offsetParent)
    //   {
    //      //curtop -= element.cells[1].clientHeight;
    //      //element = element.offsetParent;
    //      while(true)
    //      {
    //         curtop += element.offsetTop;


    //         if(!element.offsetParent)
    //         {
    //            break;
    //         }
    //         element = element.offsetParent;
    //      }
    //   }
    //   else if(element.y)
    //   {
    //      curtop += element.y;
    //   }
    //   return curtop;
}



function getTotalScrollTop(element) {
    return findAbsPosY(element) + Element.extend(element).cumulativeScrollOffset().top;
    //   var total = element.scrollTop;
    //    
    //   if(element.offsetParent)
    //   {
    //      while(true)
    //      {
    //         total += element.scrollTop;
    //         if(!element.offsetParent)
    //         {
    //            break;
    //         }
    //         element = element.offsetParent;
    //      }
    //   }

    //   return total;
}

function getTotalScrollLeft(element) {
    var total = element.scrollLeft;

    if (element.offsetParent) {
        while (true) {
            total += element.scrollLeft;
            if (!element.offsetParent) {
                break;
            }
            element = element.offsetParent;
        }
    }

    return total;
}

function AbsToRelX(absX, viewPointElement) {
    var x = findAbsPosX(viewPointElement);
    return absX - x;
}

function AbsToRelY(absY, viewPointElement) {
    var y = findAbsPosY(viewPointElement);
    return absY - y;
}

function _$(elementID) {
    var element = window.document.getElementById(elementID);
    if (element == null) {
        element = window.parent.document.getElementById(elementID);
        if (element == null) {
            if (document.getElementById('calendarFrame') != null)
                element = document.getElementById('calendarFrame').contentWindow.document.getElementById(elementID);
        }
    }

    return element;
}

function getFormatDate(date) {
    if (date.length == 4) {
        date = "0" + date;
    }
    return date;
}

function getFormatTime(time) {
    if (time.length == 4) {
        return "0" + time;
    }
    return time;
}

function addHours(time, hour) {
    var hours = parseInt(hour, 10) + parseInt(time.substring(0, time.indexOf(':')), 10);
    var minute = time.substring(time.indexOf(':') + 1, time.length);
    if (hours < 0) {
        hours = 0;
        minute = "00";
    }
    return getFormatDate(hours + ":" + minute);
}

function getTodayDate() {
    var date = new Date();
    var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
    var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();

    return date.getFullYear() + "/" + month + "/" + day;
}

function IsIE() {
    return window.navigator.appVersion.indexOf("MSIE") != -1;
}


function IsIE7() {
    if (IsIE() != true) {
        return;
    }

    return document.documentElement && typeof document.documentElement.style.maxHeight != "undefined";
}

function getInternetExplorerVersion() {

    var rv = -1
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }

    return rv;
}

function IsIE8() {

    var ver = getInternetExplorerVersion();
    if (ver > -1) {
        if (ver >= 8.0) {
            return true;
        }
    }
    return false;
}


function getElements(classnames) {
    var divs = $$(classnames);
    var calendars = new Array();
    var index = 0;
    for (i = 0; i < divs.length; ++i) {
            calendars[index] = divs[i];
            ++index;
    }
    return calendars;
}

function GetGridForEvent(event) {
    var grid = (event.target) ? event.target : event.srcElement;
    while (grid != null && grid.id != "idGrid") {
        grid = grid.parentNode;
    }
    return grid;
}

function GetEventGrid(element) {
    var tables = element.getElementsByTagName("table");
    for (i = 0; i < tables.length; ++i) {
        if (tables[i].className != "") {
            return tables[i];
        }
    }
    return null;
}

function FormatTimeToIndex(time) {
    var charIdx = time.indexOf(":");
    var hours = time.substring(0, charIdx);
    var minutes = time.substring(charIdx + 1, time.length);
    var startIndex = hours * 2;

    if (minutes >= 30) {
        startIndex++;
    }
    return startIndex;
}

/**********************************************
*  Enumerations
***********************************************/

DaysOfWeek =
{
    Monday: 1,
    Tuesday: 2,
    Wednesday: 4,
    Thursday: 8,
    Friday: 16,
    Saturday: 32,
    Sunday: 64
};

Recurrence =
{
    Once: 0,
    Daily: 1,
    Weekly: 2,
    Monthly: 3,
    Yearly: 4
};

Sequence =
{
    None: 0,
    First: 1,
    Second: 2,
    Third: 3,
    Fourth: 4,
    Last: 5
};


Months =
{
    January: 1,
    February: 2,
    March: 3,
    April: 4,
    May: 5,
    June: 6,
    Jule: 7,
    August: 8,
    September: 9,
    October: 10,
    November: 11,
    December: 12
};
