using Core.Models.PaginationModels;
using DataAccess_two.HospitalDatabase.Repositories.Abstracts.Base;
using DataAccess_two.HospitalDatabase.Repositories.Abstracts;
using Domain_one.HospitalDatabase.Tables;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TMS.Core.Models.PaginationModels;
using Microsoft.AspNetCore.Http;
using DataAccess_two.HospitalDatabase.Repositories.Concretes.Base;

namespace DataAccess_two.HospitalDatabase.Repositories.Concretes
{
    public class FirmRepository : TableRepository<Firm>, IFirmRepository
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public FirmRepository(DbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<PaginationResponseModel<Firm>> FirmDatatableAsyncWithIncludeParams(PaginationRequestModel model, Expression<Func<Firm, bool>> filter, bool companyIdRequired = true, params Expression<Func<Firm, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
