using System;
using System.Xml.Serialization;

using Sitecore.Xml.Serialization;

namespace Sitecore.Modules.EventCalendar.Logic.Utils
{
   [XmlRoot("options")]
   [Serializable]
   public class Options : XmlSerializable
   {
      #region Properties

      [XmlAttribute("siteSettingsPath")]
      public string SiteSettingsPath { get; set; }

      [XmlAttribute("controlSettingsPath")]
      public string ControlSettingsPath { get; set; }

      [XmlAttribute("calendarID")]
      public string CalendarID { get; set; }

      [XmlAttribute("date")]
      public string Date { get; set; }

      [XmlAttribute("offset")]
      public int Offset { get; set; }

      #endregion

      #region Methods

      public static Options Parse(string xml)
      {
         return (Options) LoadXml(xml, typeof (Options));
      }

      #endregion
   }
}