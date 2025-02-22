using Core.Domain_one.Abstract;
using DataAccess_two.HospitalDatabase.Repositories.Abstracts.Base;
using Domain_one.HospitalDatabase.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_two.HospitalDatabase.Repositories.Abstracts
{
    //EnginDemiroğ --> ICategoryDal:IEntityRepository<Category>
    public interface ICategoryRepository:ITableRepository<Category>
    {


    }
}
