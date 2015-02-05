using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.DAL.SqlServer.DatabaseContexts;
using RecipesSearch.Data.Models.Base;
using RecipesSearch.Data.Models.Base.Errors;

namespace RecipesSearch.BusinessServices.SqlRepositories.Base
{
    public class SqlRepositoryBase
    {
        protected readonly DatabaseContext _dbContext;

        protected SqlRepositoryBase()
        {
            _dbContext = new DatabaseContext();
        }

        protected T SaveEntity<T>(T entity, DbSet<T> table) where T : Entity, new() 
        {
            try
            {
                if (entity.Id != 0)
                {
                    var original = table.FirstOrDefault(ent => ent.Id == entity.Id);
                    if (original != null)
                    {
                        entity.ModifiedDate = DateTime.Now.ToUniversalTime();
                        entity.CreatedDate = original.ModifiedDate;

                        _dbContext.Entry(original).CurrentValues.SetValues(entity);
                        _dbContext.SaveChanges();
                        return table.FirstOrDefault(ent => ent.Id == entity.Id);
                    }
                    return AddError(new T(), StandardErrors.EntityNotFoundError);
                }
                else
                {
                    table.Add(entity);
                    _dbContext.SaveChanges();
                    return table.FirstOrDefault(ent => ent.Id == entity.Id);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.SaveEntity<{0}> failed", typeof(T)), exception);
                return AddError(new T(), StandardErrors.GeneralError);
            }      
        }

        protected T GetEntityById<T>(int id, DbSet<T> table) where T : Entity, new()
        {
            try
            {
                return table.FirstOrDefault(entity => entity.Id == id && entity.IsActive);
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.GetEntityById<{0}> failed", typeof(T)), exception);
                return AddError(new T(), StandardErrors.GeneralError);
            }
        }

        protected List<T> GetEntities<T>(DbSet<T> table) where T : Entity
        {
            try
            {
                return table
                    .Where(entity=>entity.IsActive)
                    .OrderByDescending(entity=>entity.Id)
                    .ToList();
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.GetEntities<{0}> failed", typeof(T)), exception);
                return null;
            }
        }

        protected bool DeleteEntity<T>(int id, DbSet<T> table) where T : Entity
        {
            try
            {
                var entity = table.FirstOrDefault(ent => ent.Id == id && ent.IsActive);
                if (entity != null)
                {
                    entity.IsActive = false;
                }

                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SqlRepositoryBase.DeleteEntity<{0}> failed", typeof(T)), exception);
                return false;
            }
        }

        protected T AddError<T>(T entity, Error error) where T : Entity
        {
            entity.Errors = entity.Errors ?? new List<Error>();
            entity.Errors.Add(error);
            return entity;
        }
    }
}
