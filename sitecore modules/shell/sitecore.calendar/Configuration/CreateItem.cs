using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System;

namespace Sitecore.EventCalendar.Configure
{
   public delegate void ExecuteCallback(string id);

   public class CreateItem : Create
   {
      private readonly ExecuteCallback callback;

      public CreateItem(ExecuteCallback callback)
      {
         this.callback = callback;
      }

      protected new void Run(ClientPipelineArgs args)
      {
         string strMaster = StringUtil.GetString(new string[] {args.Parameters["master"]});
         string strTemplate = StringUtil.GetString(new string[] {args.Parameters["template"]});
         Database database = Factory.GetDatabase(StringUtil.GetString(new string[] {args.Parameters["database"]}));
         if (args.IsPostBack)
         {
            if (args.HasResult)
            {
               Item parent = database.Items[args.Parameters["id"]];
               if (parent != null)
               {
                  try
                  {
                     if (strMaster.Length > 0)
                     {
						BranchItem branch = database.Branches[strMaster];
                        PostStep(Context.Workflow.AddItem(args.Result, branch, parent));
                     }
                     else
                     {
                        PostStep(database.Templates[strTemplate].AddTo(parent, args.Result));
                     }
                  }
                  catch (UnauthorizedAccessException ex)
                  {
                     Context.ClientPage.ClientResponse.Alert(ex.Message);
                  }
               }
               else
               {
                  Context.ClientPage.ClientResponse.ShowError(ResourceManager.Localize("PARENT_NOT_FOUND"), "");
                  args.AbortPipeline();
               }
            }
         }
         else
         {
            string defaultValue;
            string text =
               StringUtil.GetString(
                  new string[] {args.Parameters["prompt"], ResourceManager.Localize("ENTER_ITEM_NAME")});
            if (strMaster.Length > 0)
            {
                    defaultValue = database.Branches[strMaster].Name;
            }
            else
            {
               defaultValue = database.Templates[strTemplate].Name;
            }
            Context.ClientPage.ClientResponse.Input(text, defaultValue, Settings.ItemNameValidation,
                                                    ResourceManager.Localize("NOT_VALIDE_NAME"), 100);
             
            args.WaitForPostBack();
         }
      }

      protected void PostStep(Item item)
      {
         Assert.ArgumentNotNull(item, "item");

         if (callback != null)
         {
            callback(item.ID.ToString());
         }

         UrlString url = new UrlString();
         url.Append("ro", item.ID.ToString());
         url.Append("fo", item.ID.ToString());
         url.Append("id", item.ID.ToString());
         url.Append("la", item.Language.Name);
         url.Append("vs", item.Version.Number.ToString());
         Windows.RunApplication("Content editor", url.GetUrl());
      }
   }
}