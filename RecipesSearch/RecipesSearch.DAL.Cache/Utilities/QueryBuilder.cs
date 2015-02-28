using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.DAL.Cache.Utilities
{
    class QueryBuilder
    {
        public static string BuildGetEntitiesQuery(Type entityType)
        {
            string tableName = GetTableName(entityType);

            var parameters = String.Join(", ", ParametersBuilder.GetParametersNames(entityType, true));
            string query = String.Format("SELECT {0} FROM {1} WHERE IsActive = 1", parameters, tableName);

            return query;
        }

        public static string BuildGetEntitiesByIdQuery(Type entityType, int id)
        {
            string tableName = GetTableName(entityType);

            var parameters = String.Join(", ", ParametersBuilder.GetParametersNames(entityType, true));
            string query = String.Format("SELECT {0} FROM {1} WHERE Id = {2} AND IsActive = 1", parameters, tableName, id);

            return query;
        }

        public static string BuildUpdateQuery<T>(T entity) where T : IEntity
        {
            var entityType = typeof(T);
            string tableName = GetTableName(entityType);

            var parameters = ParametersBuilder.GetParametersNames(entityType);
            var updateSetParameters = String.Join(", ", parameters.Select(parameter => String.Format("{0} = @{0}", parameter)));

            var query = String.Format("UPDATE {0} SET {1} WHERE Id = {2}", tableName, updateSetParameters, entity.Id);

            return query;
        } 

        public static string BuildInsertQuery<T>(T entity) where T : IEntity
        {
            var entityType = typeof (T);
            string tableName = GetTableName(entityType);

            var parameters = ParametersBuilder.GetParametersNames(entityType);
            var insertParameters = String.Join(", ", parameters);
            var insertValuesParameters = String.Join(", ", parameters.Select(parameter => String.Format("@{0}", parameter)));

            var query = String.Format("INSERT INTO {0}({1}) VALUES({2})", tableName, insertParameters, insertValuesParameters);

            return query;
        }

        public static string BuildDeleteQuery(Type entityType, int id)
        {
            string tableName = GetTableName(entityType);

            string query = String.Format("UPDATE {0} SET IsActive = 0 WHERE Id = {1}", tableName, id);

            return query;
        }

        private static string GetTableName(Type entityType)
        {
            var packageNameAttribute = entityType.GetCustomAttributes(typeof(CachePackageAttribute), true).FirstOrDefault();

            if (packageNameAttribute == null)
            {
                throw new ArgumentException("CachagePackageAttribute must be specified for cache entity");
            }

            return String.Format("{0}.{1}", ((CachePackageAttribute)packageNameAttribute).GetPackageName(), entityType.Name);
        }
    }
}
