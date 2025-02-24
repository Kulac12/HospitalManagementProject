using Business_three.Services.Abstract;
using Core.Utilities.Results;
using DataAccess_two.HospitalDatabase.Repositories.Abstracts;
using Domain_one.HospitalDatabase.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_three.Services.Concrete
{
    // EnginDemiroğ --> CategoryManager
    public class CategoryService:ICategoryService
    {
        ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        public IDataResult<List<Category>> GetAll()
        {
            return new SuccessDataResult<List<Category>>(_categoryRepository.GetAll(), Messages.Messages.CategorysListed);
        }

        public IDataResult<Category> GetById(Guid categoryId)
        {
            return new SuccessDataResult<Category>(_categoryRepository.Get(c => c.Id == categoryId), Messages.Messages.CategoryListed);
        }
    }
}
