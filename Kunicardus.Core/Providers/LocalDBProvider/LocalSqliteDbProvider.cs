//using System;
//using SQLite;
//using Cirrious.MvvmCross.Plugins.File;
//using Kunicardus.Core.Helpers.Device;
//using Kunicardus.Core.Helpers.AppSettings;
//using Kunicardus.Core.Models.DB;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Linq;
//using Kunicardus.Core.Models.DB;

namespace Kunicardus.Core.Providers.LocalDBProvider
{
	/// <summary>
	/// ILocalDbProvider implementation for sqlite
	/// </summary>
	//	public class LocalSqliteDbProvider : ILocalDbProvider
	//	{
	//		private readonly ISQLiteConnection _connection;
	//
	//		public LocalSqliteDbProvider (ISQLiteConnectionFactory factory)
	////		(IAppSettings settings, ISQLiteConnectionFactory factory, IMvxFileStore fileStore, IDevice device)
	//		{
	////			var path = fileStore.PathCombine (device.DataPath, settings.LocalDbFileName);
	//			_connection = factory.Create ("unicard.db3");
	////				" New=true; Version=3;PRAGMA locking_mode=EXCLUSIVE; PRAGMA journal_mode=WAL; PRAGMA cache_size=20000; PRAGMA page_size=32768; PRAGMA synchronous=off");
	//
	//			_connection.CreateTable<UserInfo> ();
	//			_connection.CreateTable<NewsInfo> ();
	//			_connection.CreateTable<MerchantInfo> ();
	//			_connection.CreateTable<TransactionInfo> ();
	//			_connection.CreateTable<UserTypesInfo> ();
	//			_connection.CreateTable<ProductCategoryInfo> ();
	//			_connection.CreateTable<ProductsInfo> ();
	//			_connection.CreateTable<DeliveryMethod> ();
	//			_connection.CreateTable<SettingsInfo> ();
	//			_connection.CreateTable<AutoCompleteFields> ();
	//
	//		}
	//
	//		public List<T> Get<T> (Expression<Func<T, bool>> predicate) where T : DBModel, new()
	//		{
	//			lock (_connection) {
	//				return _connection.Table<T> ().Where (predicate).ToList () ?? new List<T> ();
	//			}
	//		}
	//
	//		public List<T> Get<T> () where T : DBModel, new()
	//		{
	//			lock (_connection) {
	//				return _connection.Table<T> ().ToList ();
	//			}
	//		}
	//
	//		public void Insert<T> (T entity) where T : DBModel
	//		{
	//			lock (_connection) {
	//				_connection.Insert (entity, typeof(T));
	//			}
	//		}
	//
	//		public void Insert<T> (IEnumerable<T> entities) where T : DBModel
	//		{
	//			lock (_connection) {
	//				_connection.InsertAll (entities);
	//			}
	//		}
	//
	//		public void Update<T> (T entity) where T : DBModel
	//		{
	//			lock (_connection) {
	//				_connection.Update (entity, typeof(T));
	//			}
	//		}
	//
	//		public void Update<T> (IEnumerable<T> entities) where T : DBModel
	//		{
	//			lock (_connection) {
	//				foreach (var item in entities) {
	//					_connection.Update (item, typeof(T));
	//				}
	//			}
	//		}
	//
	//		public void Delete<T> (T entity) where T : DBModel
	//		{
	//			lock (_connection) {
	//				_connection.Delete (entity);
	////					sqlcon.Commit ();
	//			}
	//		}
	//
	//		public int GetCount<T> () where T : new()
	//		{
	//			lock (_connection) {
	//				return _connection.Table<T> ().Count ();
	//			}
	//		}
	//
	//		public int GetCount<T> (Expression<Func<T, bool>> predicate) where T : new()
	//		{
	//			lock (_connection) {
	//				return _connection.Table<T> ().Where (predicate).Count ();
	//			}
	//		}
	//
	//		public bool Any<T> (Expression<Func<T, bool>> predicate) where T : DBModel, new()
	//		{
	//			lock (_connection) {
	//				return _connection.Table<T> ().Where (predicate).Take (1).Any ();
	//			}
	//		}
	//
	//		public List<T> Query<T> (string query, params object[] args) where T : new()
	//		{
	//			lock (_connection) {
	//				return _connection.Query<T> (query, args);
	//			}
	//		}
	//
	//		public void ExecuteScalar<T> (string query) where T : class
	//		{
	//			lock (_connection) {
	//				_connection.ExecuteScalar<T> (query);
	//			}
	//		}
	//
	//		public void Execute (string query)
	//		{
	//			try {
	//				_connection.Execute (query);
	//				_connection.Commit ();
	//			} catch (Exception ex) {
	//				_connection.Rollback ();
	//			}
	//		}
	//	}
}

