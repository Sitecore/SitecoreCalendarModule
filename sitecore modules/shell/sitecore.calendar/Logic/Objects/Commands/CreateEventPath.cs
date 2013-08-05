using System;
using System.Collections.Specialized;

using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Core;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.EventCalendar.Objects.Commands
{
   public class CreateEventPath : Command
   {
      #region Methods

      public override void Execute(CommandContext context)
      {
         Assert.ArgumentNotNull(context, "context");
         if (context.Items.Length == 1)
         {            
            var parameters = new NameValueCollection();
            parameters["items"] = SerializeItems(context.Items);            
            Context.ClientPage.Start(this, "Run", parameters);
         }
      }

      protected void Run(ClientPipelineArgs args)
      {
         Assert.ArgumentNotNull(args, "args");
         Item item = DeserializeItems(args.Parameters["items"])[0];

         if (args.IsPostBack)
         {
            if (args.HasResult)
            {
               string datePath = Utilities.NormalizeDate(DateTime.Now);
               Item eventItem = Utilities.CreateDatePath(item, datePath);

               if (eventItem != null)
               {
                  eventItem = eventItem.Add(args.Result, new TemplateID(CalendarIDs.EventTemplate));
               }
            }
         }
         else
         {
            string text = "Enter the name of the new item:";
            string defaultValue = "New Event";

            Context.ClientPage.ClientResponse.Input(text, defaultValue, Settings.ItemNameValidation, "'$Input' is not a valid name.", 100);
            args.WaitForPostBack();
         }
      }


      #endregion

   }
}