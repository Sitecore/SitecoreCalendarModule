using Sitecore.EventCalendar.Configure;

namespace Sitecore.Modules.EventCalendar.Forms.Dialogs
{
    public static class ClientDialogs
    {
        public static void Alert(string message)
        {
            new AlertMessage(message).Execute(null);
        }

        public static void YesNoCancelMessage(string message, string width, string height, ExecuteCallback callback)
        {
            new YesNoCancelMessage(message, width, height).Execute(callback);
        }
    }
}