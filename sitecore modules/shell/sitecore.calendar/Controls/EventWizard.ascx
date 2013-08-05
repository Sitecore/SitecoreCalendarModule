<%@ Control Language="C#" %>
<%@ Import Namespace="System.Threading"%>
<%@ Import Namespace="Sitecore.StringExtensions" %>
<div class="eventWizardFrame" id="idEventWizard">
    <div id="idEventWizardPanel" class="eventWizard" style="width: 100%">
        <input type="hidden" id="idEventID_hidden" />
        <input type="hidden" id="idScheduleID_hidden" />
        <input type="hidden" id="idCalendarID_hidden" />
        <table cellspacing="0" cellpadding="0" style="width: 100%;">
            <tr>
                <td class="eventWizardHeader" valign="top" colspan="4" style="width: 409px">
                    <table cellpadding="0" cellspacing="0" class="eventWizardTabsStrip">
                        <tr>
                            <td id="idDetails" class="eventWizardTabActive" style="width: 3px; height: 19px">
                                <a link="" class="wizardLink" onclick="EventWizard.getInstance().switchTab('idDetails');">
                                    Details</a>
                            </td>
                            <td id="idReoccur" class="eventWizardTab" style="height: 19px">
                                <a link="" class="wizardLink" onclick="EventWizard.getInstance().switchTab('idReoccur');">
                                    Reccurence</a>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" style="vertical-align: top;" class="eventWizardHeader" align="right"
                    colspan="2">
                    <a href="#" onclick="EventWizard.getInstance().Hide();return false;">
                        <img align="top" src="/sitecore modules/shell/sitecore.calendar/Images/No9x9.gif"
                            alt="Close" border="0" /></a>
                </td>
            </tr>
            <tr>
                <td valign="top" colspan="4" width="100%">
                    <table id="idDetailsTable" class="eventWizardMainPanel" style="width: 100%">
                        <tr>
                            <td align="right" style="width: 76px">
                                Subject:
                            </td>
                            <td style="width: 8px" colspan="3">
                                <input id="idSubject" class="ctrlInput" type="text" style="width: 450px;" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 76px;" align="right">
                                Location:
                            </td>
                            <td style="width: 8px; height: 14px;" colspan="3">
                                <input id="idLocation" class="ctrlInput" type="text" style="width: 450px;" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="width: 76px; white-space: nowrap" valign="top">
                                Start time:
                            </td>
                            <td style="width: 150px" valign="top">
                                <div style="white-space: nowrap">
                                    <input id="idStartDate" class="ctrlInput" style="width: 70px;" type="text" />
                                    <img id="idImgCalPopupStart" align="absMiddle" src='/sitecore modules/Shell/Sitecore.Calendar/Images/Calendar.bmp'
                                        onclick="EventWizard.getInstance().showCalendar('idStartDate');" class="calendarImage"
                                        width='24' height='24' alt='Open Calendar' border='0' />
                                    <select id="idStartTime" class="ctrlCombo" style="width: 78px;">
                                        <%
                                           var date = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
                                           
                                           for (int i = 1; i <= 48; ++i)
                                           {
                                        %>
                                        
                                             <option value='<%= date.ToString("HH:mm") + "'"  + (date.Hour == 9 && date.Minute == 0 ? "selected='selected'" : string.Empty) %>' >
                                                <%=
                                           	        date.ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern) 
                                                %>
                                             </option>
                                             
                                        <%
                                             date = date.AddMinutes(30);
                                           }
                                        %>

                                    </select>
                                </div>
                                <span class="helpText">(yyyy/mm/dd)</span>&nbsp;
                            </td>
                            <td align="right" valign="top" style="width: 60px; white-space: nowrap">
                                End time:
                            </td>
                            <td valign="top" style="width: 150px">
                                <div style="white-space: nowrap">
                                    <input id="idEndDate" class="ctrlInput" style="width: 75px;" type="text" />
                                    <img id="idImgCalPopupEnd" align="absMiddle" src='/sitecore modules/Shell/Sitecore.Calendar/Images/Calendar.bmp'
                                        onclick="EventWizard.getInstance().showCalendar('idEndDate');" class="calendarImage"
                                        width='24' height='24' alt='Open Calendar' border='0' />
                                    <select id="idEndTime" class="ctrlCombo" style="width: 78px;">
                                        <%
                                            date = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);

                                            for (int i = 1; i <= 48; ++i)
                                            {
                                        %>
                                             <option value='<%= date.ToString("HH:mm") + "'"  + (date.Hour == 9 && date.Minute == 30 ? "selected='selected'" : string.Empty) %>' >
                                                <%=
                                           	        date.ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern) 
                                                %>
                                             </option>
                                             
                                        <%
                                             date = date.AddMinutes(30);
                                           }
                                        %>
                                    </select>
                                </div>
                                <span class="helpText">(yyyy/mm/dd)</span>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="width: 76px; height: 24px">
                                Calendar:
                            </td>
                            <td colspan="3" style="height: 24px">
                                <select id="idCalendar" class="ctrlCombo" style="width: 229px">
                                    <option selected="selected"></option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="height: 119px; width: 76px;">
                                Description:
                            </td>
                            <td colspan="3" style="height: 119px; width: 457px;">
                                <textarea id="idDescription" style="width: 98%; height: 111px; font-size: small"></textarea>
                            </td>
                        </tr>
                    </table>
                    <table id="idReoccurTable" class="eventWizardMainPanel" style="display: none; width: 100%">
                        <tr>
                            <td colspan="3" style="width: 100%; height: 10px;" valign="top">
                                <table width="100%" cellpadding="0" cellspacing="0" class="recurSelector">
                                    <tr>
                                        <td width="20%">
                                            <input type="radio" name="recurList" id="idRecurOnce" value="Only Once" onclick="EventWizard.getInstance().onClickRecur(this);"
                                                checked="CHECKED" /><label for="idRecurOnce"><nobr>Only Once</nobr></label>
                                        </td>
                                        <td width="20%">
                                            <input type="radio" name="recurList" id="idRecurDaily" value="Daily" onclick="EventWizard.getInstance().onClickRecur(this);" /><label
                                                for="idRecurDaily">Daily</label>
                                        </td>
                                        <td width="20%">
                                            <input type="radio" name="recurList" id="idRecurWeekly" value="Weekly" onclick="EventWizard.getInstance().onClickRecur(this);" /><label
                                                for="idRecurWeekly">Weekly</label>
                                        </td>
                                        <td width="20%">
                                            <input type="radio" name="recurList" id="idRecurMonthly" value="Montly" onclick="EventWizard.getInstance().onClickRecur(this);" /><label
                                                for="idRecurMonthly">Monthly</label>
                                        </td>
                                        <td width="20%">
                                            <input type="radio" name="recurList" id="idRecurYearly" value="Yearly" onclick="EventWizard.getInstance().onClickRecur(this);" /><label
                                                for="idRecurYearly">Yearly</label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" valign="top">
                                <table id="idWeekRecur" style="display: none" width="100%">
                                    <tr>
                                        <td style="width: 89px; height: 9px" valign="top">
                                            <nobr>
                                    Recur every
                                    <input id="idWeekRecurNum" class="ctrlInput" style="width: 30px" type="text" />
                                    week(s) on:</nobr>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 89px;" valign="top">
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td>
                                                        <input type="checkbox" name="weekDaysList" id="idCheckMonday" value="Monday" /><div>
                                                            <label for="idCheckMonday">
                                                                Monday</label></div>
                                                    </td>
                                                    <td>
                                                        <input type="checkbox" name="weekDaysList" id="idCheckTuesday" value="Tuesday" /><div>
                                                            <label for="idCheckTuesday">
                                                                Tuesday</label>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <input type="checkbox" name="weekDaysList" id="idCheckWednesday" value="Wednesday" /><div>
                                                            <label for="idCheckWednesday">
                                                                Wednesday</label></div>
                                                    </td>
                                                    <td>
                                                        <input type="checkbox" name="weekDaysList" id="idCheckThursday" value="Thursday" /><div>
                                                            <label for="idCheckThursday">
                                                                Thursday</label></div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <input type="checkbox" name="weekDaysList" id="idCheckFriday" value="Friday" /><div>
                                                            <label for="idCheckFriday">
                                                                Friday</label></div>
                                                    </td>
                                                    <td>
                                                        <input type="checkbox" name="weekDaysList" id="idCheckSaturday" value="Saturday" />
                                                        <div>
                                                            <label for="idCheckSaturday">
                                                                Saturday</label></div>
                                                    </td>
                                                    <td>
                                                        <input type="checkbox" name="weekDaysList" id="idCheckSunday" value="Sunday" />
                                                        <div>
                                                            <label for="idCheckSunday">
                                                                Sunday</label>
                                                        </div>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                    </tr>
                                </table>
                                <table id="idDayRecur" style="display: none" width="100%">
                                    <tr>
                                        <td style="width: 89px; height: 9px" valign="top">
                                            <nobr>
                                    Recur every
                                    <input id="idDayRecurNum" class="ctrlInput" style="width: 30px" type="text" />
                                    day(s)</nobr>
                                        </td>
                                    </tr>
                                </table>
                                <table id="idMonthRecur" style="display: none" width="100%">
                                    <tr>
                                        <td style="width: 89px; height: 9px" valign="top">
                                            <nobr>
                                    The
                                    <select id="idDayOfWeekOrdinal" class="ctrlCombo" style="width: 62px">
                                       <option selected="selected" value="0">First</option>
                                       <option value="1">Second</option>
                                       <option value="2">Third</option>
                                       <option value="3">Fourth</option>
                                       <option value="4">Last</option>
                                    </select>
                                    <select id="idPartOfWeek" class="ctrlCombo" style="width: 105px">
                                       <option selected="selected" value="1">Monday</option>
                                       <option value="2">Tuesday</option>
                                       <option value="4">Wednesday</option>
                                       <option value="8">Thursday</option>
                                       <option value="16">Friday</option>
                                       <option value="32">Saturday</option>
                                       <option value="64">Sunday</option>
                                    </select>
                                    of every
                                    <input id="idMonthRecurWeekDayOrdinal" class="ctrlInput" style="width: 30px" type="text" />
                                    month(s)</nobr>
                                        </td>
                                        <td style="width: 89px; height: 9px" valign="top">
                                        </td>
                                    </tr>
                                </table>
                                <table id="idYearRecur" style="display: none" width="100%">
                                    <tr>
                                        <td style="width: 89px; height: 9px" valign="top">
                                            <nobr>
                                    Every
                                    <select id="idYearOccurMonth" class="ctrlCombo" style="width: 90px">
                                       <option selected="selected" value="1">January</option>
                                       <option value="2">February</option>
                                       <option value="3">March</option>
                                       <option value="4">April</option>
                                       <option value="5">May</option>
                                       <option value="6">June</option>
                                       <option value="7">July</option>
                                       <option value="8">August</option>
                                       <option value="9">September</option>
                                       <option value="10">October</option>
                                       <option value="11">November</option>
                                       <option value="12">December</option>
                                    </select>
                                    <input id="idYearOccurDay" class="ctrlInput" style="width: 30px" type="text" /></nobr>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table class="eventWizardBFrame" cellspacing="1" cellpadding="1">
                        <tr>
                            <td class="eventWizardButton">
                                <a id="idBtnSave" link="" class="wizardLink" onclick="EventWizard.getInstance().Save(); return false;">
                                    Save</a>
                            </td>
                            <td class="eventWizardButton">
                                <a id="idBtnClear" link="" class="wizardLink" onclick="EventWizard.getInstance().Clear(); return false;">
                                    Clear</a>
                            </td>
                            <td class="eventWizardButton">
                                <a id="idBtnDelete" link="" class="wizardLink" onclick="EventWizard.getInstance().DeleteEvent(); return false;">
                                    Delete</a>
                            </td>
                            <td class="eventWizardButton">
                                <a id="idBtnClose" link="" class="wizardLink" onclick="EventWizard.getInstance().Hide(); return false;">
                                    Close</a>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <br />
</div>
<div id="idModalBackground">
</div>
