using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.Common.Interfaces
{
    public interface IBusinessLogic<TObj, TEntity>
        where TObj : class
        where TEntity : class
    {        
        TObj Get(TEntity entity);
        List<TObj> Get(IEnumerable<TEntity> entities);
        List<TObj> Get();

        void Create(TObj obj);
        void Create(List<TObj> objs);
        //////////////////////////////////////////////
        //Update will only update the root object - and will only modify changed properties
        //////////////////////////////////////////////
        void Update(TObj obj);
        void Update(List<TObj> objs);
        //void Delete(params object[] keys);
        //////////////////////////////////////////////
        //Delete will only delete the root object
        //////////////////////////////////////////////
        void Delete(params object[] keys);
        void Delete(TObj obj);
        void Delete(List<TObj> objs);
        //////////////////////////////////////////////
        //Insert will insert all nested child objects
        //////////////////////////////////////////////
        void Insert(TObj obj);
        void Insert(List<TObj> objs);

        TObj MapEntityToObject(TEntity entity);
        IEnumerable<TObj> MapEntitiesToObjects(IEnumerable<TEntity> entities);

        TEntity MapObjectToEntity(TObj obj);
        IEnumerable<TEntity> MapObjectsToEntities(IEnumerable<TObj> objs);

        TEntity MapRootObjectToEntity(TObj obj, TEntity entity);
    }
   
}