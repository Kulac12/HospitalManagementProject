using Core.Domain_one.Abstract;
using Core.Models.PaginationModels;
using Domain_one.HospitalDatabase.Tables.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TMS.Core.Models.PaginationModels;

namespace DataAccess_two.HospitalDatabase.Repositories.Abstracts.Base
{
    //generic constraint
    //**
    //IEntity: IEntity olabilir veya IEntity implement eden bir nesne olabilir
    //new()  : new'lenebilir olmalı

    //EnginDemiroğ -->IEntityRepository<T> where T : class, IEntity, new()
    public interface ITableRepository<T> where T:BaseEntity, IEntity, new()
    {
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
