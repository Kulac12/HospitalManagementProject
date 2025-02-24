using DataAccess_two.HospitalDatabase.Repositories.Abstracts.Base;
using Domain_one.HospitalDatabase.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_two.HospitalDatabase.Repositories.Abstracts
{
    public interface IUserRepository:ITableRepository<User>
    {
        List<OperationClaim> GetClaims(User user);

    }
}
