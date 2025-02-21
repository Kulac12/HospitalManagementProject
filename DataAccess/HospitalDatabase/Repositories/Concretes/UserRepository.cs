using Core.Entities.Concrete;
using DataAccess_two.HospitalDatabase.Contexts.EntityFramework;
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
    public class UserRepository : TableRepository<User, HospitalDatabaseContext>, IUserRepository
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using (var context = new HospitalDatabaseContext())
            {
                var result = from operationClaim in context.OperationClaims
                             join userOperationClaim in context.UserOperationClaims
                                 on operationClaim.Id equals userOperationClaim.OperationClaimId
                             where userOperationClaim.UserId == user.Id
                             select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };
                return result.ToList();

            }
        }
    }
}
