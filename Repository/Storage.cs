using System;
using System.Collections.Generic;
using System.Linq;
using PetaPoco;
using Repository.Enums;
using Repository.Models;

namespace Repository
{
    public class Storage<TEntity> : IStorage<TEntity> where TEntity : BaseEntity
    {
        private readonly Lazy<Database> db = new Lazy<Database>(CreateDb);
        private const string connectionName = "Data";
        private readonly ICommandSaver commandSaver;
        private readonly IUserContext userContext;

        public Storage(ICommandSaver commandSaver, IUserContext userContext)
        {
            this.commandSaver = commandSaver;
            this.userContext = userContext;
        }

        private static Database CreateDb()
        {
            return new Database(connectionName);
        }

        public TEntity GetByKey(object keyValue)
        {
            return db.Value.SingleOrDefault<TEntity>(keyValue);
        }

        public IEnumerable<TEntity> GetAll()
        {
            var pd = TableInfo.FromPoco(typeof(TEntity));
            var sql = "SELECT * FROM " + pd.TableName;

            return db.Value.Query<TEntity>(sql);
        }

        public IEnumerable<TEntity> Find(string sqlCondition, params object[] args)
        {
            return db.Value.Query<TEntity>(sqlCondition, args);
        }

        public IEnumerable<TEntity> Query(Sql sql)
        {
            return db.Value.Query<TEntity>(sql);
        }

        public IEnumerable<TEntity> Find(Sql sql)
        {
            return db.Value.Fetch<TEntity>(sql);
        }

        public TEntity Single(string sqlCondition, params object[] args)
        {
            return db.Value.Single<TEntity>(sqlCondition, args);
        }

        public TEntity First(string sqlCondition, params object[] args)
        {
            return db.Value.First<TEntity>(sqlCondition, args);
        }

        public TEntity Insert(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
            db.Value.Insert(entity);
            db.Value.Insert(commandSaver.CreateCommand(entity, CommandTypes.Insert, GetUserName()));
            return entity;
        }

        public virtual void Delete(TEntity entity)
        {
            db.Value.Delete(entity);
            db.Value.Insert(commandSaver.CreateCommand(entity, CommandTypes.Delete, GetUserName()));
        }

        public virtual void Delete(string sqlCondition, params object[] args)
        {
            db.Value.Delete<TEntity>(sqlCondition, args);
        }

        public void Update(TEntity entity)
        {
            db.Value.Update(entity);
            db.Value.Insert(commandSaver.CreateCommand(entity, CommandTypes.Update, GetUserName()));
        }

        public Page<TEntity> Get(long pageNumber, long itemsPerPage, string sqlCondition, params object[] args)
        {
            return db.Value.Page<TEntity>(pageNumber, itemsPerPage, sqlCondition, args);
        }

        public int Count()
        {
            return db.Value.ExecuteScalar<int>("select count(*) from " + TableInfo.FromPoco(typeof(TEntity)).TableName);
        }

        public int Count(string sqlCondition, params object[] args)
        {
            return db.Value.Query<TEntity>(sqlCondition, args).Count();
        }

        private string GetUserName()
        {
            var principal = userContext.User;
            return principal == null ? "test" : principal.Identity.Name;
        }
    }
}