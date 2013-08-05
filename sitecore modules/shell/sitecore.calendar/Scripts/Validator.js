/******************************************************
* Validator
******************************************************/
function Validator() {
}

Validator._instance = null;

Validator.getInstance = function() {
   if(Validator._instance == null) {
      Validator._instance = new Validator();
   } 

   return Validator._instance;
}


Validator.prototype.IsEmpty = function(element) { 
   if(element == null) {
      throw new Error("ELEMENT IS NULL");
   } 
   
   if( element.value == "") {
      this.indicateError(element,"Field cannot be empty!");
      return false;
   }
   
   if( this.Sanitize(element.value) == "") {
      this.indicateError(element,"Field name is invalid or field cannot be empty!");
      return false;
   }
   
   return true;
}


Validator.prototype.Sanitize = function(original) {
    var results = "";

    if (original == null || original.length == 0) {
        return "";
    }

    var c;
    original = original.replace(/^\s+|\s+$/g, '');
    
    for (i = 0; i < original.length; i++) {
           c = original.charAt(i);
           if ((c >= 'A' && c <= 'Z') ||
               (c >= 'a' && c <= 'z') ||
               (c >= '0' && c <= '9') ||
                c == ' ' || c == '-') {
                  results += c;
               }
    }
    
    if (results.charAt(0) == '-') {
        results = "";
    }

    return results;
}


Validator.prototype.IsValidDate = function(element) {  
   var error = "Date Format is invalid!";
   if (this.validate( element, /^\d\d\d\d[/](0[1-9]|1[012])[/](0[1-9]|[12][0-9]|3[01])$/,  error)) {
        if(!Validator.getInstance().IsValidDateScorpe(element)) {
            this.indicateError(element, error);
            return false;	
        }
        return true;
    }
    return false;
}


Validator.prototype.validate = function(element, rexExpPattern, errorMsg) {
   if(element == null) {
      throw new Error("ELEMENT IS NULL");
   } 
   element.style.backgroundColor = "";

   var value = element.value;
   if( !value.match(rexExpPattern)) {
      this.indicateError(element, errorMsg);
      return false;	
   } 
 
   return true;
}

Validator.prototype.IsValidDateScorpe = function(element) {
   var value = element.value;
   var year = parseInt(value.substring(0, 4), 10);
   var month = parseInt(value.substring(5, 7), 10);
   var day = parseInt(value.substring(8, 10), 10);
   
   if (month > 0 && month <= 12 &&
       getDays(month, year) >= day) {
       return true;
   }
   return false;
}

Validator.prototype.isValidDayNumber = function(month, day, element, errorMsg){
    if(day <= 0 || getMaxDaysCountInMonth(month) < day){
        this.indicateError(element, errorMsg);
        return false;	
    }
    return true;
}

Validator.prototype.indicateError = function(element, errorMsg) {
   var oldBC = element.style.backgroundColor;
   element.style.backgroundColor = "red";

   alert(errorMsg);   

   if( element.visible == true) {
      element.focus();
   }

   element.style.backgroundColor = oldBC;
}


Validator.prototype.validateDatesRange = function(startDate, endDate) {
   var start = this.normalizeDate(startDate.value);
   var end   = this.normalizeDate(endDate.value);

   if ( start > end) {
      window.status = start + " "+ end;
      this.indicateError(endDate, "Start Date occurs after End Date!");
      
      return false;
   }
   
   endDate.style.backgroundColor = "";
   return true;
}


Validator.prototype.validateTimeRange = function(startDate, startTime, endDate, endTime) {
   var start = this.normalizeDate(startDate.value);
   var end   = this.normalizeDate(endDate.value);

   if(Date.parse(start) < Date.parse(end)) {
      return true;
   }

   if( this.compareTimes(startTime.value, endTime.value) == false) {
      this.indicateError(endTime, "Start Time occurs after End Time!");
      
      return false;
   }
   
   endTime.style.backgroundColor = "";
   return true;
}

Validator.prototype.normalizeDate = function(date) {
   var d = new Date();
   var arr = date.split('/');
   d.setFullYear(arr[0], arr[1], arr[2]);
   return d; 
}

Validator.prototype.compareTimes = function(startTime, endTime) {
   var startTimeArr = startTime.split(":");
   var endTimeArr   = endTime.split(":");
   
   if( startTimeArr[0] >  endTimeArr[0]) {
      return false;
   }
 
   if( ( startTimeArr[0] == endTimeArr[0]) &&  (startTimeArr[1] > endTimeArr[1]) ) {
      return false;
   }
   
   return true;
}

Validator.prototype.validateFieldLength = function(element, limit) {
   if(element == null) {
      throw new Error("ELEMENT IS NULL");
   } 
   
   if( element.value.length > limit) {
      this.indicateError(element, "Field length cannot exceed more than " + limit + " symbols");
      return false;
   } 

   element.style.backgroundColor = "";
   return true;
}
