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
    public class Doctor:BaseEntity, IEntity
    {
       
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }
        public string DoctorSpecialty { get; set; }

        [ForeignKey(nameof(User))] //bu ilişki FluentApı de verildi
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Polyclinic))]// bu ilişki FluentApı de verildi
        public Guid PolyclinicId { get; set; }


        //Tablolar arası ilişkiler 1-n n-n 1-1 gibi
        public Polyclinic Polyclinic { get; set; }
        public User User { get; set; }

        // Many-to-Many ilişki
        public ICollection<DoctorPatient> DoctorPatient { get; set; }

    }
}
