﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Data.CacheClient;
using RecipesSearch.DAL.Cache.Utilities;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Models.Base;
using RecipesSearch.Data.Views;

namespace RecipesSearch.DAL.Cache.Adapters.Base
{
    public class CacheAdapter : IDisposable
    {
        protected readonly CacheConnection CacheConnection = new CacheConnection();

        public CacheAdapter()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Cache"].ConnectionString;
            CacheConnection.ConnectionString = connectionString;
            CacheConnection.Open();
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
            CacheConnection.Close();
        }

        protected void AddSqlParameters(CacheCommand command, IEnumerable<CacheParameter> parameters)
        {
            foreach (var cacheParameter in parameters)
            {
                command.Parameters.Add(cacheParameter);
            }            
        }

        protected string GetFullProcedureName(string procedureName)
        {
            return String.Format("{0}.{1}", Constants.DefaultCachePackage, procedureName);
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