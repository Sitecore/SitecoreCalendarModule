using Sitecore.Web.UI.Sheer;
using System;

namespace Sitecore.Modules.EventCalendar.Forms.Dialogs
{
    [Serializable]
    public class AlertMessage : BasePipelineMessage
    {
        private readonly string message;

        public AlertMessage(string message)
        {
            this.message = message;
        }

        protected override void ShowUI()
        {
            Context.ClientPage.ClientResponse.Alert(message);
        }
    }
}