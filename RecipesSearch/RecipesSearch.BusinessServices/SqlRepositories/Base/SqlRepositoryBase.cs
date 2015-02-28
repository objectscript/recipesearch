using System;
using System.Collections.Generic;
using System.Linq;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories.Base
{
    public class SqlRepositoryBase
    {
        protected T SaveEntity<T>(T entity) where T : Entity, new() 
        {
            try
            {
                using (var cacheAdapter = new CacheAdapter())
                {
                    if (entity.Id != 0)
                    {
                        var original = cacheAdapter.GetEntityById<T>(entity.Id);
                        if (original != null)
                        {
                            entity.ModifiedDate = DateTime.Now.ToUniversalTime();
                            entity.CreatedDate = original.CreatedDate;

                            cacheAdapter.UpdateEntity(entity);

                            return cacheAdapter.GetEntityById<T>(entity.Id);
                        }
                        return null;
                    }
                    else
                    {
                        entity.CreatedDate = DateTime.Now.ToUniversalTime();
                        entity.ModifiedDate = entity.CreatedDate;

                        cacheAdapter.InsertEntity(entity);

                        return cacheAdapter.GetEntityById<T>(entity.Id);
                    }
                }           
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.SaveEntity<{0}> failed", typeof(T)), exception);
                return null;
            }      
        }

        protected T GetEntityById<T>(int id) where T : Entity, new()
        {
            try
            {
                using (var cacheAdapter = new CacheAdapter())
                {
                    return cacheAdapter.GetEntityById<T>(id);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.GetEntityById<{0}> failed", typeof(T)), exception);
                return null;
            }
        }

        protected List<T> GetEntities<T>() where T : Entity, new()
        {
            try
            {
                using (var cacheAdapter = new CacheAdapter())
                {
                    return cacheAdapter
                        .GetEntities<T>()
                        .OrderByDescending(entity => entity.Id)
                        .ToList();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.GetEntities<{0}> failed", typeof(T)), exception);
                return null;
            }
        }

        protected bool DeleteEntity<T>(int id) where T : Entity
        {
            try
            {
                using (var cacheAdapter = new CacheAdapter())
                {
                    return cacheAdapter.DeleteEntity<T>(id);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.DeleteEntity<{0}> failed", typeof(T)), exception);
                return false;
            }
        }
    }
}
