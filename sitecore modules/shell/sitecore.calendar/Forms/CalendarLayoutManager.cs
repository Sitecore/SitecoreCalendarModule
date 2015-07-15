using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Layouts;
using Sitecore.Modules.EventCalendar.Core;
using Sitecore.Modules.EventCalendar.Forms.Dialogs;
using Sitecore.Modules.EventCalendar.Shell.Controls;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using WebLiteral = System.Web.UI.LiteralControl;
using HtmlLiteral = Sitecore.Web.UI.HtmlControls.Literal;
using XmlButton = Sitecore.Web.UI.HtmlControls.Button;
using HtmlEdit = Sitecore.Modules.EventCalendar.Shell.Controls.Edit;
using HtmlControl = Sitecore.Web.UI.HtmlControls.Control;
using RenderingSettings = Sitecore.Modules.EventCalendar.Shell.Controls.Helper.RenderingSettings;
using Sitecore.Data.Fields;
using System.Reflection;

namespace Sitecore.Modules.EventCalendar.Forms
{
   public class CalendarLayoutManager : ApplicationForm
   {
      public static readonly string AgendaViewRenderingID = "{15CA9BAF-A481-4C28-91E8-DDC005F7D742}";
      public static readonly string CalendarSelectorRenderingID = "{4D8BBFBC-9143-4DB7-8382-261064CA531D}";
      public static readonly string customSettingRoot = "{DA5662CC-1D7E-4E2E-9985-09F5B2DCE4C1}";
      public static readonly string DateSelectorRenderingID = "{73C5D583-3D76-4DA0-A263-D8D257D84F09}";
      public static readonly string DayViewRenderingID = "{76C9D153-D740-422F-9503-FDBC09025C5D}";
      public static readonly string defaultPlaceholderName = "content";
      public static readonly string HtmlHeaderRenderingID = "{7D2264C1-67E9-4859-BD54-6DE3FF299038}";
      public static readonly string MiniCalendarRenderingID = "{A285A8E1-1FB3-4E53-ACEE-5F9E79E3E9CC}";
      public static readonly string ModuleSettingsRoot = "{C337C648-8385-4873-8369-6D928EF196F3}";
      public static readonly string MonthViewRenderingID = "{7DD267E1-A5D0-4A65-98C3-170D833FAA9C}";
      public static readonly string ProgressBarRenderingID = "{724C78CE-B424-46D5-8B63-DA6D427172AB}";

      public static readonly string ProgressBarSettingsName = "ProgressBarSettingsPath";
      public static readonly string SelectorSettingsName = "SelectorSettingsPath";

      public static readonly string settingItemFilter =
         "Contains('{0},{1}A87A00B1-E6DB-45AB-8B54-636FEC3B5523{2}', @@templateid)";

      public static readonly string SettingsItemRoot = "{DA5662CC-1D7E-4E2E-9985-09F5B2DCE4C1}";
      public static readonly string siteSettingsPathName = "SiteSettingsPath";
      public static readonly string ViewSettingsName = "ViewSettingsPath";
      public static readonly string viewSettingsPathName = "ViewSettingsPath";
      public static readonly string WeekViewRenderingID = "{966128D4-B7AB-4180-8EA9-5A84A19D909F}";
      protected static Dictionary<string, string> mappingNameParamerts = new Dictionary<string, string>();
      protected static Dictionary<string, string> mappingRenderings = new Dictionary<string, string>();

      protected static Dictionary<string, RenderingSettings> mappingSettings =
         new Dictionary<string, RenderingSettings>();

      protected XmlButton btnCancel;
      protected XmlButton btnEditModuleSettings;
      protected XmlButton btnEditSettingsItem;
      protected XmlButton btnNewModuleSettings;
      protected XmlButton btnNewSettingsItem;
      protected XmlButton btnOk;
      protected HtmlEdit edtItemPlaceholder;
      protected Listbox listRendarings;
      protected GridPanel ManagerPanel;

      private ModuleSettings module;
      protected DataContext ModuleSettingsDataContext;
      protected GridPanel PlaceholderPanel;
      protected DataContext SettingsItemDataContext;
      private string siteSettingsPathDefault = string.Empty;
      protected ControlsTreeList tlControlList;
      protected TreePicker tpModuleSettings;
      protected TreePicker tpSettingsItem;
      protected GridPanel TreeListPanel;
      protected Border SettingsItemBorder;
      protected Border ModuleSettingsBorder;

      public CalendarLayoutManager()
      {
         LoadMapping(null);
         InitParametsName();
      }

      #region Properties

      private IList<RenderingSettings> Renderings
      {
         get
         {
            List<RenderingSettings> list = new List<RenderingSettings>();
            RenderingSettings settings;
            foreach (ListItem item in ListRenderings.Items)
            {
               if (mappingSettings.TryGetValue(item.Value, out settings))
               {
                  settings.ID = item.Value.Split('|')[1];
                  list.Add(settings);
               }
            }
            return list;
         }
      }

      private Listbox ListRenderings
      {
         get
         {
            return tlControlList.ListRenderings;
         }
      }

      private static Item SelectedItem
      {
         get
         {
            return Factory.GetDatabase(WebUtil.GetQueryString("db")).GetItem(WebUtil.GetQueryString("id"));
         }
      }

      #endregion

      #region Protected methods

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         if (TreeListPanel == null)
         {
            TreeListPanel = ManagerPanel.Controls[0] as GridPanel;
         }
         TreeListPanel.ID = GetID("TreeListPanel");

         if (!Context.ClientPage.IsEvent)
         {
            mappingSettings.Clear();
            tlControlList = new ControlsTreeList();
            tlControlList.ID = GetID("TreeList");
            tlControlList.Source = "/sitecore/layout/Renderings/Calendar";
            tlControlList.IncludeTemplatesForDisplay = "Webcontrol,Rendering" +
                                                       (StaticSettings.IsStarterKit ? "" : ",Xsl Rendering");
            tlControlList.IncludeTemplatesForSelection = "Webcontrol,Rendering" +
                                                         (StaticSettings.IsStarterKit ? "" : ",Xsl Rendering");
            tlControlList.AllowMultipleSelection = true;
            tlControlList.PreRender += OnControlsManagerLoad;
            tlControlList.Value = GetSelectedItems();

            TreeListPanel.Controls.Add(tlControlList);

            edtItemPlaceholder = new HtmlEdit();
            edtItemPlaceholder.ID = "etdPlaceholder";
            PlaceholderPanel.Controls.Add(edtItemPlaceholder);

            ModuleSettingsDataContext.DefaultItem = "";
            ModuleSettingsDataContext.Root = "";
            tpModuleSettings.Value = "";

            SettingsItemDataContext.DefaultItem = "";
            SettingsItemDataContext.Root = "";
            tpSettingsItem.Value = "";

            DisableEditMode(true);
         }
         else
         {
            tlControlList = TreeListPanel.FindControl(GetID("TreeList")) as ControlsTreeList;
            edtItemPlaceholder = PlaceholderPanel.FindControl("etdPlaceholder") as HtmlEdit;
         }

         tlControlList.OnListSelectionChanged += OnListSelectionChanged;
         tlControlList.OnSelectedItemChanged += OnSelectItemChanged;

         tpModuleSettings.Load += OnModuleSettingsLoad;
         tpModuleSettings.PreRender += OnModuleSettingsPreRender;
         tpModuleSettings.OnChanged += OnChangeModuleSettings;
         tpSettingsItem.OnChanged += OnChangeSettingsItem;
         tpSettingsItem.Load += OnSettingsSiteLoad;
         tpSettingsItem.PreRender += OnSettingsItemPreRender;
         edtItemPlaceholder.PreRender += OnPlaceholderRender;
         edtItemPlaceholder.OnChanged += OnPlaceholderChange;
         btnEditSettingsItem.OnClick += OnEditSettingsItem;
         btnNewSettingsItem.OnClick += OnNewSettingsItem;
         btnEditModuleSettings.OnClick += OnEditModuleSettings;
         btnNewModuleSettings.OnClick += OnNewModuleSettings;

         btnOk.OnClick += OnOkClick;
         btnCancel.OnClick += OnCancelClick;
      }

      protected virtual void OnControlsManagerLoad(object sender, EventArgs e)
      {
         if (!Context.ClientPage.IsEvent)
         {
            LoadRenderingsNode();

            if (ListRenderings.SelectedItem != null)
            {
               ListRenderings.SelectedItem.Selected = true;
            }
            tlControlList.Value = GetSelectedItems();
         }
      }

      protected virtual void OnOkClick(object sender, EventArgs e)
      {
         IList<RenderingSettings> list = Renderings;

         OnSettingsItemPreRender(this, null);
         OnModuleSettingsPreRender(this, null);

				 //if (IsIncludedHtmlHeader(list) || StaticSettings.IsStarterKit || list.Count == 0)
				 //{
            try
            {
               if (IsOnlyIncludedHtmlHeader(list))
               {
                  ClientDialogs.YesNoCancelMessage(ResourceManager.Localize("REMOVE_ONE_HTMLHEADER"), null, null,
                                                   delegate(string result)
                                                   {
                                                      if (result == "cancel")
                                                      {
                                                         return;
                                                      }
                                                      else if (result == "yes")
                                                      {
                                                         list.Clear();
                                                      }

                                                      SaveRendering(list);
                                                      ExitWindow();
                                                   });
               }
               else
               {
                  SaveRendering(list);
                  ExitWindow();
               }
            }
            catch (UnauthorizedAccessException ex)
            {
               Context.ClientPage.ClientResponse.Alert(ex.Message);
            }
				 //}
				 //else
				 //{
				 //   ClientDialogs.Alert(ResourceManager.Localize("REQUIRES_HTMLHEADER"));
				 //}
      }

      protected virtual void OnCancelClick(object sender, EventArgs e)
      {
         ExitWindow();
      }

      protected virtual void OnNewModuleSettings(object sender, EventArgs e)
      {
         OnSettingsItemPreRender(this, null);
         Item item = StaticSettings.MasterDatabase.GetItem(ID.Parse(customSettingRoot));
         CommandContext context = new CommandContext(item);

         Item itemNew = StaticSettings.MasterDatabase.GetItem(ID.Parse(ModuleSettingsRoot));

         context.Parameters.Add("id", customSettingRoot);
         context.Parameters.Add("master", BranchMapping.GetBrunch(itemNew).ToString());
         context.Parameters.Add("template", itemNew.TemplateID.ToString());

         NewExecute(context);

      }


      protected virtual void OnEditModuleSettings(object sender, EventArgs e)
      {
         Item item = StaticSettings.MasterDatabase.GetItem(ID.Parse(tpModuleSettings.Value));

         UrlString url = new UrlString();
         url.Append("ro", item.ID.ToString());
         url.Append("fo", item.ID.ToString());
         url.Append("id", item.ID.ToString());
         url.Append("la", item.Language.Name);
         url.Append("vs", item.Version.Number.ToString());

         Windows.RunApplication("Content editor", url.ToString());
      }

      protected virtual void OnEditSettingsItem(object sender, EventArgs e)
      {
         OnSettingsItemPreRender(this, null);
         Item item = StaticSettings.MasterDatabase.GetItem(ID.Parse(tpSettingsItem.Value));

         UrlString url = new UrlString();
         url.Append("ro", item.ID.ToString());
         url.Append("fo", item.ID.ToString());
         url.Append("id", item.ID.ToString());
         url.Append("la", item.Language.Name);
         url.Append("vs", item.Version.Number.ToString());

         Windows.RunApplication("Content editor", url.ToString());
      }

      protected virtual void OnSettingsItemPreRender(object sender, EventArgs e)
      {
         if (ListRenderings.SelectedItem == null)
         {
            DisableSettingsItem();
         }
         else
         {
            SettingsItemDataContext.Root = SettingsItemRoot;

            Listbox list = ListRenderings;
            RenderingSettings settings;
            if (mappingSettings.TryGetValue(list.SelectedItem.Value, out settings))
            {
               tpSettingsItem.Value = settings.ViewItemPath;
            }
            else
            {
               tpSettingsItem.Value = string.Empty;
            }

            if (tpSettingsItem.Value != string.Empty)
            {
               Item item = StaticSettings.MasterDatabase.GetItem(tpSettingsItem.Value);
               if (item != null)
               {
                  SettingsItemDataContext.Filter = string.Format(settingItemFilter, item.TemplateID, "{", "}");
                  tpSettingsItem.Value = item.ID.ToString();
               }

               tpSettingsItem.Disabled = false;
               tpSettingsItem.ReadOnly = false;
               btnNewSettingsItem.Disabled = false;
               btnEditSettingsItem.Disabled = false;

               SheerResponse.SetOuterHtml(SettingsItemBorder.ID, SettingsItemBorder);
            }
            else
            {
               DisableSettingsItem();
            }
         }
      }

      protected virtual void OnModuleSettingsPreRender(object sender, EventArgs e)
      {
         if (ListRenderings.SelectedItem == null)
         {
            DisableModuleSettings();
         }
         else
         {
            ModuleSettingsDataContext.Root = ModuleSettingsRoot;

            Listbox list = ListRenderings;
            RenderingSettings settings;
            string value;
            if (mappingRenderings.TryGetValue(list.SelectedItem.Value.Split('|')[1], out value))
            {
               if (value != string.Empty || list.SelectedItem.Value.Split('|')[1] == HtmlHeaderRenderingID)
               {
                  if (mappingSettings.TryGetValue(list.SelectedItem.Value, out settings))
                  {
                     tpModuleSettings.Value = settings.SiteSettingsPath;
                  }
               }
               else
               {
                  tpModuleSettings.Value = string.Empty;
               }
            }
            else
            {
               tpModuleSettings.Value = string.Empty;
            }

            if (tpModuleSettings.Value != string.Empty)
            {
               tpModuleSettings.Disabled = false;
               btnEditModuleSettings.Disabled = false;
               btnNewModuleSettings.Disabled = false;

               SheerResponse.SetOuterHtml(ModuleSettingsBorder.ID, ModuleSettingsBorder);
            }
            else
            {
               DisableModuleSettings();
            }
         }
      }

      protected virtual void OnChangeModuleSettings(object sender, EventArgs e)
      {
         Listbox list = ListRenderings;
         if (list.SelectedItem != null)
         {
            RenderingSettings settings;
            if (mappingSettings.TryGetValue(list.SelectedItem.Value, out settings))
            {
               Item itemDefault = StaticSettings.MasterDatabase.GetItem(settings.SiteSettingsPath);
               Item itemCurrent = StaticSettings.MasterDatabase.GetItem(tpModuleSettings.Value);

               if (itemCurrent != null && itemDefault != null && itemDefault.TemplateID == itemCurrent.TemplateID)
               {
                  LoadMapping(tpModuleSettings.Value);
               }
               else
               {
                  tpModuleSettings.Value = ModuleSettingsRoot;
               }
            }
            else
            {
               tpModuleSettings.Value = ModuleSettingsRoot;
            }

            if (mappingSettings.ContainsKey(list.SelectedItem.Value))
            {
               mappingSettings[list.SelectedItem.Value].SiteSettingsPath = tpModuleSettings.Value;
            }
         }
      }

      protected virtual void OnChangeSettingsItem(object sender, EventArgs e)
      {
         Listbox list = ListRenderings;
         if (list.SelectedItem != null)
         {
            string value;
            mappingRenderings.TryGetValue(list.SelectedItem.Value.Split('|')[1], out value);

            RenderingSettings settings;
            if (mappingSettings.TryGetValue(list.SelectedItem.Value, out settings))
            {
               Item itemDefault = StaticSettings.MasterDatabase.GetItem(settings.ViewItemPath);
               Item itemCurrent = StaticSettings.MasterDatabase.GetItem(tpSettingsItem.Value);

               if (!(itemCurrent != null && itemDefault != null && itemDefault.TemplateID == itemCurrent.TemplateID))
               {
                  tpSettingsItem.Value = value;
               }
            }
            else
            {
               tpSettingsItem.Value = value;
            }

            if (mappingSettings.ContainsKey(list.SelectedItem.Value))
            {
               mappingSettings[list.SelectedItem.Value].ViewItemPath = tpSettingsItem.Value;
            }
         }
      }

      protected virtual void OnSettingsSiteLoad(object sender, EventArgs e)
      {
         if (tlControlList.Value == string.Empty)
         {
            SettingsItemDataContext.Root = "";
            tpSettingsItem.Value = "";
         }
         else
         {
            SettingsItemDataContext.Root = SettingsItemRoot;
         }
      }

      protected virtual void OnModuleSettingsLoad(object sender, EventArgs e)
      {
         if (tlControlList.Value == string.Empty)
         {
            ModuleSettingsDataContext.Root = "";
            tpModuleSettings.Value = "";
         }
         else
         {
            ModuleSettingsDataContext.Root = ModuleSettingsRoot;
         }
      }

      protected virtual void OnPlaceholderRender(object sender, EventArgs e)
      {
         if (tlControlList.Value != string.Empty && ListRenderings.SelectedItem != null)
         {
            RenderingSettings settings;
            if (mappingSettings.TryGetValue(ListRenderings.SelectedItem.Value, out settings))
            {
               edtItemPlaceholder.Value = settings.PlaceholderName;
            }
            else
            {
               edtItemPlaceholder.Value = defaultPlaceholderName;
            }
            edtItemPlaceholder.Disabled = false;
         }
         else
         {
            edtItemPlaceholder.Value = "";
            edtItemPlaceholder.Disabled = true;
         }
      }

      protected virtual void OnPlaceholderChange(object sender, EventArgs e)
      {
         if (ListRenderings.SelectedItem != null && edtItemPlaceholder.Value != string.Empty)
         {
            RenderingSettings settings;
            if (mappingSettings.TryGetValue(ListRenderings.SelectedItem.Value, out settings))
            {
               settings.PlaceholderName = edtItemPlaceholder.Value;
            }
         }
      }

      protected virtual void OnListSelectionChanged(object sender, EventArgs e)
      {
         if (tlControlList.Value == string.Empty)
         {
            DisableEditMode(true);
         }
         else
         {
            DisableEditMode(false);
            Listbox list = ListRenderings;
            list.SelectedItem.Selected = true;

            UpdateRenderingsSettings();
         }
      }

      protected virtual void OnSelectItemChanged(object sender, EventArgs e)
      {
         tpSettingsItem.Value = "";
         UpdateSettingItem();
      }

      #endregion

      #region Xml Format

      private static RenderingDefinition CreateRenderingNode(RenderingSettings setting)
      {
         RenderingDefinition definition = new RenderingDefinition();

         definition.ItemID = setting.ID;
         definition.UniqueId = ID.NewID.ToString();
         definition.Placeholder = setting.PlaceholderName;

         StringBuilder parametrs = new StringBuilder();

         string settingsName;
         if (mappingNameParamerts.TryGetValue(setting.ID, out settingsName) &&
             setting.ViewItemPath != null && setting.ViewItemPath != string.Empty)
         {
            parametrs.Append(settingsName).Append("=");
            parametrs.Append(setting.ViewItemPath);
            parametrs.Append("&");
         }

         parametrs.Append(siteSettingsPathName).Append("=");
         parametrs.Append(setting.SiteSettingsPath);
         parametrs.ToString();

         definition.Parameters = parametrs.ToString();
         return definition;
      }

      private static RenderingDefinition GetHtmlRendering()
      {
         RenderingDefinition definition = new RenderingDefinition();
         definition.ItemID = HtmlHeaderRenderingID;
         definition.UniqueId = ID.NewID.ToString();
         definition.Placeholder = "html-head-meta-data";

         return definition;
      }

      private static void DeleteRenderingNode(DeviceDefinition device)
      {
         ArrayList renderings = new ArrayList();
         foreach (RenderingDefinition rendering in device.Renderings)
         {
            if (!ContainRenderingsID(rendering.ItemID))
            {
               renderings.Add(rendering);
            }
         }

         device.Renderings = renderings;
      }

      private void LoadRenderingsNode()
      {
         Item item = SelectedItem;

				 if (item.Fields[FieldIDs.LayoutField] != null)
         {
					 LayoutDefinition layout =
								 ExistMethodInLayoutField("GetFieldValue") ?
								 LayoutDefinition.Parse(InvokeGetFieldValueMethod(item.Fields[FieldIDs.LayoutField]).ToString()) :
								 LayoutDefinition.Parse(item.Fields[FieldIDs.LayoutField].Value);

					 DeviceDefinition device = layout.GetDevice(StaticSettings.DefaultDeviceID.ToString());

            if (device != null)
            {
               foreach (RenderingDefinition node in device.Renderings)
               {
                  if (!(StaticSettings.IsStarterKit && node.ItemID == HtmlHeaderRenderingID))
                  {
										if (ContainRenderingsID(node.ItemID))
                     {
                        string settingsName;
                        if (!mappingNameParamerts.TryGetValue(node.ItemID, out settingsName))
                        {
                           settingsName = viewSettingsPathName;
                        }

                        NameValueCollection values = node.Parameters != null
                                                        ? WebUtil.ParseUrlParameters(node.Parameters, '&')
                                                        : null;
                        var setting =
                           new RenderingSettings(node.ItemID,
                                                 values == null ? string.Empty : values[settingsName],
                                                 node.Placeholder,
                                                 values == null ? string.Empty : values[siteSettingsPathName]);
                        AddRendering(WebUtil.GetQueryString("db"), setting);
                     }
                  }
               }
            }
         }
      }

      #endregion

			#region Private methods

      private static void SaveRendering(ICollection<RenderingSettings> list)
      {
         Item currentItem = SelectedItem;
				 if (currentItem.Fields[FieldIDs.LayoutField] != null)
         {
					 LayoutDefinition layout =
									ExistMethodInLayoutField("GetFieldValue") ?
									LayoutDefinition.Parse(InvokeGetFieldValueMethod(currentItem.Fields[FieldIDs.LayoutField]).ToString()) :
									LayoutDefinition.Parse(currentItem.Fields[FieldIDs.LayoutField].Value);

         		if (layout != null)
            {
               DeviceDefinition device = layout.GetDevice(StaticSettings.DefaultDeviceID.ToString());

               if (device != null)
               {
               	DeleteRenderingNode(device);

               	if (StaticSettings.IsStarterKit && list.Count > 0)
               	{
               		device.Renderings.Add(GetHtmlRendering());
               	}

               	foreach (RenderingSettings setting in list)
               	{
               		device.Renderings.Add(CreateRenderingNode(setting));
               	}

               	currentItem.Editing.BeginEdit();

								if (ExistMethodInLayoutField("SetFieldValue") && currentItem.Name != "__Standard Values")
               	{
									InvokeSetFieldValueMethods(currentItem.Fields[FieldIDs.LayoutField], layout.ToXml());
               	}
								else
               	{
									currentItem.Fields[FieldIDs.LayoutField].Value = layout.ToXml();
               	}

               	currentItem.Editing.EndEdit();
               }
            }
         }
      }

      private void LoadMapping(string modulePath)
      {
         if (siteSettingsPathDefault != modulePath)
         {
            siteSettingsPathDefault = modulePath;
            module = new ModuleSettings(siteSettingsPathDefault);

            mappingRenderings.Clear();
            mappingRenderings.Add(AgendaViewRenderingID, module.AgendaViewSettings.ID.ToString());
            mappingRenderings.Add(MonthViewRenderingID, module.MonthViewSettings.ID.ToString());
            mappingRenderings.Add(WeekViewRenderingID, module.WeekViewSettings.ID.ToString());
            mappingRenderings.Add(DayViewRenderingID, module.DayViewSettings.ID.ToString());
            mappingRenderings.Add(CalendarSelectorRenderingID, module.CalendarSelectorSettings.ID.ToString());
            mappingRenderings.Add(DateSelectorRenderingID, module.DateSelectorSettings.ID.ToString());
            mappingRenderings.Add(HtmlHeaderRenderingID, string.Empty);
            mappingRenderings.Add(MiniCalendarRenderingID, module.MiniCalendarSettings.ID.ToString());
            mappingRenderings.Add(ProgressBarRenderingID, module.ProgressBarSettings.ID.ToString());
         }
      }

      private string GetID(string id)
      {
         return (ManagerPanel.ID + "_" + id);
      }

      private void DisableEditMode(bool value)
      {
         btnEditModuleSettings.Disabled = value;
         tpModuleSettings.Disabled = value;
      }

      private void DisableModuleSettings()
      {
         ModuleSettingsDataContext.Root = "";
         tpModuleSettings.Value = "";
         tpModuleSettings.Disabled = true;
         btnNewModuleSettings.Disabled = true;
         btnEditModuleSettings.Disabled = true;
      }

      private void DisableSettingsItem()
      {
         SettingsItemDataContext.Root = "";
         tpSettingsItem.Value = "";
         tpSettingsItem.Disabled = true;
         btnEditSettingsItem.Disabled = true;
         btnNewSettingsItem.Disabled = true;
      }

      private void UpdateSettingItem()
      {
         Listbox list = ListRenderings;

         if (list != null && list.Items.Length != 0 && list.SelectedItem != null)
         {
            string value;
            RenderingSettings settings;
            if (mappingRenderings.TryGetValue(list.SelectedItem.Value.Split('|')[1], out value) && value != string.Empty)
            {
               if (tpSettingsItem.Value != string.Empty)
               {
                  Item itemDefault = StaticSettings.MasterDatabase.GetItem(ID.Parse(value));
                  Item itemCurrent = StaticSettings.MasterDatabase.GetItem(tpSettingsItem.Value);

                  if (itemCurrent != null && itemDefault.TemplateID == itemCurrent.TemplateID)
                  {
                     return;
                  }
               }
               if (mappingSettings.TryGetValue(list.SelectedItem.Value, out settings))
               {
                  tpSettingsItem.Value = settings.ViewItemPath;
                  if (StaticSettings.MasterDatabase.GetItem(settings.SiteSettingsPath) != null)
                  {
                     tpModuleSettings.Value = settings.SiteSettingsPath;
                  }
                  else
                  {
                     tpModuleSettings.Value = ModuleSettingsRoot;
                  }
                  LoadMapping(tpModuleSettings.Value);
                  return;
               }

               tpSettingsItem.Value = value;
               tpModuleSettings.Value = ModuleSettingsRoot;
               LoadMapping(tpModuleSettings.Value);
            }
            else
            {
               if (list.SelectedItem.Value.Split('|')[1] == HtmlHeaderRenderingID)
               {
                  if (mappingSettings.TryGetValue(list.SelectedItem.Value, out settings))
                  {
                     if (StaticSettings.MasterDatabase.GetItem(settings.SiteSettingsPath) != null)
                     {
                        tpModuleSettings.Value = settings.SiteSettingsPath;
                     }
                     else
                     {
                        tpModuleSettings.Value = ModuleSettingsRoot;
                     }
                  }
               }
               else
               {
                  tpModuleSettings.Value = "";
               }
               tpSettingsItem.Value = "";
               LoadMapping(tpModuleSettings.Value);
            }
         }
      }

      private void UpdateRenderingsSettings()
      {
         Listbox list = ListRenderings;
         RenderingSettings settings;
         string value;
         foreach (ListItem item in list.Items)
         {
            if (!mappingSettings.TryGetValue(item.Value, out settings))
            {
               if (mappingRenderings.TryGetValue(item.Value.Split('|')[1], out value))
               {
                  mappingSettings.Add(item.Value,
                                      new RenderingSettings(value, defaultPlaceholderName, module.SiteID.ToString()));
               }
            }
         }
      }

      private static string GetSelectedItems()
      {
         StringBuilder values = new StringBuilder();

         foreach (RenderingSettings setting in mappingSettings.Values)
         {
            if (values.Length != 0)
            {
               values.Append("|");
            }
            values.Append(setting.ID);
         }

         return values.ToString();
      }

      private static bool ContainRenderingsID(string key)
      {
         return mappingRenderings.ContainsKey(key);
      }

      private void AddRendering(string databaseName, RenderingSettings settings)
      {
         if (StaticSettings.IsStarterKit && settings.ID == HtmlHeaderRenderingID)
         {
            return;
         }

         if (settings.ViewItemPath == null || settings.ViewItemPath == string.Empty ||
             StaticSettings.MasterDatabase.GetItem(settings.ViewItemPath) == null)
         {
            string value;
            if (mappingRenderings.TryGetValue(settings.ID, out value))
            {
               settings.ViewItemPath = value;
            }
         }

         if (settings.SiteSettingsPath == null || settings.SiteSettingsPath == string.Empty ||
             StaticSettings.MasterDatabase.GetItem(settings.SiteSettingsPath) == null)
         {
            settings.SiteSettingsPath = ModuleSettingsRoot;
         }

         ListItem child = new ListItem();

         child.ID = Control.GetUniqueID("I");
         ListRenderings.Controls.Add(child);

         Item item = Factory.GetDatabase(databaseName).Items[settings.ID];

         child.Value = child.ID + "|" + settings.ID;
         child.Header = item.DisplayName;

         mappingSettings.Add(child.Value, settings);
      }

      private static void InitParametsName()
      {
         mappingNameParamerts.Clear();
         mappingNameParamerts.Add(AgendaViewRenderingID, ViewSettingsName);
         mappingNameParamerts.Add(MonthViewRenderingID, ViewSettingsName);
         mappingNameParamerts.Add(WeekViewRenderingID, ViewSettingsName);
         mappingNameParamerts.Add(DayViewRenderingID, ViewSettingsName);
         mappingNameParamerts.Add(CalendarSelectorRenderingID, SelectorSettingsName);
         mappingNameParamerts.Add(MiniCalendarRenderingID, SelectorSettingsName);
         mappingNameParamerts.Add(DateSelectorRenderingID, SelectorSettingsName);
         mappingNameParamerts.Add(ProgressBarRenderingID, ProgressBarSettingsName);
      }

      private static bool IsIncludedHtmlHeader(IEnumerable<RenderingSettings> list)
      {
         foreach (RenderingSettings settings in list)
         {
            if (settings.ID == HtmlHeaderRenderingID)
            {
               return true;
            }
         }

         return false;
      }

      private static bool IsOnlyIncludedHtmlHeader(ICollection<RenderingSettings> list)
      {
         if (list.Count == 0)
         {
            return false;
         }
         foreach (RenderingSettings settings in list)
         {
            if (settings.ID != HtmlHeaderRenderingID)
            {
               return false;
            }
         }

         return true;
      }

      #endregion

			#region Reflection static methods

			private static bool ExistMethodInLayoutField(String methodName)
			{
				Type type = typeof(LayoutField);

				foreach (MethodInfo method in type.GetMethods())
				{
					if (method.Name == methodName) return true;
				}

				return false;
			}

			private static object InvokeGetFieldValueMethod(Field field)
			{
				object result = null;

				Assembly assemblyInstance = Assembly.Load("Sitecore.Kernel");

				if (assemblyInstance != null)
				{
					Type layoutFieldClass = assemblyInstance.GetType("Sitecore.Data.Fields.LayoutField", false, true);

					if (layoutFieldClass != null)
					{
						MethodInfo getFieldValueMethod = layoutFieldClass.GetMethod("GetFieldValue");

						if (getFieldValueMethod != null)
						{
							result = getFieldValueMethod.Invoke(null, new object[] { field });
						}
					}
				}

				return result;
			}

		 private static void InvokeSetFieldValueMethods(Field field, String value)
		 {
			 Assembly assemblyInstance = Assembly.Load("Sitecore.Kernel");

			 if (assemblyInstance != null)
			 {
				 Type layoutFieldClass = assemblyInstance.GetType("Sitecore.Data.Fields.LayoutField", false, true);

				 if (layoutFieldClass != null)
				 {
					 MethodInfo setFieldValueMethod = layoutFieldClass.GetMethod("SetFieldValue");

					 if (setFieldValueMethod != null)
					 {
						 setFieldValueMethod.Invoke(null, new object[] { field, value });
					 }
				 }
			 }
		 }

			#endregion

			#region handlers

			public void OnNewSettingsItem(object sender, EventArgs e)
      {
         OnSettingsItemPreRender(this, null);
         Item item = StaticSettings.MasterDatabase.GetItem(customSettingRoot);
         CommandContext context = new CommandContext(item);

         Item itemNew = StaticSettings.MasterDatabase.GetItem(tpSettingsItem.Value);

         context.Parameters.Add("id", customSettingRoot);
         context.Parameters.Add("master", BranchMapping.GetBrunch(itemNew).ToString());
         context.Parameters.Add("template", itemNew.TemplateID.ToString());

         NewExecute(context);
      }

      protected void RunPipelineNew(ClientPipelineArgs args)
      {
         string strMaster = StringUtil.GetString(new string[] { args.Parameters["master"] });
         string strTemplate = StringUtil.GetString(new string[] { args.Parameters["template"] });
         Database database = Factory.GetDatabase(StringUtil.GetString(new string[] { args.Parameters["database"] }));
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
                  new string[] { args.Parameters["prompt"], ResourceManager.Localize("ENTER_ITEM_NAME") });
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

      public void NewExecute(CommandContext context)
      {
         if (context.Items.Length == 1)
         {
            Item item = context.Items[0];
            if (!item.Access.CanCreate())
            {
               SheerResponse.Alert("You do not have permission to create a new item here.", new string[0]);
            }
            else
            {
               string str = StringUtil.GetString(new string[] { context.Parameters["template"] });
               string str2 = StringUtil.GetString(new string[] { context.Parameters["master"] });
               string str3 = StringUtil.GetString(new string[] { context.Parameters["prompt"] });
               NameValueCollection parameters = new NameValueCollection();
               BranchItem item2 = null;
               TemplateItem item3 = null;
               if (str2.Length > 0)
               {
                  item2 = Context.ContentDatabase.Branches[str2];
                  Error.Assert(item2 != null, "Master \"" + str2 + "\" not found.");
               }
               else if (str.Length > 0)
               {
                  item3 = Context.ContentDatabase.Templates[str];
                  Error.Assert(item3 != null, "Template \"" + str + "\" not found.");
               }
               if ((item2 != null) || (item3 != null))
               {
                  parameters["prompt"] = str3;
                  parameters["id"] = item.ID.ToString();
                  parameters["database"] = item.Database.Name;
                  if (item2 != null)
                  {
                     parameters["master"] = item2.ID.ToString();
                  }
                  if (item3 != null)
                  {
                     parameters["template"] = item3.ID.ToString();
                  }
                  Context.ClientPage.Start(this, "RunPipelineNew", parameters);
               }
            }
         }
      }

      protected void PostStep(Item item)
      {
         Assert.ArgumentNotNull(item, "item");

         if (item.TemplateID == CalendarIDs.ModuleSettingsTemplate)
         {
            tpModuleSettings.Value = item.ID.ToString();
            OnChangeModuleSettings(this, null);
         }
         else
         {
            tpSettingsItem.Value = item.ID.ToString();
            OnChangeSettingsItem(this, null);
         }

         Context.ClientPage.ClientResponse.Redraw();
         

         UrlString url = new UrlString();
         url.Append("ro", item.ID.ToString());
         url.Append("fo", item.ID.ToString());
         url.Append("id", item.ID.ToString());
         url.Append("la", item.Language.Name);
         url.Append("vs", item.Version.Number.ToString());
         Windows.RunApplication("Content editor", url.GetUrl());
      }

      #endregion handlers
   }
}