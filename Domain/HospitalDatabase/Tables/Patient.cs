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


        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; }

        // 1 hasta, N randevu
        public ICollection<Appointment> Appointment { get; set; }
    }
}
