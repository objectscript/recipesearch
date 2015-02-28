using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Data.CacheClient;

namespace RecipesSearch.DAL.Cache.Utilities
{
    public static class ParametersBuilder
    {
        public static List<CacheParameter> BuildParametersFromObject(object item, bool includeKeyProperty = false)
        {
            if (item == null)
            {
                return new List<CacheParameter>();
            }

            IList<PropertyInfo> props = GetDatabaseProperties(item.GetType(), includeKeyProperty);

            if (!props.Any())
            {
                props = item.GetType().GetProperties();
            }

            return BuildParameters(item, props);
        }


        public static List<String> GetParametersNames(Type entityType, bool includeKeyProperty = false)
        {
            return GetDatabaseProperties(entityType, includeKeyProperty)                
                .Select(propertyInfo => propertyInfo.Name)
                .ToList();
        }

        private static List<PropertyInfo> GetDatabaseProperties(Type entityType, bool includeKeyProperty)
        {
            return entityType
                .GetProperties()
                .Where(x => x.GetCustomAttributes(true).All(y => y.GetType() != typeof(NotMappedAttribute)))
                .Where(parameter => includeKeyProperty || parameter.GetCustomAttributes(true).All(y => y.GetType() != typeof(KeyAttribute)))
                .ToList();
        }

        private static List<CacheParameter> BuildParameters(object item, IEnumerable<PropertyInfo> props)
        {
            var parameters = new List<CacheParameter>();

            foreach (var prop in props)
            {
                var paramName = prop.Name;

                parameters.Add(new CacheParameter
                {
                    ParameterName = string.Format("{0}", paramName),
                    CacheDbType = GetParameterType(prop.PropertyType),
                    Value = prop.GetValue(item, null)
                });
            }

            return parameters;
        }

        private static CacheDbType GetParameterType(Type propertyType)
        {
            var typeCode = Type.GetTypeCode(propertyType);
            switch (typeCode)
            {
                case TypeCode.Boolean: return CacheDbType.Bit;
                case TypeCode.Byte: return CacheDbType.TinyInt;
                case TypeCode.Char: return CacheDbType.NVarChar;
                case TypeCode.DateTime: return CacheDbType.DateTime;
                case TypeCode.Decimal: return CacheDbType.Numeric;
                case TypeCode.Double: return CacheDbType.Double;
                case TypeCode.Int32: return CacheDbType.Int;
                case TypeCode.Int64: return CacheDbType.BigInt;
                case TypeCode.Single: return CacheDbType.Double;
                case TypeCode.String: return CacheDbType.NVarChar;
                case TypeCode.Object:
                    if (propertyType == typeof (Guid) || Nullable.GetUnderlyingType(propertyType) == typeof (Guid))
                    {
                        return CacheDbType.UniqueIdentifier;
                    }
                    else
                    {
                        return CacheDbType.NVarChar;
                    }
                default: return CacheDbType.NVarChar;
            }
        }
    }
}
