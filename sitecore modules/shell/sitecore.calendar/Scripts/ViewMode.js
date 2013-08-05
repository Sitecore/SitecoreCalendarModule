function ViewMode() {
    this._calendars = null;
    this.Init();
}

ViewMode._instance = null;

ViewMode.getInstance = function () {
    if (ViewMode._instance == null) {
        ViewMode._instance = new ViewMode();
    }

    return ViewMode._instance;
}

ViewMode.prototype.Init = function () {
    this._calendar = _$('idGrid').parentNode.parentNode;
}

ViewMode.prototype.SwitchToDayView = function (idCalendar) {
    var tableGrid = _$(idCalendar).firstChild.lastChild.previousSibling;

    ViewMode.getInstance()._calendar = _$(idCalendar);
    Proxy.getInstance().SwitchToDayView(
                idCalendar,
                tableGrid.getAttribute('date'),
                _$(idCalendar).getAttribute('siteSettings'),
                _$(idCalendar).getAttribute('viewSettingPath'),
                tableGrid.getAttribute('numHours'),
                ViewMode.getInstance().onUpdateOneView);
}

ViewMode.prototype.SwitchToMonthView = function (idCalendar) {
    var tableGrid = _$(idCalendar).firstChild.lastChild.previousSibling;

    ViewMode.getInstance()._calendar = _$(idCalendar);
    Proxy.getInstance().SwitchToMonthView(
                idCalendar,
                tableGrid.getAttribute('date'),
                _$(idCalendar).getAttribute('siteSettings'),
                _$(idCalendar).getAttribute('viewSettingPath'),
                tableGrid.getAttribute('numHours'),
                ViewMode.getInstance().onUpdateOneView);
}

ViewMode.prototype.SwitchToWeekView = function (idCalendar) {
    var tableGrid = _$(idCalendar).firstChild.lastChild.previousSibling;

    ViewMode.getInstance()._calendar = _$(idCalendar);
    Proxy.getInstance().SwitchToWeekView(
                idCalendar,
                tableGrid.getAttribute('date'),
                _$(idCalendar).getAttribute('siteSettings'),
                _$(idCalendar).getAttribute('viewSettingPath'),
                tableGrid.getAttribute('numHours'),
                ViewMode.getInstance().onUpdateOneView);
}

ViewMode.prototype.SwitchToDayViewToDay = function (idCalendar, date) {
    ViewMode.getInstance()._calendar = _$(idCalendar);
    Proxy.getInstance().SwitchToDayView(
                idCalendar,
                date,
                _$(idCalendar).getAttribute('siteSettings'),
                _$(idCalendar).getAttribute('viewSettingPath'),
                _$(idCalendar).firstChild.lastChild.previousSibling.getAttribute('numHours'),
                ViewMode.getInstance().onUpdateOneView);
}

ViewMode.prototype.SwitchToMonthViewToDay = function (idCalendar, date) {
    ViewMode.getInstance()._calendar = _$(idCalendar);
    Proxy.getInstance().SwitchToMonthView(
                idCalendar,
                date,
                _$(idCalendar).getAttribute('siteSettings'),
                _$(idCalendar).getAttribute('viewSettingPath'),
                _$(idCalendar).firstChild.lastChild.previousSibling.getAttribute('numHours'),
                ViewMode.getInstance().onUpdateOneView);
}

ViewMode.prototype.SwitchToWeekViewToDay = function (idCalendar, date) {
    ViewMode.getInstance()._calendar = _$(idCalendar);
    Proxy.getInstance().SwitchToWeekView(
                idCalendar,
                date,
                _$(idCalendar).getAttribute('siteSettings'),
                _$(idCalendar).getAttribute('viewSettingPath'),
                _$(idCalendar).firstChild.lastChild.previousSibling.getAttribute('numHours'),
                ViewMode.getInstance().onUpdateOneView);
}

ViewMode.prototype.onUpdateView = function (html) {
    ViewMode.getInstance().onUpdateAllView(html, true);
    if (ViewMode.getInstance()._calendar.firstChild.className != "monthGrid") {
        ViewMode.getInstance()._calendar.style.position = "relative";
    }
    else {
        ViewMode.getInstance()._calendar.style.position = "static";
        ViewMode.getInstance()._calendar.firstChild.style.position = "relative";
    }
}

ViewMode.prototype.onUpdateOneView = function (html) {
    ViewMode.getInstance().onUpdateAllView(html, false);
    if (ViewMode.getInstance()._calendar.firstChild.className != "monthGrid") {
        ViewMode.getInstance()._calendar.style.position = "relative";
    }
    else {
        ViewMode.getInstance()._calendar.style.position = "static";
        ViewMode.getInstance()._calendar.firstChild.style.position = "relative";
    }
}


ViewMode.prototype.onUpdateAllView = function (html, updateAll) {
    if (ViewMode.getInstance()._calendar != null) {
        ViewMode.getInstance()._calendar.innerHTML = html;
        if (updateAll) {
            ViewMode.getInstance().onUpdateCalendars();
        }
    }
}


ViewMode.prototype.onUpdateCalendars = function () {
    var classes = [];
    classes["calendar"] = "calendar";
    classes["calendarMonth"] = "calendarMonth";
    //classes["navCalendarDiv"] = "navCalendarDiv";

    ViewMode.getInstance()._calendars = getElements("div.calendar, div.calendarMonth");

    for (i = 0; i < ViewMode.getInstance()._calendars.length; ++i) {
        if (ViewMode.getInstance()._calendars[i] != ViewMode.getInstance()._calendar) {
            var calendar = $(ViewMode.getInstance()._calendars[i]);
            if (calendar.hasClassName("calendar") || calendar.hasClassName("calendarMonth")) {
                calendar = calendar.firstChild.lastChild.previousSibling;
            }

            if (calendar.hasClassName('eventToolbarFrame') || calendar.hasClassName('typeViewSelector')) {
                continue;
            }

            var options = new Sitecore.Modules.EventCalendar.Logic.Utils.Options();
            options.CalendarID = ViewMode.getInstance()._calendars[i].id;
            options.Date = calendar.getAttribute('date');
            options.SiteSettingsPath = ViewMode.getInstance()._calendars[i].getAttribute('siteSettings');
            options.ControlSettingsPath = ViewMode.getInstance()._calendars[i].getAttribute('viewSettingPath') || ViewMode.getInstance()._calendars[i].getAttribute('selectorSettingPath')

            Proxy.getInstance()._service.GetViewHTML(
                   i,
                   calendar.getAttribute('startHour') || 0,
                   calendar.getAttribute('mode'),
                   calendar.getAttribute('mode'),
                   calendar.getAttribute('numHours') || 0,
                   options,
                   function (html) {
                       html = html.substring(html.indexOf("<calendarNum>") + 13, html.length);
                       var num = parseInt(html.substring(0, html.indexOf("</calendarNum>")), 10);
                       html = html.substring(html.indexOf(">") + 1, html.length);
                       ViewMode.getInstance()._calendars[num].innerHTML = html;


                       if (ViewMode.getInstance()._calendars[num].firstChild.className != "monthGrid") {
                           ViewMode.getInstance()._calendars[num].style.position = "relative";
                       }
                       else {
                           ViewMode.getInstance()._calendars[num].style.position = "static";
                           ViewMode.getInstance()._calendars[num].firstChild.style.position = "relative";
                       }

                   })
        }
    }

    TimeHelper.Clear();
}

