using Core.Domain_one.Abstract;
using Core.Models.EnumModels;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        //enum olarak sakladık (db string olarak tutuldu.)
        public Role UserRole;

        public string UserImgFile { get; set; }

        //Navigation Property
        //Bir kullanıcı hem doktor hem hasta olabilir
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }

    }
}
