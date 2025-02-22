using Business_three.Services.Abstract;
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

        public List<Category> GetAll()
        {
            return new List<Category>(_categoryRepository.GetAll());
        }
    }
}
