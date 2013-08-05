<%@ Page Language="C#" AutoEventWireup="true" Inherits="Sitecore.Modules.EventCalendar.UI.CalendarPopup" %>
<%@ Import Namespace="Sitecore.Modules.EventCalendar.Utils" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head" runat="server">
    <base target="_self" />
    <title>Select Date</title>
    <link href="/sitecore modules/shell/sitecore.calendar/Themes/Calendar.css" rel="stylesheet" />
    <link href="/sitecore modules/shell/sitecore.calendar/Themes/Colors.css" rel="stylesheet" />   
    <% Response.Write(string.Format("<link href='{0}' rel='stylesheet' type='text/css' />", 
                                    StaticSettings.Theme)); %>

</head>
<body style="position: absolute; margin: 0; padding: 0; width: 100%;height: 100%;display: table-cell; vertical-align: middle;resize:none" align="center">

    <form id="idFormPopup" runat="server" style="width: 100%;height: 100%;display: table-cell; vertical-align: middle;" align="center">
        <div style="font-size: 10px;display: table-cell; vertical-align: middle;" align="center">
            <asp:Calendar ID="idCalendarPopup" runat="server" Width="100%" DayNameFormat="FirstTwoLetters"
                BackColor="White" Height="100%" BorderStyle="Solid" BorderWidth="1px" CellPadding="3"
                EnableTheming="True" EnableViewState="False" CssClass="navCalendar" OnSelectionChanged="CalendarPopup_SelectionChanged">
                <SelectedDayStyle CssClass="navDaySelected"></SelectedDayStyle>
                <DayStyle CssClass="navCalendarText"></DayStyle>
                <WeekendDayStyle CssClass="navWeekdays"></WeekendDayStyle>
                <OtherMonthDayStyle CssClass="navOtherMonth" />
                <NextPrevStyle ForeColor="White" CssClass="navNextPrev"></NextPrevStyle>
                <DayHeaderStyle CssClass="navDayHeader"></DayHeaderStyle>
                <TitleStyle BackColor="white" CssClass="navHeader"></TitleStyle>
            </asp:Calendar>
        </div>
    </form>
</body>
</html>
