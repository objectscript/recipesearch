using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RecipesSearch.DAL.Cache.Utilities
{
    public class ObjectMapper
    {
        public static List<T> Map<T>(IDataReader reader) where T : new()
        {
            var items = new List<T>();
            var propInfo = typeof(T).GetProperties();

            using (reader)
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new T();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var prop = propInfo.FirstOrDefault(x => x.Name.ToLower() == reader.GetName(i).ToLower());

                            if (prop == null) continue;
                            prop.SetValue(item, reader.GetValue(i) is DBNull ? null : reader.GetValue(i), null);
                        }

                        items.Add(item);
                    }
                }
            }

            return items;
        }
    }
}