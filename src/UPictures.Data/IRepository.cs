using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UPictures.Data
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        TEntity GetById(TKey id);

        void Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);        

        IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> predicate);
    }    
}