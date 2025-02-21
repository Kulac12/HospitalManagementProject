using DataAccess_two.HospitalDatabase.Repositories.Abstracts;
using DataAccess_two.HospitalDatabase.Repositories.Concretes.Base;
using Domain_one.HospitalDatabase.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_two.HospitalDatabase.Repositories.Concretes
{
    public class UserRepository : TableRepository<User>, IUserRepository
    {
        public UserRepository(DbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext, httpContextAccessor)
        {
        }
    }
}
