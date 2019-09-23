using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WM.Common.Interfaces
{
    public interface IGenericDataRepository<TEntity>
        where TEntity : class
    {
        System.Collections.Generic.IEnumerable<TEntity> GetAll();
        System.Collections.Generic.IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties);
        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate);
        void Insert(TEntity entity);     
        void Update(TEntity entityToUpdate);
        void Delete(TEntity entity);
        void Delete(params object[] keys);
    }
}

