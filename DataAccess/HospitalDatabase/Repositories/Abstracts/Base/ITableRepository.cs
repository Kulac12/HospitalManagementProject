using Core.Models.PaginationModels;
using Domain_one.AC.Abstract;
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
    public interface ITableRepository<T> where T: BaseEntity, IEntity
    {
        Task<IQueryable<T>> CustomQuery();
        Task<List<T>> GetAllDeletedControlAsync(Expression<Func<T, bool>> filter = null, bool withDeleted = false, bool companyIdRequired = true);
        Task<int> CountOfEntity(bool withDeleted = false);
        Task<bool> AnyEntity(Expression<Func<T, bool>> filter);
        Task<T> GetLastOrDefault(Expression<Func<T, bool>> filter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="companyIdRequired"></param>
        /// <returns></returns>
        Task<List<T>> GetAllWithoutBaseEntityParamsSetAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true);

        /// <summary>
        /// Verilen tablo ve onunla  birlikte içerilmesi istenen tabloları bir arada getirip server side şekilde listeleyip sorgulamaya yarar.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<PaginationResponseModel<T>> GetPaginateAllAsyncWithIncludeParams(
            PaginationRequestModel model,
             Expression<Func<T, bool>> filter,
             bool companyIdRequired = true,
             params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Id değeri verilen silinmemiş veri tabanı kaydını geriye döndürür.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(Guid id, bool companyIdRequired = true);

        /// <summary>
        /// Talep edilen tabloya ait filtrelenmiş listeyi döndürür.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true);
        /// <summary>
        /// Talep edilen tabloya ait filtrelenmiş kaydı döndürür.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool companyIdRequired = true);
        /// <summary>
        /// Belirtilen filtreye göre, tabloyu ilişkili olduğu tabloları ile birlikte getirir. Kullanım için "includes" parametresine ilişkili olan tabloları aralara virgül koyarak yazabilirsiniz.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<List<T>> GetAllAsyncWithIncludeParams(Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Belirtilen filtreye göre, tabloyu ilişkili olduğu tabloları ile birlikte listeler. Kullanım için "includes" parametresine ilişkili olan tabloları aralara virgül koyarak yazabilirsiniz.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>

        Task<T> GetAsyncWithIncludeParams(Expression<Func<T, bool>> filter, bool companyIdRequired = true, params Expression<Func<T, object>>[] includes);


        /// <summary>
        /// Gönderilen objeyi veri tabanına kaydeder.
        /// </summary>
        /// <param name="entity"></param>
        Task<T> AddAsync(T entity, bool save = false);

        /// <summary>
        /// Liste olarak verilen objeleri veri tabanına sırasıyla ekler.
        /// </summary>
        /// <param name="entities"></param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Objeyi veri tabanında günceller.
        /// </summary>
        /// <param name="entity"></param>
        T Update(T entity);

        /// <summary>
        /// Id'si verilen kaydı veri tabanından soft delete şeklinde siler. 
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
        /// <summary>
        /// Id'si verilen kaydı veri tabanından hard delete şeklinde siler. 
        /// </summary>
        /// <param name="entity"></param>
        void Remove(T entity);

        /// <summary>
        /// Kayıt listesini veri tabanından soft delete şeklinde siler. 
        /// </summary>
        /// <param name="entities"></param>
        void DeleteRange(List<T> entities);

        /// <summary>
        /// Kayıt listesini veri tabanından hard delete şeklinde siler. 
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(List<T> entities);
        /// <summary>
        /// Kayıt listesini veri tabanında toplu güncelleme için kullanılır.
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange(List<T> entities);
        void DeleteRangeById(List<Guid> entityIds);
        Task<T> AddOrUpdateAsync(T entity);
    }
}
