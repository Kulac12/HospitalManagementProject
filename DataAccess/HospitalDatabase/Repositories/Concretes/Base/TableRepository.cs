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
using Domain_one.AC.Abstract;

namespace DataAccess_two.HospitalDatabase.Repositories.Concretes.Base
{
    public class TableRepository<T> : ITableRepository<T> where T : BaseEntity, IEntity
    {
        public DbSet<T> Table => _dbContext.Set<T>();
        protected DbContext _dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TableRepository(DbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<T> AddAsync(T entity, bool save = false)
        {
            var a = await Table.AddAsync(entity);
            if (!save || await _dbContext.SaveChangesAsync() > 0)
                return a.Entity;
            return null;
        }
        public async Task<T> AddOrUpdateAsync(T entity)
        {
            EntityEntry<T> entry = Table.Find(entity.Id) == null
                ? await Table.AddAsync(entity)
                : Table.Update(entity);
            return entry.Entity;
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await Table.AddRangeAsync(entities);
        }
        public void Delete(T entity)
        {
            try
            {
                entity.Deleted = true;
                Table.Update(entity);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public void Remove(T entity)
        {
            Table.Remove(entity);
        }
        public T Update(T entity)
        {
            var model = _dbContext.Entry(entity);
            model.State = EntityState.Detached;
            Table.Update(entity);


            //model.State = EntityState.Detached;
            return entity;
        }
        public void DeleteRange(List<T> entities)
        {
            try
            {
                foreach (var item in entities)
                {
                    item.Deleted = true;
                }
                Table.UpdateRange(entities);
            }
            catch (Exception ex)
            {

                throw;
            }
            //foreach (var item in entitiesList)
            //{
            //    var model = _dbContext.Entry(item);
            //    model.State = EntityState.Detached;
            //}
        }
        public void DeleteRangeById(List<Guid> entityIds)
        {
            try
            {
                List<T> entities = new List<T>();
                foreach (var id in entityIds)
                {
                    var item = Table.Find(id);
                    item.Deleted = true;
                    _dbContext.Entry(item).State = EntityState.Detached;
                    entities.Add(item);
                }
                Table.UpdateRange(entities);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //foreach (var item in entitiesList)
            //{
            //    var model = _dbContext.Entry(item);
            //    model.State = EntityState.Detached;
            //}
        }
        public void RemoveRange(List<T> entities)
        {
            Table.RemoveRange(entities);
        }
        Guid UserCompanyId()
        {
            var user = httpContextAccessor.HttpContext?.User;
            Guid companyId = Guid.Empty;
            var userCompanyId = user?.Claims?.FirstOrDefault(x => x.Type == "CompanyId")?.Value ?? "";
            if (userCompanyId != "")
                companyId = Guid.Parse(userCompanyId);
            return companyId;
        }
        public async Task<int> CountOfEntity(bool withDeleted = false)
        {
            return withDeleted ? await Table.AsNoTracking().CountAsync() :
                await Table.AsNoTracking().Where(x => !x.Deleted).CountAsync();
        }
        public async Task<bool> AnyEntity(Expression<Func<T, bool>> filter) => await Table.AsNoTracking().AnyAsync(filter);
        public async Task<T> GetLastOrDefault(Expression<Func<T, bool>> filter) => await Table.AsNoTracking().OrderBy(x => x.CreateTime).Where(x => !x.Deleted).LastOrDefaultAsync(filter);

        public async Task<IQueryable<T>> CustomQuery()
        {
            return await Task.FromResult(Table.AsNoTracking());
        }

        #region
        //public async Task<List<T>> GetAllWithoutBaseEntityParamsSetAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true)
        //{
        //    return companyIdRequired ?
        //         filter != null ?
        //        await Table.AsNoTracking().Where(filter).Where(x => x.CompanyId == UserCompanyId()).ToListAsync() :
        //        await Table.AsNoTracking().Where(x => x.CompanyId == UserCompanyId()).ToListAsync() :
        //         filter != null ?
        //        await Table.AsNoTracking().Where(filter).ToListAsync() :
        //        await Table.AsNoTracking().ToListAsync();
        //}
        //public async Task<List<T>> GetAllDeletedControlAsync(Expression<Func<T, bool>> filter = null, bool withDeleted = false, bool companyIdRequired = true)
        //{
        //    bool? deletedStatus = withDeleted ? null : false;
        //    return companyIdRequired ?
        //         filter != null ?
        //        await Table.AsNoTracking().Where(filter).Where(x => (deletedStatus == null || x.Deleted == deletedStatus) && x.CompanyId == UserCompanyId()).ToListAsync() :
        //        await Table.AsNoTracking().Where(x => (deletedStatus == null || x.Deleted == deletedStatus) && x.CompanyId == UserCompanyId()).ToListAsync() :
        //         filter != null ?
        //        await Table.AsNoTracking().Where(filter).Where(x => deletedStatus == null || x.Deleted == deletedStatus).ToListAsync() :
        //        await Table.AsNoTracking().Where(x => deletedStatus == null || x.Deleted == deletedStatus).ToListAsync();
        //}
        //public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true)
        //{
        //    return companyIdRequired ?
        //         filter != null ?
        //        await Table.AsNoTracking().Where(filter).Where(x => !x.Deleted && x.CompanyId == UserCompanyId()).ToListAsync() :
        //        await Table.AsNoTracking().Where(x => !x.Deleted && x.CompanyId == UserCompanyId()).ToListAsync() :
        //         filter != null ?
        //        await Table.AsNoTracking().Where(filter).Where(x => !x.Deleted).ToListAsync() :
        //        await Table.AsNoTracking().Where(x => !x.Deleted).ToListAsync();
        //}
        //public async Task<T> GetAsync(Expression<Func<T, bool>> filter, bool companyIdRequired = true)
        //{
        //    return companyIdRequired ?
        //        await Table.AsNoTracking()
        //        .Where(x => !x.Deleted && x.CompanyId == UserCompanyId()).SingleOrDefaultAsync(filter) :
        //        await Table.AsNoTracking()
        //        .Where(x => !x.Deleted).SingleOrDefaultAsync(filter);
        //}
        //public async Task<T> GetByIdAsync(Guid id, bool companyIdRequired = true)
        //{
        //    companyIdRequired = false;
        //    return
        //        companyIdRequired ?
        //        await Table.AsNoTracking().Where(x => !x.Deleted && x.CompanyId == UserCompanyId()).FirstOrDefaultAsync(x => x.Id == id)
        //        :
        //        await Table.AsNoTracking().Where(x => !x.Deleted).FirstOrDefaultAsync(x => x.Id == id);
        //}
        //public async Task<List<T>> GetAllAsyncWithIncludeParams(Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes)
        //{
        //    companyIdRequired = false;

        //    IQueryable<T> entities = Table;

        //    var list = companyIdRequired
        //        /// CompanyId değeri kontrol edilsin mi true ise sorguya dahil edilir, değil ise kontrol edilmez.
        //        ? filter != null
        //            ? entities.AsNoTracking()
        //                      .Where(filter)
        //                      .Where(x => !x.Deleted && x.CompanyId == UserCompanyId())
        //                      .AsQueryable()
        //                      .AsNoTracking()
        //            : entities.AsNoTracking()
        //                      .Where(x => !x.Deleted && x.CompanyId == UserCompanyId())
        //                      .AsQueryable()
        //                      .AsNoTracking()
        //        : filter != null
        //            ? entities.AsNoTracking()
        //                      .Where(filter)
        //                      .Where(x => !x.Deleted)
        //                      .AsQueryable()
        //                      .AsNoTracking()
        //            : entities.AsNoTracking()
        //                      .Where(x => !x.Deleted)
        //                      .AsQueryable()
        //                      .AsNoTracking();

        //    list = list.IncludeMultiple(includes);
        //    return await list.AsQueryable().ToListAsync();
        //}

        //public async Task<PaginationResponseModel<T>> GetPaginateAllAsyncWithIncludeParams(PaginationRequestModel model,
        //    Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes)
        //{
        //    IQueryable<T> entities = Table;
        //    var list =
        //        // CompanyId değeri kontrol edilsin mi true ise sorguya dahil edilir, değil ise kontrol edilmez.
        //        companyIdRequired ?
        //        filter != null
        //        ?
        //        entities.AsNoTracking()
        //        .Where(filter)
        //        .Where(x => !x.Deleted && x.CompanyId == UserCompanyId())
        //        .AsQueryable()
        //        :
        //        entities.AsNoTracking()
        //        .Where(x => !x.Deleted && x.CompanyId == UserCompanyId())
        //        .AsQueryable()
        //        :
        //        filter != null
        //        ?
        //        entities.AsNoTracking()
        //        .Where(filter)
        //        .Where(x => !x.Deleted)
        //        .AsQueryable()
        //        :
        //        entities.AsNoTracking()
        //        .Where(x => !x.Deleted)
        //        .AsQueryable();

        //    list = list.IncludeMultiple(includes);

        //    string orderType = model.IsAscending ? " ASC" : " DESC";

        //    list = string.IsNullOrEmpty(model.OrderColumnName) ? list : list.OrderBy(model.OrderColumnName + orderType);
        //    int listCount = list.Count();


        //    // Pagination İşlemleri
        //    model.Skip = model.Skip > 0 ? model.Skip : 1;
        //    model.Take = model.Take > 0 ? Convert.ToInt32(model.Take) : 10;

        //    int skip = (model.Skip - 1) * model.Take >= 0
        //                ? Convert.ToInt32((model.Skip - 1) * model.Take)
        //                : 10;
        //    var paginateResult = list.Skip(skip).Take(model.Take).ToList();


        //    return await Task.FromResult(new PaginationResponseModel<T>() { Data = paginateResult, TotalCount = listCount });


        //}


        //public async Task<T> GetAsyncWithIncludeParams(Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes)
        //{
        //    IQueryable<T> entities = Table;
        //    entities = entities.IncludeMultiple(includes);
        //    return companyIdRequired ? await entities
        //        .Where(x => !x.Deleted &&
        //         x.CompanyId == UserCompanyId())
        //        .SingleOrDefaultAsync(filter) : await entities
        //        .Where(x => !x.Deleted)
        //        .SingleOrDefaultAsync(filter);
        //}
        #endregion


        public void UpdateRange(List<T> entities)
        {
            Table.UpdateRange(entities);
        }

        public Task<List<T>> GetAllDeletedControlAsync(Expression<Func<T, bool>> filter = null, bool withDeleted = false, bool companyIdRequired = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllWithoutBaseEntityParamsSetAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true)
        {
            throw new NotImplementedException();
        }

        public Task<PaginationResponseModel<T>> GetPaginateAllAsyncWithIncludeParams(PaginationRequestModel model, Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(Guid id, bool companyIdRequired = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllAsyncWithIncludeParams(Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsyncWithIncludeParams(Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
