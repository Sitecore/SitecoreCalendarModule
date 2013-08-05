
function leapYear(year)
{
   Last2Digits = year % 100
   if (Last2Digits == 0)
   {
       flag = year % 400
   }
   else
   {
       flag = year % 4
   }
   if (flag == 0)
   {
       return true;     
   }
   else
   {
      return false;     
   } 
}

function getDays(month, year) {
  var days = 31;
  switch (month) 
  {
      case 2 : days = (leapYear(year)) ? 29 : 28;
               break; 
      case 4 :
      case 6 :
      case 9 :
      case 11:
               days = 30;
               break;
  }
  
  return days;
}

function getMaxDaysCountInMonth(month) {
    var days = 31;
    switch (month) {
        case 2: days = 29;
            break;
        case 4:
        case 6:
        case 9:
        case 11:
            days = 30;
            break;
    }

    return days;
}

function UpdateDateSelector()
{
    if (_$('idSelectorMonth') != null && _$('idSelectorYear') != null)
    {
        var days = getDays(parseInt(_$('idSelectorMonth').selectedIndex + 1, 10), parseInt(_$('idSelectorYear').value, 10));    
        var selectDays = _$('idSelectorDay');    
        if (selectDays != null)
        {
            while (selectDays.length > days)
            {
                selectDays.remove(selectDays.length - 1);
            }
            while (selectDays.length < days)
            {
                var option = document.createElement('option');
                option.text = selectDays.length + 1;
                option.value = selectDays.length + 1;
                try
                {
                    selectDays.add(option, null);
                }
                catch(ex)
                {
                    selectDays.add(option);
                }                
            }       
        }
    }
}