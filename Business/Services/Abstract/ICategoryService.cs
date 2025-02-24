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
       Task< IDataResult<List<Category>>> GetAll();
       Task<IDataResult<Category>> GetById(Guid categoryId);

    }
}
