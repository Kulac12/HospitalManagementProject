using DataAccess_two.HospitalDatabase.Contexts.EntityFramework;
using DataAccess_two.HospitalDatabase.Repositories.Abstracts;
using DataAccess_two.HospitalDatabase.Repositories.Concretes.Base;
using Domain_one.HospitalDatabase.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_two.HospitalDatabase.Repositories.Concretes
{
    public class CategoryRepository : TableRepository<Category, HospitalDatabaseContext>, ICategoryRepository
    {


    }
}
