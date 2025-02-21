using Core.Domain_one.Abstract;
using Core.Models.EnumModels.Role;
using Domain_one.HospitalDatabase.Tables.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_one.HospitalDatabase.Tables
{
    public class User:BaseEntity,IEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public Role UserRole;
        public string UserImgFile { get; set; }
    }
}
