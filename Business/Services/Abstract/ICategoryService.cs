using Core.Utilities.Results;
using Domain_one.HospitalDatabase.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_three.Services.Abstract
{
    public interface ICategoryService
    {
       IDataResult<List<Category>> GetAll();
       IDataResult<Category> GetById(Guid categoryId);

    }
}
