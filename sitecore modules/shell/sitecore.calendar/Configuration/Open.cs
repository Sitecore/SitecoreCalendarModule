using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System.Collections.Specialized;

namespace Sitecore.EventCalendar.Configure
{
   public class Open : Command
   {
      public override void Execute(CommandContext context)
      {
         Error.AssertObject(context, "context");
         if (context.Items.Length == 1)
         {
            Execute(context.Items[0]);
         }
      }

      protected virtual void Execute(Item item)
      {
         Error.AssertObject(item, "item");
         NameValueCollection parameters = new NameValueCollection();
         parameters["id"] = item.ID.ToString();
         parameters["databasename"] = item.Database.Name;
         Context.ClientPage.Start(this, "Run", parameters);


      }

      protected virtual void Run(ClientPipelineArgs args)
      {
         string databaseName = args.Parameters["databasename"];
         string id = args.Parameters["id"];

         Database database = Factory.GetDatabase(databaseName);
         Assert.IsNotNull(database, typeof (Database), "Database \"" + database + "\" not found.", new object[0]);
         Item item = database.Items[id];

         if (item != null)
         {
					 if (item.Fields[FieldIDs.LayoutField] != null && item.Fields[FieldIDs.LayoutField].Value != string.Empty)
            {
                  if (!args.IsPostBack)
                  {
                     UrlString url = new UrlString(UIUtil.GetUri("control:Calendar.ConfigureControls"));
                     url.Append("id", item.ID.ToString());
                     url.Append("db", item.Database.Name);

                     Windows.RunApplication("Calendar ConfigureControls", url.GetUrl()); 
                  }
            }
            else
            {
               Context.ClientPage.ClientResponse.Alert(ResourceManager.Localize("ITEM_HAS_NO_LAYOUT"));
            }
         }
         else
         {
            SheerResponse.Alert("Item not found.", new string[0]);
         }
      }
   }
}