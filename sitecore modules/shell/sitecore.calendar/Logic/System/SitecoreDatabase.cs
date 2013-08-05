using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Modules.EventCalendar
{
   /// <summary>
   /// Class for encapsulating Sitecore Database access
   /// </summary>
   public class SitecoreDatabase
   {
      private Database _database;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="database">A Sitecore Database</param>
      public SitecoreDatabase(Database database)
      {
         _database = database;
      }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="database">A Sitecore Database name</param>
      public SitecoreDatabase(string database)
      {
         _database = Factory.GetDatabase(database);
      }

      /// <summary>
      /// Get the current Database
      /// </summary>
      public Database Database
      {
         get
         {
            return _database;
         }
         set
         {
            _database = value;
         }
      }

      /// <summary>
      /// Get a Sitecore item from current Database by ID
      /// </summary>
      /// <param name="id">Sitecore ID</param>
      /// <returns>Sitecore.Data.Items.Item</returns>
      public Item GetItem(ID id)
      {
         return Database.Items.GetItem(id);
      }

      /// <summary>
      /// Get a Sitecore item from current Database by Path
      /// </summary>
      /// <param name="path">Sitecore Path</param>
      /// <returns>Sitecore.Data.Items.Item</returns>
      public Item SelectSingleItem(string path)
      {
         return path != null ? Database.SelectSingleItem(path) : null;
      }

      /// <summary>
      /// Get a Sitecore item from current Database by Path
      /// </summary>
      /// <param name="path">Sitecore Path</param>
      /// <returns>Sitecore.Data.Items.Item</returns>
      public Item GetItem(string path)
      {
         return path != null ? Database.GetItem(path) : null;
      }
   }
}