
function ConfirmEx() {
    this._window = '';
}

var _modalDialog;

ConfirmEx.prototype = {
    
    show : function(operation, name, handler){     
        this._clear();
        this._dialogShow(operation, name, handler);
    },
    
    _clear : function (){
        _modalDialog = {value : '', mode : '', eventhandler : ''};
    },
    
    _dialogShow : function(operation, name, handler){

      _modalDialog.eventhandler = handler;
       
      var args = 'help:no;dialogWidth:350px;dialogHeight:150px;toolbar:no;location:no;status:no;menubar:no;scroll:no;scrollbars:no,resizable:no;';  
      var url = "/sitecore%20modules/shell/sitecore.calendar/controls/ConfirmEx.aspx" + "?operation=" + operation + "&name=" + name;
        
      if (window.showModalDialog) {
         this._window = window.showModalDialog(url, self, args);
      }else {
         this._window = window.open(url, self, args);
      }          
   }  
}
