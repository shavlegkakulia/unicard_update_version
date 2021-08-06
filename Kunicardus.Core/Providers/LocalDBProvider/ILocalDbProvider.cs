using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Kunicardus.Core.Models.DB;

namespace Kunicardus.Core.Providers.LocalDBProvider
{
	/// <summary>
	/// Defines the interface for local database providers such as sqlite
	/// </summary>
	public interface ILocalDbProvider : IDisposable
	{
		/// <summary>
		/// Gets a list of entities based on query predicate
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <param name="predicate">Where clause for query</param>
		/// <returns>A list of all entities matching the T type and predicate</returns>
		List<T> Get<T> (Expression<Func<T, bool>> predicate) where T : DBModel, new();

		/// <summary>
		/// Get a list of all entities
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <returns>A list of all entities matching the T type</returns>
		List<T> Get<T> () where T : DBModel, new();

		/// <summary>
		/// Insert a new entity into local db
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <param name="entity">Instance of entity to insert</param>
		void Insert<T> (T entity) where T : DBModel;

		/// <summary>
		/// Insert a new entities into local db
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <param name="entities">List of instances of entity to insert</param>
		void Insert<T> (IEnumerable<T> entities) where T : DBModel;

		/// <summary>
		/// Update existing entity in local db
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <param name="entity">Entity to update, key must exist in local db</param>
		void Update<T> (T entity) where T : DBModel;

		/// <summary>
		/// Update existing list of entities in local db
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <param name="entities">Entities to update, keys must exist in local db</param>
		void Update<T> (IEnumerable<T> entities) where T : DBModel;

		/// <summary>
		/// Delete existing entity from local db
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <param name="entity">ENtity to delete, key must exist in local db</param>
		void Delete<T> (T entity) where T : DBModel;

		/// <summary>
		/// Gets the number of entities in local db table
		/// </summary>
		/// <typeparam name="T">Type of entity in local db to query</typeparam>
		/// <returns>Number of entities</returns>
		int GetCount<T> () where T : new();

		/// <summary>
		/// Gets the number of entities in the local db table that match predicate
		/// </summary>
		/// <typeparam name="T">Type of entity in the local db to query</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		int GetCount<T> (Expression<Func<T, bool>> predicate) where T : new();

		/// <summary>
		/// Checks to see if there are any entities in the local db table that match the predicate
		/// </summary>
		/// <typeparam name="T">Type of entity in the local db to query</typeparam>
		/// <param name="predicate"></param>
		/// <returns>Where clause for query</returns>
		bool Any<T> (Expression<Func<T, bool>> predicate) where T : DBModel, new();

		List<T> Query<T> (string query, params object[] args) where T : new();

		void ExecuteScalar<T> (string query) where T : class;

		void Execute (string query);
	}
}

