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
    public class DoctorPatient:BaseEntity, IEntity
    {
        [ForeignKey(nameof(Doctor))]
        public Guid DoctorId { get; set; }

        [ForeignKey(nameof(Patient))]
        public Guid PatientId { get; set; }

    }
}
