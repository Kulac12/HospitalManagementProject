using Core.Models.PaginationModels;
using DataAccess_two.HospitalDatabase.Repositories.Abstracts.Base;
using Domain_one.HospitalDatabase.Tables.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TMS.Core.Models.PaginationModels;
using Core.Domain_one.Abstract;

namespace DataAccess_two.HospitalDatabase.Repositories.Concretes.Base
{
    //EnginDemiroğ --> EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
    public class TableRepository<TEntity, TContext> : ITableRepository<TEntity>
        where TEntity : BaseEntity, IEntity, new()    // bu bir veri tabanı tablosu olacak ama IEntity direk yazamasın diye new() yazıyoruz. 
       where TContext : DbContext, new()  //Bu bir DbContext sınıfı olacak ama direk kendi adını yazamasın diye yine onu da new ledik.
    {
        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public TEntity Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
