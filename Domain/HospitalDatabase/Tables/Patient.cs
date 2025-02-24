using Core.Domain_one.Abstract;
using Domain_one.HospitalDatabase.Tables.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_one.HospitalDatabase.Tables
{
    public class Patient:BaseEntity,IEntity
    {
        public string PatientName { get; set; }
        public string IdentityNumber { get; set; }
        public string Phone { get; set; }


        [ForeignKey(nameof(Doctor))]
        public Guid UserId { get; set; }


        //Tablolar ile aralarındaki ilişki
        public ICollection<User> User { get; set; }
    }
}
