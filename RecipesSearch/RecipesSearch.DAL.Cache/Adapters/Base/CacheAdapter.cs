using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using InterSystems.Data.CacheClient;
using RecipesSearch.DAL.Cache.Utilities;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.DAL.Cache.Adapters.Base
{
    public class CacheAdapter : IDisposable
    {
        protected CacheConnection CacheConnection = new CacheConnection();

        public CacheAdapter()
        {
            OpenConnection();
        }

        protected void OpenConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Cache"].ConnectionString;
            CacheConnection.ConnectionString = connectionString;
            CacheConnection.Open();
        }

        protected void EnsureConnectionOpened()
        {
            if (CacheConnection.State == ConnectionState.Closed || CacheConnection.State == ConnectionState.Broken)
            {
                Dispose();

                CacheConnection = new CacheConnection();
                OpenConnection();
            }
        }

        public List<T> GetEntities<T>() where T : IEntity, new()
        {
            var query = QueryBuilder.BuildGetEntitiesQuery(typeof (T));

            var command = new CacheCommand(query, CacheConnection);
            command.CommandType = CommandType.Text;

            var dataReader = command.ExecuteReader();

            var entities = ObjectMapper.Map<T>(dataReader);

            return entities;
        }

        public T GetEntityById<T>(int id) where T : IEntity, new()
        {
            var query = QueryBuilder.BuildGetEntitiesByIdQuery(typeof(T), id);

            var command = new CacheCommand(query, CacheConnection);
            command.CommandType = CommandType.Text;

            var dataReader = command.ExecuteReader();

            var entities = ObjectMapper.Map<T>(dataReader);

            return entities.FirstOrDefault();
        }

        public bool UpdateEntity<T>(T entity) where T : IEntity
        {
            return UpsertEntity(entity, QueryBuilder.BuildUpdateQuery);
        }

        public bool InsertEntity<T>(T entity) where T : IEntity
        {
            return UpsertEntity(entity, QueryBuilder.BuildInsertQuery);
        }

        public bool DeleteEntity<T>(int id) where T : IEntity
        {
            var query = QueryBuilder.BuildDeleteQuery(typeof(T), id);

            var command = new CacheCommand(query, CacheConnection);
            command.CommandType = CommandType.Text;

            return command.ExecuteNonQuery() != 0;
        } 

        public void Dispose()
        {
            if(CacheConnection != null && CacheConnection.State != ConnectionState.Closed)
            {
                CacheConnection.Close();
                CacheConnection.Dispose();
            }       
        }

        protected void AddSqlParameters(CacheCommand command, IEnumerable<CacheParameter> parameters)
        {
            foreach (var cacheParameter in parameters)
            {
                command.Parameters.Add(cacheParameter);
            }            
        }

        protected string GetFullProcedureName(string procedureName, string packageName = "")
        {
            if(String.IsNullOrEmpty(packageName))
            {
                packageName = Constants.DefaultCachePackage;
            }
            return String.Format("{0}.{1}", packageName, procedureName);
        }

        private bool UpsertEntity<T>(T entity, Func<T, string> queryBuilder)
        {
            var query = queryBuilder(entity);
            var parameters = ParametersBuilder.BuildParametersFromObject(entity);

            var command = new CacheCommand(query, CacheConnection);
            command.CommandType = CommandType.Text;

            AddSqlParameters(command, parameters);

            return command.ExecuteNonQuery() != 0;
        }
    }
}
