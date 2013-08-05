using System;
using Sitecore.Globalization;

namespace Sitecore.Modules.EventCalendar.Forms.Dialogs
{
    [Serializable]
    public class YesNoCancelMessage : BasePipelineMessage
    {
        private readonly string height;
        private readonly string message;
        private readonly string width;

        public YesNoCancelMessage(string message, string width, string height)
        {
            this.message = message;
            this.width = width;
            this.height = height;
        }
       
        protected override void ShowUI()
        {
            Context.ClientPage.ClientResponse.YesNoCancel(Translate.Text(message), width, height);
        }
    }
}