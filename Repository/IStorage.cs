using System.Collections.Generic;
using PetaPoco;

namespace Repository
{
    public interface IStorage<TEntity> where TEntity : class
    {
        TEntity GetByKey(object keyValue);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(string sqlCondition, params object[] args);
        IEnumerable<TEntity> Query(Sql sql);
        IEnumerable<TEntity> Find(Sql sql);
        TEntity Single(string sqlCondition, params object[] args);
        TEntity First(string sqlCondition, params object[] args);
        TEntity Insert(TEntity entity);
        void Delete(TEntity entity);
        void Delete(string sqlCondition, params object[] args);
        void Update(TEntity entity);
        Page<TEntity> Get(long pageNumber, long itemsPerPage, string sqlCondition, params object[] args);
        int Count();
        int Count(string sqlCondition, params object[] args);
    }
}