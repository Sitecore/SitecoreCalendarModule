using System;
using System.Xml.Serialization;

using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.EventCalendar.Core.Configuration;
using Sitecore.Modules.EventCalendar.Exceptions;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Xml.Serialization;

namespace Sitecore.Modules.EventCalendar.Objects
{
   [Serializable]
   [XmlRoot("event")]
   public class Event : XmlSerializable
   {
      #region Constants

      [XmlIgnore]
      public static readonly string DescriptionField = "Description";
      [XmlIgnore]
      public static readonly string EndDateField = "End Date";
      [XmlIgnore]
      public static readonly string EndTimeField = "End Time";
      [XmlIgnore]
      public static readonly string LocationField = "Location";
      [XmlIgnore]
      public static readonly string RelatedItemField = "Related Item";
      [XmlIgnore]
      public static readonly string ScheduleIDField = "ScheduleID";
      [XmlIgnore]
      public static readonly string StartDateField = "Start Date";
      [XmlIgnore]
      public static readonly string StartTimeField = "Start Time";
      [XmlIgnore]
      public static readonly string TitleField = "Title";

      #endregion

      #region Private Variables

      [XmlIgnore]
      private string _endTime = string.Empty;

      [XmlIgnore]
      private Item _item;

      #endregion

      #region Initialization Methods

      public Event()
      {
         Path = string.Empty;
         Description = String.Empty;
         Location = string.Empty;
         Name = string.Empty;
         Title = String.Empty;
         StartTime = string.Empty;
         ReadOnly = false;
         EndDate = string.Empty;
         StartDate = string.Empty;
         ScheduleID = string.Empty;
         ID = Data.ID.NewID.ToString();
      }

      public Event(ID eventID)
      {
         Path = string.Empty;
         Description = String.Empty;
         Location = string.Empty;
         Name = string.Empty;
         Title = String.Empty;
         StartTime = string.Empty;
         ReadOnly = false;
         EndDate = string.Empty;
         StartDate = string.Empty;
         ScheduleID = string.Empty;
         ID = Data.ID.Null.ToString();
         Item eventItem = StaticSettings.EventSourceDatabase.GetItem(eventID);

         if (eventItem != null)
         {
            if (eventItem.TemplateID != StaticSettings.EventTemplate.ID)
            {
               throw new EventCalendarException(
                  String.Format(ResourceManager.Localize("UNSUPPORT_TEMPLATE"), eventItem.Name,
                                eventItem.TemplateName, StaticSettings.EventTemplate.Name));
            }

            Title = eventItem.Fields[TitleField].Value;
            Description = eventItem.Fields[DescriptionField].Value;
            ID = eventID.ToString();
            StartTime = Utilities.NormalizeTime(eventItem.Fields[StartTimeField].Value);
            _endTime = Utilities.NormalizeTime(eventItem.Fields[EndTimeField].Value);
            StartDate =
               Utilities.NormalizeDate(
                  DateUtil.IsoDateToDateTime(eventItem.Fields[StartDateField].Value));
            EndDate =
               Utilities.NormalizeDate(
                  DateUtil.IsoDateToDateTime(eventItem.Fields[EndDateField].Value));
            ScheduleID = eventItem.Fields[ScheduleIDField].Value;
            Location = eventItem.Fields[LocationField].Value;

            ReadOnly = !SecurityManager.CanWrite(eventItem);
            _item = eventItem;
            Name = eventItem.Name;

            LinkField relItemField = eventItem.Fields[RelatedItemField];
            if (relItemField != null)
            {
               Item relItem = relItemField.TargetItem;
               if (relItem != null)
               {
                  Path = Utilities.GetFriendlyURL(relItem) + "?id=" + eventID;
               }
            }

            LoadAdditionalAttributes(eventItem);
         }
         else
         {
            throw new EventCalendarException(
               String.Format(ResourceManager.Localize("COULD_NOT_FIND_EVENT"), eventID));
         }
      }

      public Event(Item eventItem)
         : this(eventItem.ID)
      {
      }

      #endregion

      #region Properties

      [XmlIgnore]
      internal bool IsVisible
      {
         get
         {
            return _item != null && SecurityManager.CanRead(_item);
         }
      }

      [XmlIgnore]
      public bool IsRecurrentEvent
      {
         get
         {
            if (string.IsNullOrEmpty(ScheduleID) || Data.ID.Parse(ScheduleID).IsNull ||
                StaticSettings.EventSourceDatabase.GetItem(ScheduleID) == null)
            {
               return false;
            }

            return true;
         }
      }

      [XmlIgnore]
      public bool IsChanged
      {
         get
         {
            var tmpEvent = new Event(Data.ID.Parse(ID));

            if (tmpEvent.Title != Title || tmpEvent.Description != Description ||
                tmpEvent.StartDate != StartDate || IsDateScopeChanged ||
                tmpEvent.Location != Location)
            {
               return true;
            }

            return false;
         }
      }

      [XmlIgnore]
      public bool IsDateScopeChanged
      {
         get
         {
            var tmpEvent = new Event(Data.ID.Parse(ID));

            if (tmpEvent.StartDate != StartDate || tmpEvent.EndDate != EndDate)
            {
               return true;
            }

            return false;
         }
      }

      [XmlAttribute("attributes")]
      public string[] Attributes { get; set; }

      [XmlAttribute("id")]
      public string ID { get; set; }

      [XmlAttribute("scheduleID")]
      public string ScheduleID { get; set; }

      [XmlAttribute("startDate")]
      public string StartDate { get; set; }

      [XmlAttribute("endDate")]
      public string EndDate { get; set; }

      [XmlAttribute("readOnly")]
      public bool ReadOnly { get; set; }

      [XmlAttribute("startTime")]
      public string StartTime { get; set; }

      [XmlAttribute("endTime")]
      public string EndTime
      {
         get
         {
            if (Utilities.StringToDate(StartDate) < Utilities.StringToDate(EndDate) &&
                TimeSpan.Parse(StartTime) > TimeSpan.Parse(_endTime))
            {
               return "23:30";
            }
            return _endTime;
         }
         set
         {
            _endTime = value;
         }
      }

      [XmlAttribute("title")]
      public string Title { get; set; }

      [XmlAttribute("name")]
      public string Name { get; set; }

      [XmlAttribute("location")]
      public string Location { get; set; }

      [XmlAttribute("description")]
      public string Description { get; set; }

      [XmlAttribute("path")]
      public string Path { get; set; }

      #endregion

      #region Methods

      private void LoadAdditionalAttributes(BaseItem evenItem)
      {
         string[] attributes = StaticSettings.AdditionalFields;
         if (attributes.Length > 0)
         {
            Attributes = new string[2 * attributes.Length];
            int pos = 0;
            for (int i = 0; i < attributes.Length; ++i)
            {
               if (!string.IsNullOrEmpty(attributes[i]))
               {
                  Attributes[pos++] = attributes[i];
                  Attributes[pos++] = evenItem.Fields[attributes[i]] != null
                                         ? evenItem.Fields[attributes[i]].Value : "";
               }
            }
         }
      }

      private void SaveAdditionalAttributes(BaseItem evenItem)
      {
         int i = 0;

         if (Attributes != null)
         {
            while (i < Attributes.Length)
            {
               string FieldName = Attributes[i++];
               if (i < Attributes.Length)
               {
                  if (evenItem.Fields[FieldName] != null)
                  {
                     evenItem.Fields[FieldName].Value = Attributes[i++];
                  }
               }
            }
         }
      }

      public static void Delete(Item item)
      {
         item.Delete();
      }

      public void Save()
      {
         SaveToItem(GetTargetItem(), false);
      }

      public void SaveToItem(Item eventItem, bool seriesUpdate)
      {
         if (eventItem == null)
         {
            return;
         }

         eventItem.Editing.BeginEdit();
         eventItem.Fields[TitleField].Value = Title.Trim(' ');
         eventItem.Fields[DescriptionField].Value = Description;
         eventItem.Fields[StartTimeField].Value = Utilities.NormalizeTime(StartTime);
         eventItem.Fields[EndTimeField].Value = Utilities.NormalizeTime(_endTime);
         eventItem.Fields[LocationField].Value = Location;

         if (seriesUpdate)
         {
            return;
         }

         eventItem.Fields[StartDateField].Value = Utilities.StringToIsoDate(StartDate);
         eventItem.Fields[EndDateField].Value = Utilities.StringToIsoDate(EndDate);

         if (string.IsNullOrEmpty(ScheduleID))
         {
            eventItem.Fields[ScheduleIDField].Value = string.Empty;
         }
         else
         {
            eventItem.Fields[ScheduleIDField].Value = ScheduleID;
         }

         SaveAdditionalAttributes(eventItem);
         eventItem.Editing.EndEdit();
      }

      public Item GetTargetItem()
      {
         if (Data.ID.IsID(ID))
         {
            _item = StaticSettings.EventTargetDatabase.GetItem(ID);
         }
         return _item;
      }

      public Item GetItem()
      {
         if (Data.ID.IsID(ID))
         {
            _item = StaticSettings.EventSourceDatabase.GetItem(ID);
         }
         return _item;
      }

      public Schedule GetSchedule()
      {
         if (Data.ID.IsID(ScheduleID))
         {
            return new Schedule(ScheduleID);
         }
         return null;
      }

      public static Event Parse(string xml)
      {
         return (Event)LoadXml(xml, typeof(Event));
      }

      #endregion
   }
}