using System;
using Sitecore.EventCalendar.Configure;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.EventCalendar.Forms.Dialogs
{
    [Serializable]
    public abstract class BasePipelineMessage
    {
        private ExecuteCallback callback;

        public virtual void Execute(ExecuteCallback callback)
        {
            this.callback = callback;
            Context.ClientPage.Start(this, "Pipeline");
        }

        protected virtual void Pipeline(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                ShowUI();
                Context.ClientPage.ClientResponse.Redraw();
                args.WaitForPostBack();
            }
            else
            {
                if (args.HasResult)
                {
                    if (callback != null)
                    {
                        callback(args.Result);
                    }
                }
            }
        }

        protected abstract void ShowUI();
    }
}