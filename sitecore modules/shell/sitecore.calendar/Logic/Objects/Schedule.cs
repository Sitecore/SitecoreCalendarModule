using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EventCalendar.Utils;
using Sitecore.Xml.Serialization;

using DaysCollection = Sitecore.Modules.EventCalendar.Utils.DaysOfWeek;

namespace Sitecore.Modules.EventCalendar.Objects
{
   [Serializable]
   [XmlRoot("schedule")]
   public class Schedule : XmlSerializable
   {
      #region Constants

      [XmlIgnore]
      public static readonly string EndDateField = "End Date";
      [XmlIgnore]
      public static readonly string RecurDaily = "Daily";
      [XmlIgnore]
      public static readonly string RecurDaysOfWeekField = "RecurDaysOfWeek";
      [XmlIgnore]
      public static readonly string RecurEveryNField = "RecurEveryN";
      [XmlIgnore]
      public static readonly string RecurMonthly = "Monthly";
      [XmlIgnore]
      public static readonly string RecurMonthNameField = "RecurMonthName";
      [XmlIgnore]
      public static readonly string RecurOnce = "Once";
      [XmlIgnore]
      public static readonly string RecurrenceField = "Recurrence";
      [XmlIgnore]
      public static readonly string RecurSeqOrdinalField = "RecurEveryNSeqOrdinal";
      [XmlIgnore]
      public static readonly string RecurWeekly = "Weekly";
      [XmlIgnore]
      public static readonly string RecurYearly = "Yearly";
      [XmlIgnore]
      public static readonly string StartDateField = "Start Date";

      #endregion Constants

      #region Private Variables

      [XmlIgnore]
      private string _endDate = string.Empty;

      [XmlIgnore]
      private string _startDate = string.Empty;

      [XmlIgnore]
      private Item _sourceItem;

      #endregion Private Variables

      #region Accessor Methods

      [XmlAttribute("id")]
      public string ID { get; set; }

      [XmlAttribute("month")]
      public Month Month { get; set; }

      [XmlAttribute("daysCollection")]
      public DaysCollection DaysOfWeek { get; set; }

      [XmlAttribute("sequence")]
      public Sequence Sequence { get; set; }

      [XmlAttribute("frequency")]
      public int Frequency { get; set; }

      [XmlAttribute("recurrence")]
      public Recurrence Recurrence { get; set; }

      [XmlAttribute("startDate")]
      public string StartDate
      {
         get
         {
            if (string.IsNullOrEmpty(_startDate) && IsNew == false)
            {
               Item tmp = GetSourceItem();
               if (tmp != null)
               {
                  _startDate = Utilities.NormalizeDate(DateUtil.IsoDateToDateTime(tmp.Fields[StartDateField].Value));
               }
            }

            return _startDate;
         }
         set
         {
            _startDate = value;
         }
      }

      [XmlAttribute("endDate")]
      public string EndDate
      {
         get
         {
            if (string.IsNullOrEmpty(_endDate) && IsNew == false)
            {
               Item tmp = GetSourceItem();
               if (tmp != null)
               {
                  _endDate = Utilities.NormalizeDate(DateUtil.IsoDateToDateTime(tmp.Fields[EndDateField].Value));
               }
            }

            return _endDate;
         }
         set
         {
            _endDate = value;
         }
      }

      [XmlIgnore]
      public bool IsNew
      {
         get
         {
            return StaticSettings.EventSourceDatabase.GetItem(Data.ID.Parse(ID)) == null;
         }
      }

      [XmlIgnore]
      public bool IsChanged
      {
         get
         {
            if (IsNew)
            {
               return true;
            }

            var tmpSchedule = new Schedule(Data.ID.Parse(ID));

            if (tmpSchedule.DaysOfWeek != DaysOfWeek ||
                tmpSchedule.Frequency != Frequency || tmpSchedule.Month != Month || tmpSchedule.Recurrence != Recurrence ||
                tmpSchedule.Sequence != Sequence ||
                tmpSchedule.StartDate != StartDate || tmpSchedule.EndDate != EndDate)
            {
               return true;
            }

            return false;
         }
      }

      #endregion Accessor Methods

      #region Initialization Methods

      public Schedule()
      {
         Frequency = 0;
         Recurrence = Recurrence.Once;
         Sequence = Sequence.None;
         DaysOfWeek = DaysCollection.None;
         Month = Month.None;
         ID = Data.ID.NewID.ToString();
      }

      public Schedule(string id)
         : this(Data.ID.Parse(id))
      {
         Assert.ArgumentNotNullOrEmpty(id, id);
         Assert.IsTrue(Data.ID.IsID(id), "the value is not id");
      }

      public Schedule(ID id)
      {
         Frequency = 0;
         Recurrence = Recurrence.Once;
         Sequence = Sequence.None;
         DaysOfWeek = DaysCollection.None;
         Month = Month.None;
         ID = id.ToString();
         _sourceItem = StaticSettings.EventSourceDatabase.Database.Items[id];
         loadRecurrence(_sourceItem);
      }

      private void loadRecurrence(Item scItem)
      {
         if (scItem == null)
         {
            return;
         }

         int value;
         int.TryParse(scItem.Fields[RecurEveryNField].Value, out value);
         Frequency = value;

         Field fld = scItem.Fields[RecurrenceField];
         if (fld.Value == RecurDaily)
         {
            Recurrence = Recurrence.Daily;
            return;
         }

         if (fld.Value == RecurWeekly)
         {
            Recurrence = Recurrence.Weekly;

            loadDaysOfWeek(scItem);

            return;
         }

         if (fld.Value == RecurMonthly)
         {
            Recurrence = Recurrence.Monthly;

            Sequence = (Sequence)Enum.Parse(typeof(Sequence), scItem[RecurSeqOrdinalField]);
            loadDaysOfWeek(scItem);

            return;
         }

         if (fld.Value == RecurYearly)
         {
            Recurrence = Recurrence.Yearly;
            Month = (Month)Enum.Parse(typeof(Month), scItem[RecurMonthNameField]);

            return;
         }

         Recurrence = Recurrence.Once;
      }

      #endregion Initialization Methods

      #region Methods

      public Item GetSourceItem()
      {
         if (_sourceItem == null)
         {
            _sourceItem = StaticSettings.EventSourceDatabase.Database.Items[Data.ID.Parse(ID)];
         }

         return _sourceItem;
      }

      public Item GetTargetItem()
      {
         return StaticSettings.EventTargetDatabase.Database.Items[Data.ID.Parse(ID)];
      }

      public IEnumerable<Event> GetTargetEvents()
      {
         var events = new List<Event>();
         var links = Globals.LinkDatabase.GetReferrers(GetTargetItem());
         foreach (var link in links)
         {
            events.Add(new Event(link.SourceItemID));
         }

         return events;
      }

      public void SaveRecurrence(Item schedule)
      {
         if (schedule == null)
         {
            return;
         }

         using (new EditContext(schedule))
         {
            if (IsNew)
            {
               schedule.Fields[StartDateField].Value = Utilities.StringToIsoDate(_startDate);
               schedule.Fields[EndDateField].Value = Utilities.StringToIsoDate(_endDate);
            }

            switch (Recurrence)
            {
               case Recurrence.Daily:
                  schedule[RecurrenceField] = RecurDaily;
                  break;

               case Recurrence.Monthly:
                  schedule[RecurrenceField] = RecurMonthly;
                  saveDaysOfWeek(schedule);
                  schedule[RecurSeqOrdinalField] = Sequence.ToString();
                  break;

               case Recurrence.Weekly:
                  schedule[RecurrenceField] = RecurWeekly;
                  saveDaysOfWeek(schedule);
                  break;

               case Recurrence.Yearly:
                  schedule[RecurrenceField] = RecurYearly;
                  schedule[RecurMonthNameField] = Month.ToString();
                  break;

               default:
                  schedule[RecurrenceField] = RecurOnce;
                  break;
            }

            if (Recurrence != Recurrence.Once)
            {
               schedule[RecurEveryNField] = Frequency.ToString();
            }
         }
      }

      private void loadDaysOfWeek(BaseItem scItem)
      {
         if (scItem != null)
         {
            string daysList = scItem[RecurDaysOfWeekField];

            if (!string.IsNullOrEmpty(daysList))
            {
               var days = daysList.Split('|');

               DaysOfWeek = DaysCollection.None;

               foreach (var day in days)
               {
                  if (day != string.Empty)
                  {
                     DaysOfWeek |= (DaysCollection)Enum.Parse(typeof(DaysCollection), day);
                  }
               }
            }
         }
      }

      private void saveDaysOfWeek(BaseItem scItem)
      {
         if (scItem == null)
         {
            return;
         }

         string days = string.Empty;

         if ((DaysOfWeek & DaysCollection.Monday) == DaysCollection.Monday)
         {
            days += DaysCollection.Monday + "|";
         }

         if ((DaysOfWeek & DaysCollection.Tuesday) == DaysCollection.Tuesday)
         {
            days += DaysCollection.Tuesday + "|";
         }

         if ((DaysOfWeek & DaysCollection.Wednesday) == DaysCollection.Wednesday)
         {
            days += DaysCollection.Wednesday + "|";
         }

         if ((DaysOfWeek & DaysCollection.Thursday) == DaysCollection.Thursday)
         {
            days += DaysCollection.Thursday + "|";
         }

         if ((DaysOfWeek & DaysCollection.Friday) == DaysCollection.Friday)
         {
            days += DaysCollection.Friday + "|";
         }

         if ((DaysOfWeek & DaysCollection.Saturday) == DaysCollection.Saturday)
         {
            days += DaysCollection.Saturday + "|";
         }

         if ((DaysOfWeek & DaysCollection.Sunday) == DaysCollection.Sunday)
         {
            days += DaysCollection.Sunday.ToString();
         }

         scItem[RecurDaysOfWeekField] = days;
      }

      #endregion Methods

      #region Static Methods

      public static Schedule Parse(string xml)
      {
         if (string.IsNullOrEmpty(xml))
         {
            return null;
         }

         try
         {
            return (Schedule)LoadXml(xml, typeof(Schedule));
         }
         catch (Exception)
         {
            return null;
         }
      }

      #endregion

   }
}