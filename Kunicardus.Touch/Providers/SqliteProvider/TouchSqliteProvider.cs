using System;
using SQLite;
using Kunicardus.Core.Providers.LocalDBProvider;
using Foundation;
using UIKit;
using System.IO;
using Kunicardus.Core.Models.DB;
using System.Collections.Generic;
using System.Linq;
using Kunicardus.Core.Models;

namespace Kunicardus.Touch.Providers.SqliteProvider
{
	public class TouchSqliteProvider : SQLiteConnection, ILocalDbProvider
	{
		
		#region Props & Vars

		static object locker = new object ();

		private static string SystemDocumentsPath ()
		{
			int SystemVersion = Convert.ToInt16 (UIDevice.CurrentDevice.SystemVersion.Split ('.') [0]);
			if (SystemVersion >= 8) {
				return NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].Path;
			}
			return Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
		}

		public static string DBPath {
			get {
				return Path.Combine (SystemDocumentsPath (), "unicard.db");
			}
		}

		#endregion

		#region Ctors

		public TouchSqliteProvider () : base (DBPath)
		{
			CreateTable<UserInfo> ();
			CreateTable<NewsInfo> ();
			CreateTable<MerchantInfo> ();
			CreateTable<TransactionInfo> ();
			CreateTable<UserTypesInfo> ();
			CreateTable<ProductCategoryInfo> ();
			CreateTable<ProductsInfo> ();
			CreateTable<DeliveryMethod> ();
			CreateTable<SettingsInfo> ();
			CreateTable<AutoCompleteFields> ();
			CreateTable<OrganizationModel> ();
            CreateTable<VersionsModel>();
        }

		#endregion

		#region ILocalDbProvider implementation

		public new System.Collections.Generic.List<T> Get<T> (System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Kunicardus.Core.Models.DB.DBModel, new()
		{
			lock (locker) {
				return base.Get<List<T>> (predicate);
			}
		}

		public System.Collections.Generic.List<T> Get<T> () where T : Kunicardus.Core.Models.DB.DBModel, new()
		{
			lock (locker) {
				return base.Table<T> ().ToList ();
			}
		}

		public void Insert<T> (T entity) where T : Kunicardus.Core.Models.DB.DBModel
		{
			lock (locker) {
				base.Insert (entity);
			}
		}

		public void Insert<T> (System.Collections.Generic.IEnumerable<T> entities) where T : Kunicardus.Core.Models.DB.DBModel
		{
			lock (locker) {
				base.InsertAll (entities);
			}
		}

		public void Update<T> (T entity) where T : Kunicardus.Core.Models.DB.DBModel
		{
			lock (locker) {
				base.Update (entity, typeof(T));
			}
		}

		public void Update<T> (System.Collections.Generic.IEnumerable<T> entities) where T : Kunicardus.Core.Models.DB.DBModel
		{
			lock (locker) {
				base.UpdateAll (entities);
			}
		}

		public void Delete<T> (T entity) where T : Kunicardus.Core.Models.DB.DBModel
		{
			lock (locker) {
				base.Delete (entity);
			}
		}

		public int GetCount<T> () where T : new()
		{
			lock (locker) {
				return base.Table<T> ().Count ();
			}
		}

		public int GetCount<T> (System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : new()
		{
			lock (locker) {
				return base.Get<List<T>> (predicate).Count;
			}
		}

		public bool Any<T> (System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Kunicardus.Core.Models.DB.DBModel, new()
		{
			//return base.Table<T> ().A
			throw new NotImplementedException ();
		}

		public new System.Collections.Generic.List<T> Query<T> (string query, params object[] args) where T : new()
		{
			lock (locker) {
				return base.Query<T> (query, args);
			}
		}

		public void ExecuteScalar<T> (string query) where T : class
		{
			lock (locker) {
				base.ExecuteScalar<T> (query);
			}
		}

		public void Execute (string query)
		{
			lock (locker) {
				base.Execute (query);
			}
		}

		#endregion
	}
}

