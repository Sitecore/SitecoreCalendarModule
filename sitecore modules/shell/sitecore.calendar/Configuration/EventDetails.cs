using System;

using Sitecore.Data.Items;
using Sitecore.Modules.EventCalendar.Objects;
using Sitecore.Modules.EventCalendar.Utils;

namespace Sitecore.Modules.EventCalendar.Configuration
{
   public class EventDetails
   {
      private BranchItem branch = StaticSettings.EventBranch;
      private DateTime date;
      private string defaultDescription;
      private string defaultLocation;
      private string defaultTitle;
      private string description;
      private int descriptionCount;
      private string endTime;
      private EventList list;

      private string location;
      private int locationCount;

      private string startTime;
      private string title;
      private int titleCount;

      public BranchItem Branch
      {
         get
         {
            return branch;
         }
         set
         {
            branch = value;
         }
      }

      public EventList List
      {
         get
         {
            return list;
         }
         set
         {
            list = value;
         }
      }

      public string Title
      {
         get
         {
            return title;
         }
         set
         {
            title = value;
         }
      }

      public int TitleCount
      {
         get
         {
            return titleCount;
         }
         set
         {
            titleCount = value;
         }
      }

      public string Location
      {
         get
         {
            return location;
         }
         set
         {
            location = value;
         }
      }

      public int LocationCount
      {
         get
         {
            return locationCount;
         }
         set
         {
            locationCount = value;
         }
      }

      public string Description
      {
         get
         {
            return description;
         }
         set
         {
            description = value;
         }
      }

      public int DescriptionCount
      {
         get
         {
            return descriptionCount;
         }
         set
         {
            descriptionCount = value;
         }
      }

      public DateTime Date
      {
         get
         {
            return date;
         }
         set
         {
            date = value;
         }
      }

      public string StartTime
      {
         get
         {
            return startTime;
         }
         set
         {
            startTime = value;
         }
      }

      public string EndTime
      {
         get
         {
            return endTime;
         }
         set
         {
            endTime = value;
         }
      }

      public string DefaultTitle
      {
         get
         {
            return defaultTitle;
         }
         set
         {
            defaultTitle = value;
         }
      }

      public string DefaultLocation
      {
         get
         {
            return defaultLocation;
         }
         set
         {
            defaultLocation = value;
         }
      }

      public string DefaultDescription
      {
         get
         {
            return defaultDescription;
         }
         set
         {
            defaultDescription = value;
         }
      }
   }
}