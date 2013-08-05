<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <base target="_self" />
    <title>Confirm<%=Request.QueryString["operation"] %></title>
    <style type="text/css">
        body
        {
            background-color: ThreeDFace;
            font-family: Tahoma;
            font-size: 8pt;
            font-weight: lighter;
        }
        form
        {
            padding: 10px;
        }
        .buttons
        {
            text-align: center;
            padding: 15px 10px 0px 10px;
        }
        .buttons input
        {
            width: 70px;
            font-size: 1em;
        }
        .message
        {
            padding-left: 20px;
        }
        .choise
        {
            padding: 3px 0px 3px 55px;
        }
    </style>

    <script language="javascript" type="text/javascript">
        function Selected(){
            var oneMode = document.getElementById("One");
            var allMode = document.getElementById("All");
            
            return oneMode.checked ? "one" : "all";
        };
        
        function SetResult(result){        

            if (window.dialogArguments) { 
            
                window.opener = window.dialogArguments;                
            };
            
            window.opener._modalDialog.eventhandler(result, Selected());            
            self.close();
            
            return false;
        }
    </script>

</head>
<body>
    <form id="form1">
    <div>
        <table width="100%" height="100%">
            <tr>
                <td>
                    <img src="<%=Sitecore.Resources.Images.GetThemedImageSource("Applications/32x32/Warning.png", Sitecore.Web.UI.ImageDimension.id32x32)%>" 
                        width="32" height="32" margin="8px 0px 8px 16px" />
                </td>
                <td>
                    <div class="message">
                        Do you want to
                        <%=Request.QueryString["operation"].ToLower()%>
                        all occurrences of the recurring event
                        <%=Request.QueryString["name"]%>, or just this one?
                    </div>
                </td>
            </tr>
        </table>
        <div class="choise">
            <div>
                <input id="One" value="One" type="radio" checked="true" name="Mode" />
                <label for="One">
                    <%=Request.QueryString["operation"]%>
                    this occurrence.</label>
            </div>
            <div>
                <input id="All" value="All" type="radio" name="Mode" />
                <label for="All">
                    <%=Request.QueryString["operation"]%>
                    the series.</label>
            </div>
        </div>
        <div class="buttons">
            <input id="Yes" type="submit" onclick="javascript:return SetResult('yes');" value="Yes" name="Comnfirm$Yes" />&nbsp;&nbsp;
            <input id="No" type="submit" onclick="javascript:return SetResult('no');" value="No" name="Comnfirm$No" />
        </div>
    </div>
    </form>
</body>
</html>
