using System;
using System.Text;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Applications.Install.Controls;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.WebControls;

using WebLiteral = System.Web.UI.LiteralControl;
using HtmlLiteral = Sitecore.Web.UI.HtmlControls.Literal;
using XmlButton = Sitecore.Web.UI.HtmlControls.Button;
using HmtlEdit = Sitecore.Web.UI.HtmlControls.Edit;

namespace Sitecore.Modules.EventCalendar.Shell.Controls
{
   public class ControlsTreeList : TreeList
   {
      public static readonly string updateButtonCommand =
         "var items = document.getElementById('ManagerPanel_TreeList_selected');items.style.width=items.parentNode.clientWidth+'px';";

      public static readonly string updateListCommand = updateButtonCommand +
         "document.getElementById('ManagerPanel_TreeList_help').innerHTML = items.selectedIndex>=0?items.options[items.selectedIndex].innerHTML:'';return document.getElementById('ManagerPanel_TreeList_help').innerHTML;";

      private Listbox listRendarings;
      public EventHandler OnSelectedItemChanged;

      public Listbox ListRenderings
      {
         get
         {
            if (listRendarings == null)
            {
               listRendarings = FindControl(GetID("selected")) as Listbox;
            }
            return listRendarings;
         }
      }

      protected void SelectItemChange()
      {
         if (OnSelectedItemChanged != null)
         {
            OnSelectedItemChanged(this, null);
         }
      }

      protected override void OnPreRender(EventArgs e)
      {
         base.OnPreRender(e);

         HtmlLiteral literal = Controls[0].Controls[0] as HtmlLiteral;
         if (literal != null)
         {
            literal.Text = "Controls";
         }

         GridPanel panel = Controls[0] as GridPanel;

         if (panel != null)
         {
            panel.Style.Add("Background", "#e9e9e9");
            panel.Style.Add("border", "0");
            panel.Style.Add("height", "100%");
         }

         Listbox list = ListRenderings;

         WebLiteral divText = Controls[0].Controls[8] as WebLiteral;
         if (divText != null && list.SelectedItem != null)
         {
            Item item = StaticSettings.MasterDatabase.GetItem(list.SelectedItem.Value.Split('|')[1]);
            if (item != null)
            {
               if (divText.Text != null)
               {
                  divText.Text = divText.Text.Insert(divText.Text.IndexOf("</div>"), item.Name);
               }
               else
               {
                  divText.Text = item.Name;
               }
            }
         }

         list.Size = "14";

         StringBuilder script = new StringBuilder();
         script.Append(
            Sitecore.Context.ClientPage.GetClientEvent(ID + ".SelectItemChange").Remove(0, "javascript:return".Length));
         script.Append(";");
         script.Append(updateListCommand);
         list.Attributes.Add("onchange", script.ToString());
      }
   }
}