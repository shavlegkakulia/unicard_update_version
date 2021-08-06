using System;
using Kuni.Core.Providers.LocalDBProvider;
using System.IO;
using Kuni.Core.Models.DB;
using System.Collections.Generic;
using System.Linq;
using Kuni.Core.Models;
using SQLite;

namespace Kunicardus.Droid.Providers.DroidSqLiteProvider
{
    public class DroidSqLiteProvider : SQLiteConnection, ILocalDbProvider
    {
        static object locker = new object();

        public static string DBPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "unicard.db");
            }
        }

        public DroidSqLiteProvider() : base(DBPath)
        {
            
            lock (locker)
            {
                CreateTable<UserInfo>();
                CreateTable<NewsInfo>();
                CreateTable<MerchantInfo>();
                CreateTable<TransactionInfo>();
                CreateTable<UserTypesInfo>();
                CreateTable<ProductCategoryInfo>();
                CreateTable<ProductsInfo>();
                CreateTable<DeliveryMethod>();
                CreateTable<SettingsInfo>();
                CreateTable<AutoCompleteFields>();
                CreateTable<OrganizationModel>();
                CreateTable<VersionsModel>();
            }
       }

        #region ILocalDbProvider implementation

        public System.Collections.Generic.List<T> Get<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Kuni.Core.Models.DB.DBModel, new()
        {
            lock (locker)
            {
                return base.Get<List<T>>(predicate);
            }
        }

        public System.Collections.Generic.List<T> Get<T>() where T : Kuni.Core.Models.DB.DBModel, new()
        {
            lock (locker)
            {
                return base.Table<T>().ToList();
            }
        }

        public void Insert<T>(T entity) where T : Kuni.Core.Models.DB.DBModel
        {
            lock (locker)
            {
                base.Insert(entity);
            }
        }

        public void Insert<T>(System.Collections.Generic.IEnumerable<T> entities) where T : Kuni.Core.Models.DB.DBModel
        {
            lock (locker)
            {
                base.InsertAll(entities);
            }
        }

        public void Update<T>(T entity) where T : Kuni.Core.Models.DB.DBModel
        {
            lock (locker)
            {
                base.Update(entity, typeof(T));
            }
        }

        public void Update<T>(System.Collections.Generic.IEnumerable<T> entities) where T : Kuni.Core.Models.DB.DBModel
        {
            lock (locker)
            {
                base.UpdateAll(entities);
            }
        }

        public void Delete<T>(T entity) where T : Kuni.Core.Models.DB.DBModel
        {
            lock (locker)
            {
                base.Delete(entity);
            }
        }

        public int GetCount<T>() where T : new()
        {
            lock (locker)
            {
                return base.Table<T>().Count();
            }
        }

        public int GetCount<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : new()
        {
            lock (locker)
            {
                return base.Get<List<T>>(predicate).Count;
            }
        }

        public bool Any<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Kuni.Core.Models.DB.DBModel, new()
        {
            //return base.Table<T> ().A
            throw new NotImplementedException();
        }

        public System.Collections.Generic.List<T> Query<T>(string query, params object[] args) where T : new()
        {
            lock (locker)
            {
                return base.Query<T>(query, args);
            }
        }

        public void ExecuteScalar<T>(string query) where T : class
        {
            lock (locker)
            {
                base.ExecuteScalar<T>(query);
            }
        }

        public void Execute(string query)
        {
            lock (locker)
            {
                base.Execute(query);
            }
        }

        public void Dispose()
        {
            
        }

        #endregion
    }
}

