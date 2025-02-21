using Core.Models.PaginationModels;
using DataAccess_two.HospitalDatabase.Repositories.Abstracts.Base;
using Domain_one.HospitalDatabase.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TMS.Core.Models.PaginationModels;

namespace DataAccess_two.HospitalDatabase.Repositories.Abstracts
{
    public interface IFirmRepository : ITableRepository<Firm>
    {
        Task<PaginationResponseModel<Firm>> FirmDatatableAsyncWithIncludeParams(
    PaginationRequestModel model,
           Expression<Func<Firm, bool>> filter,
           bool companyIdRequired = true,
           params Expression<Func<Firm, object>>[] includes);
    }
}
